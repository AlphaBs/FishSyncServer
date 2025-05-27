using AlphabetUpdateServer.Pages.Shared.Components.BucketOwner;
using AlphabetUpdateServer.Services.Users;
using Microsoft.AspNetCore.Mvc;

namespace AlphabetUpdateServer.ViewComponents;

public class BucketOwnerViewComponent : ViewComponent
{
    private readonly BucketOwnerService _ownerService;

    public BucketOwnerViewComponent(BucketOwnerService ownerService)
    {
        _ownerService = ownerService;
    }

    public IViewComponentResult Invoke(string id, bool showEdit)
    {
        var owners = _ownerService.GetOwners(id);
        return View(new BucketOwnerModel
        {
            Owners = owners,
            ShowEdit = showEdit
        });
    }
}