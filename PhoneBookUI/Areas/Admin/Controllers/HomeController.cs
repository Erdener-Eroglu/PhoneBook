using Microsoft.AspNetCore.Mvc;

namespace PhoneBookUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("a/[Controller]/[Action]/{id?}")]
    public class HomeController : Controller
    {
        public IActionResult Dashboard()
        {
            //Temayı giydirip gelcez.
            return View();
        }
    }
}
