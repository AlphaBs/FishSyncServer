using System.ComponentModel;

namespace AlphabetUpdateServer.Models.Buckets;

public class BucketLimitations
{
    public static readonly BucketLimitations NoLimits = new BucketLimitations
    {
        IsReadOnly = false,
        MaxFileSize = long.MaxValue,
        MaxNumberOfFiles = long.MaxValue,
        MaxBucketSize = long.MaxValue,
        ExpiredAt = DateTimeOffset.MaxValue,
        MonthlyMaxSyncCount = int.MaxValue
    };

    [DisplayName("읽기 전용")]
    public bool IsReadOnly { get; set; }
    
    [DisplayName("최대 파일 크기")]
    public long MaxFileSize { get; set; }
    
    [DisplayName("최대 파일 수")]
    public long MaxNumberOfFiles { get; set; }
    
    [DisplayName("최대 버킷 용량")]
    public long MaxBucketSize { get; set; }
    
    [DisplayName("사용 만료일")]
    public DateTimeOffset ExpiredAt { get; set; }
    
    [DisplayName("월별 최대 동기화 횟수")]
    public int MonthlyMaxSyncCount { get; set; }
}