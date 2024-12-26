using System.ComponentModel;
using AlphabetUpdateServer.Models.Buckets;
using Microsoft.AspNetCore.Mvc;

namespace AlphabetUpdateServer.Pages.Web.Buckets;

public class BucketUsageModel
{
    public bool ShowLimitations { get; set; } = true;
    public BucketLimitations Limitations { get; set; }
    
    [DisplayName("사용중인 버킷 용량")]
    public long CurrentBucketSize { get; set; }
    
    [DisplayName("가장 큰 파일의 용량")]
    public long CurrentMaxFileSize { get; set; }
    
    [DisplayName("저장된 파일 수")]
    public int CurrentFileCount { get; set; }
    
    [DisplayName("이번달 동기화 횟수")]
    public int CurrentMonthlySyncCount { get; set; }
}