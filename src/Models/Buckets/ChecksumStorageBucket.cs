using System.Diagnostics;
using AlphabetUpdateServer.Models.ChecksumStorages;

namespace AlphabetUpdateServer.Models.Buckets;

public class ChecksumStorageBucket : IBucket
{
    public ChecksumStorageBucket(
        BucketLimitations limitations,
        IChecksumStorage checksumStorage)
    {
        LastUpdated = DateTimeOffset.MinValue;
        Limitations = limitations;
        this.checksumStorage = checksumStorage;
    }

    public DateTimeOffset LastUpdated { get; private set; }
    public BucketLimitations Limitations { get; set; }

    private IChecksumStorage checksumStorage;
    private IEnumerable<BucketFile> files { get; set; } = new List<BucketFile>();

    public ValueTask<IEnumerable<BucketFile>> GetFiles()
    {
        return new ValueTask<IEnumerable<BucketFile>>(files);
    }

    public async ValueTask<BucketSyncResult> Sync(IEnumerable<BucketSyncFile> syncFiles)
    {
        if (Limitations.IsReadOnly)
        {
            throw new BucketLimitationException(BucketLimitationException.ReadonlyBucket);
        }
        if (Limitations.ExpiredAt < DateTimeOffset.UtcNow)
        {
            throw new BucketLimitationException(BucketLimitationException.ExpiredBucket);
        }
        
        var actions = new List<BucketSyncAction>();
        var bucketFiles = new List<BucketFile>();
        var pathSet = new HashSet<string>();

        // (체크섬, 파일) 쌍 만들고 유효성 검사
        long totalSize = 0;
        long fileCount = 0;
        var requestChecksumFileMap = new Dictionary<string, BucketSyncFile>();
        foreach (var syncFile in syncFiles)
        {
            fileCount++;

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

            // 최대 버킷 크기 초과
            if (totalSize > Limitations.MaxBucketSize)
            {
                throw new BucketLimitationException(BucketLimitationException.ExceedMaxBucketSize);
            }
            // 최대 파일 개수 초과
            if (fileCount > Limitations.MaxNumberOfFiles)
            {
                throw new BucketLimitationException(BucketLimitationException.ExceedMaxNumberOfFiles);
            }
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
                        Path: requestFile.Path!,
                        Location: queryFile.Location,
                        Metadata: new FileMetadata(
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
            // 파일 유효성 검사가 끝난 파일만 map 에 남아있어야 함
            Debug.Assert(!string.IsNullOrEmpty(remainFile.Path));
            Debug.Assert(!string.IsNullOrEmpty(remainFile.Checksum));

            var action = checksumStorage.CreateSyncAction(remainFile.Checksum);
            actions.Add(new BucketSyncAction
            (
                Path: remainFile.Path,
                Action: action
            ));
        }
        if (actions.Any())
            return BucketSyncResult.ActionRequired(actions);

        // 모든 파일의 유효성 검사가 성공한 경우에만
        var result = BucketSyncResult.Success(updatedAt);
        await UpdateFiles(bucketFiles, updatedAt);
        return result;
    }

    public ValueTask UpdateFiles(IEnumerable<BucketFile> files, DateTimeOffset updatedAt)
    {
        this.files = files;
        LastUpdated = updatedAt;
        return new ValueTask();
    }
}