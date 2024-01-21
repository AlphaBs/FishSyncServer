using AlphabetUpdateServer.Models.ChecksumStorages;

namespace AlphabetUpdateServer.Models.Buckets;

public class ChecksumBaseBucket
{
    public ChecksumBaseBucket(
        string id,
        DateTimeOffset lastUpdated,
        BucketLimitations limitations)
    {
        Id = id;
        LastUpdated = lastUpdated;
        Limitations = limitations;
    }

    public string Id { get; }
    public DateTimeOffset LastUpdated { get; private set; }
    public BucketLimitations Limitations { get; set; }
    public virtual IEnumerable<BucketFile> Files { get; private set; } = new List<BucketFile>();

    public async IAsyncEnumerable<BucketFileLocation> GetFiles(IFileChecksumStorage checksumStorage)
    {
        // IFileChecksumStorage 에서 파일의 실제 위치를 찾고 파일 목록을 반환함
        var checksumFileMap = Files.ToDictionary(f => f.Metadata.Checksum, f => f);

        // 찾아야 할 체크섬 전체를 질의해서 실제 파일의 위치 찾기
        var checksumLocations = checksumStorage.Query(checksumFileMap.Keys);
        await foreach (var checksumLocation in checksumLocations)
        {
            var fileEntity = checksumFileMap[checksumLocation.Metadata.Checksum];
            checksumFileMap.Remove(checksumLocation.Metadata.Checksum);

            // 파일 위치랑 메타데이터 반환
            yield return new BucketFileLocation(
                Path: fileEntity.Path,
                Location: checksumLocation.Location ?? string.Empty,
                Metadata: fileEntity.Metadata);
        }

        // ChecksumStorage 에서 찾지 못한 파일은 Location 을 null 로 하여 반환
        foreach (var file in checksumFileMap.Values)
        {
            yield return new BucketFileLocation(
                Path: file.Path,
                Location: null,
                Metadata: file.Metadata);
        }
    }

    public async ValueTask<BucketSyncResult> Sync(
        IEnumerable<BucketSyncFile> syncFiles, 
        IFileChecksumStorage checksumStorage)
    {
        if (Limitations.IsReadOnly)
        {
            return BucketSyncResult.ActionRequired(BucketSyncActionFactory.ReadOnlyBucket());
        }
        if (Limitations.ExpiredAt < DateTimeOffset.UtcNow)
        {
            return BucketSyncResult.ActionRequired(BucketSyncActionFactory.ExpiredBucket());
        }
        
        var actions = new List<BucketSyncAction>();
        var bucketFiles = new List<BucketFile>();
        var pathSet = new HashSet<string>();

        // (체크섬, 파일) 쌍 만들고 유효성 검사
        long totalSize = 0;
        var requestChecksumFileMap = new Dictionary<string, BucketSyncFile>();
        foreach (var syncFile in syncFiles)
        {
            // validation
            if (string.IsNullOrEmpty(syncFile.Checksum) ||
                string.IsNullOrEmpty(syncFile.Path) ||
                syncFile.Size < 0)
            {
                actions.Add(BucketSyncActionFactory.InvalidFileSize(syncFile));
            }
            // 최대 파일 크기 초과
            else if (syncFile.Size > Limitations.MaxFileSize)
            {
                actions.Add(BucketSyncActionFactory.ExceedMaxFileSize(syncFile));
            }
            // 중복 경로 검사
            else if (!pathSet.Add(syncFile.Path))
            {
                actions.Add(BucketSyncActionFactory.DuplicatedFilePath(syncFile));
            }
            else
            {
                totalSize += syncFile.Size;
                requestChecksumFileMap[syncFile.Checksum] = syncFile;
            }
        }
        // 최대 버킷 크기 초과
        if (totalSize > Limitations.MaxBucketSize)
        {
            actions.Add(BucketSyncActionFactory.ExceedMaxBucketSize());
        }
        // early return
        if (actions.Any())
        {
            return BucketSyncResult.ActionRequired(actions);
        }

        // 동기화 요청한 파일과 IFileChecksumStorage 에 등록된 파일과 비교
        var queryFiles = checksumStorage.Query(requestChecksumFileMap.Keys);
        var updatedAt = DateTimeOffset.UtcNow;

        await foreach (var queryFile in queryFiles)
        {
            if (requestChecksumFileMap.TryGetValue(queryFile.Metadata.Checksum, out var requestFile))
            {
                if (requestFile.Size != queryFile.Metadata.Size) // 메타데이터 비교
                {
                    actions.Add(BucketSyncActionFactory.WrongFileSize(requestFile));
                }
                else
                {
                    bucketFiles.Add(new BucketFile(
                        BucketId: Id,
                        Path: requestFile.Path!,
                        Metadata: new BucketFileMetadata(
                            Size: queryFile.Metadata.Size,
                            LastUpdated: updatedAt,
                            Checksum: queryFile.Metadata.Checksum
                        )));
                }

                // 찾은 파일은 map 에서 전부 지우고 못찾은 파일만 map 에 남겨둠
                requestChecksumFileMap.Remove(queryFile.Metadata.Checksum);
            }
        }

        // IFileChecksumStorage 에서 찾을 수 없는 파일은 따로 작업 필요
        foreach (var remainFile in requestChecksumFileMap.Values)
        {
            var action = checksumStorage.CreateSyncAction(remainFile);
            actions.Add(action);
        }
        if (actions.Any())
            return BucketSyncResult.ActionRequired(actions);

        // 모든 파일의 유효성 검사가 성공한 경우에만
        var result = BucketSyncResult.Success(updatedAt);
        UpdateFiles(bucketFiles, updatedAt);
        return result;
    }

    public void UpdateFiles(IEnumerable<BucketFile> files, DateTimeOffset updatedAt)
    {
        Files = files;
        LastUpdated = updatedAt;
    }
}