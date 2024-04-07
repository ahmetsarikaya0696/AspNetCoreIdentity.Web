using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreIdentity.Web.Controllers
{
    public class OrdersController : Controller
    {
        [Authorize(Policy = "PermissionOrderReadOrderDeleteStockDeletePolicy")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
