using System.Diagnostics;
using System.Linq;
using AlphabetUpdateServer.Models.Buckets.SyncActions;
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
        var requestChecksumFileMap = new Dictionary<string, List<BucketSyncFile>>();
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
                if (!requestChecksumFileMap.TryGetValue(syncFile.Checksum, out var syncFileList))
                    requestChecksumFileMap[syncFile.Checksum] = syncFileList = new List<BucketSyncFile>();
                syncFileList.Add(syncFile);
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

        // 동기화 요청한 파일과 ChecksumStorage 에 등록된 파일과 비교
        var syncResult = await checksumStorage.Sync(requestChecksumFileMap.Keys);
        var updatedAt = DateTimeOffset.UtcNow;

        // ChecksumStorage 에서 찾은 파일
        foreach (var checksumStorageFile in syncResult.SuccessFiles)
        {
            if (requestChecksumFileMap.TryGetValue(checksumStorageFile.Checksum, out var requestFiles))
            {
                foreach (var requestFile in requestFiles)
                {
                    if (requestFile.Size != checksumStorageFile.Metadata.Size) // 메타데이터 비교
                    {
                        actions.Add(BucketSyncActionFactory.WrongFileSize(requestFile));
                    }
                    else
                    {
                        bucketFiles.Add(new BucketFile
                        (
                            Path: requestFile.Path!,
                            Location: checksumStorageFile.Location,
                            Metadata: new FileMetadata(
                            Size: checksumStorageFile.Metadata.Size,
                            LastUpdated: updatedAt,
                            Checksum: checksumStorageFile.Metadata.Checksum
                        )));
                    }
                }

                // 찾은 파일은 map 에서 전부 지우고 못찾은 파일만 map 에 남겨둠
                requestChecksumFileMap.Remove(checksumStorageFile.Metadata.Checksum);
            }
        }

        // ChecksumStorage 에서 SyncAction 이 필요한 파일
        foreach (var action in syncResult.RequiredActions)
        {
            if (requestChecksumFileMap.TryGetValue(action.Checksum, out var requestFiles))
            {
                foreach (var requestFile in requestFiles)
                {
                    actions.Add(new BucketSyncAction(requestFile.Path!, action.Action));
                    requestChecksumFileMap.Remove(action.Checksum);
                }
            }
        }

        // ChecksumStorage 에서 처리하지 못한 파일
        foreach (var remainFile in requestChecksumFileMap.Values.SelectMany(list => list))
        {
            // 파일 유효성 검사가 끝난 파일만 map 에 남아있어야 함
            Debug.Assert(!string.IsNullOrEmpty(remainFile.Path));
            Debug.Assert(!string.IsNullOrEmpty(remainFile.Checksum));

            actions.Add(BucketSyncActionFactory.UnknownError(remainFile));
        }

        // SyncAction 필요없는 경우, 파일 목록 업데이트 가능
        if (actions.Any())
        {
            return BucketSyncResult.ActionRequired(actions);
        }
        else
        {
            var result = BucketSyncResult.Success(updatedAt);
            await UpdateFiles(bucketFiles, updatedAt);
            return result;
        }
    }

    public ValueTask UpdateFiles(IEnumerable<BucketFile> files, DateTimeOffset updatedAt)
    {
        this.files = files;
        LastUpdated = updatedAt;
        return new ValueTask();
    }
}