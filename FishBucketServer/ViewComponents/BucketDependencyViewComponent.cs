using AlphabetUpdateServer.Pages.Shared.Components.BucketDependency;
using AlphabetUpdateServer.Services.Buckets;
using Microsoft.AspNetCore.Mvc;

namespace AlphabetUpdateServer.ViewComponents;

public class BucketDependencyViewComponent : ViewComponent
{
    private readonly BucketService _bucketService;

    public BucketDependencyViewComponent(BucketService bucketService)
    {
        _bucketService = bucketService;
    }

    public IViewComponentResult Invoke(string id, bool showEdit)
    {
        var dependencies = _bucketService.GetDependencies(id);
        return View(new BucketDependencyModel
        {
            Dependencies = dependencies,
            ShowEdit = showEdit
        });
    }
}