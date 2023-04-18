using Microsoft.AspNetCore.Mvc;
using PhoneBookBusinessLayer.InterfacesOfManagers;
using PhoneBookUI.Areas.Admin.Models;

namespace PhoneBookUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Route("a/[Controller]/[Action]/{id?}")] bu route verildiğinde [action] yazan yere action'ın tam adını yazmadan sayfa açılmaz
    [Route("admin")] // bu route verildiğinde controller'a nasıl ulaşıldığı belirtilir ve action'a ulaşılma konusundaki  kuralı action üzerinde yazılan kural belirler.
    public class HomeController : Controller
    {
        private readonly IMemberManager _memberManager;
        private readonly IPhoneTypeManager _phoneTypeManager;
        private readonly IMemberPhoneManager _memberPhoneManager;

        public HomeController(IMemberManager memberManager, IPhoneTypeManager phoneTypeManager, IMemberPhoneManager memberPhoneManager)
        {
            _memberManager = memberManager;
            _phoneTypeManager = phoneTypeManager;
            _memberPhoneManager = memberPhoneManager;
        }

        [Route("dsh")] //Action'un ismi çok uzun olabilir url'e action'ın isminin hepsini yazmak istemezsek action'a Route verebiliriz.
        public IActionResult Dashboard()
        {
            //Bu ay sisteme kayıt olan üye sayısı.
            DateTime thisMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            ViewBag.MontlyMemberCount = _memberManager.GetAll(x => x.CreatedDate > thisMonth.AddDays(-1)).Data.Count;
            //Bu ay sisteme eklenen numara sayısı.
            ViewBag.MontlyContactCount = _memberPhoneManager.GetAll(x => x.CreatedDate > thisMonth.AddDays(-1)).Data.Count;

            //En son eklenen üyenin adı soyadı
            var lastMember = _memberManager.GetAll().Data.LastOrDefault();
            ViewBag.LastMember = $"{lastMember?.Name} {lastMember?.Surname}";

            //Rehbere son eklenen kişi
            var lastContact = _memberPhoneManager.GetAll().Data.LastOrDefault();
            ViewBag.LastContact = lastContact?.FriendNameSurname;


            return View();
        }
    }
}
