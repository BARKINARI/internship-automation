using Microsoft.AspNetCore.Mvc;

namespace WebUI.ViewComponents.AdminLayoutViewComponents;

public class _AdminLayoutHeadComponentPartial : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View();
    }
}