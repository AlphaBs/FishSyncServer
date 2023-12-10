using AlphabetUpdateServer.Entities;
using AlphabetUpdateServer.Models.ChecksumStorages;

namespace AlphabetUpdateServer.Models.Buckets;

public class BucketSyncProcessor
{
    public static async ValueTask<(BucketSyncResult, IEnumerable<BucketFileEntity>)> Sync(
        string id,
        IFileChecksumStorage checksumStorage,
        IEnumerable<BucketSyncFile> files)
    {
        var actions = new List<BucketSyncAction>();
        var actionFactory = new BucketSyncActionFactory();
        var fileEntities = new List<BucketFileEntity>();
        var pathSet = new HashSet<string>();

        // (체크섬, 파일) 쌍 만들고 유효성 검사
        var requestChecksumFileMap = new Dictionary<string, BucketSyncFile>();
        foreach (var file in files)
        {
            // validation
            if (string.IsNullOrEmpty(file.Checksum) ||
                string.IsNullOrEmpty(file.Path) ||
                file.Size < 0)
            {
                throw new ArgumentException(nameof(files));
            }

            // 중복 경로 검사
            if (!pathSet.Add(file.Path))
            {
                actions.Add(actionFactory.DuplicatedFilePath(file));
            }

            requestChecksumFileMap[file.Checksum] = file;
        }
        if (actions.Any())
            return (BucketSyncResult.ActionRequired(actions), Enumerable.Empty<BucketFileEntity>());

        // 동기화 요청한 파일과 IFileChecksumStorage 에 등록된 파일과 비교
        var queryFiles = checksumStorage.Query(requestChecksumFileMap.Keys);
        var updated = DateTimeOffset.UtcNow;

        await foreach (var queryFile in queryFiles)
        {
            if (requestChecksumFileMap.TryGetValue(queryFile.Checksum, out var requestFile))
            {
                if (requestFile.Size != queryFile.Size) // 메타데이터 비교
                {
                    actions.Add(actionFactory.FileSizeValidation(requestFile));
                }
                else
                {
                    fileEntities.Add(new BucketFileEntity
                    {
                        BucketId = id,
                        Path = requestFile.Path!,
                        Size = queryFile.Size,
                        LastUpdated = updated,
                        Checksum = queryFile.Checksum
                    });
                }

                // 찾은 파일은 map 에서 전부 지우고 못찾은 파일만 map 에 남겨둠
                requestChecksumFileMap.Remove(queryFile.Checksum);
            }
        }

        // IFileChecksumStorage 에서 찾을 수 없는 파일은 따로 작업 필요
        foreach (var remainHash in requestChecksumFileMap.Keys)
        {
            var action = checksumStorage.CreateSyncAction(remainHash);
            actions.Add(action);
        }
        if (actions.Any())
            return (BucketSyncResult.ActionRequired(actions), Enumerable.Empty<BucketFileEntity>());

        // 모든 파일의 유효성 검사가 성공한 경우에만
        return (BucketSyncResult.Success(updated), fileEntities);
    }
}