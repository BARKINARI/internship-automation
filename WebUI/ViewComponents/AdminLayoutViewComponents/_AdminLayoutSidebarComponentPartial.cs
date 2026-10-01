using Microsoft.AspNetCore.Mvc;

namespace WebUI.ViewComponents.AdminLayoutViewComponents;

public class _AdminLayoutSidebarComponentPartial : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View();
    }
}