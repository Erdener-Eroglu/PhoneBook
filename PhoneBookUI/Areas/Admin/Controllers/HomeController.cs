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
            var lastMember = _memberManager.GetAll().Data.OrderBy(x=>x.CreatedDate).LastOrDefault();
            ViewBag.LastMember = $"{lastMember?.Name} {lastMember?.Surname}";

            //Rehbere son eklenen kişi
            var lastContact = _memberPhoneManager.GetAll().Data.LastOrDefault();
            ViewBag.LastContact = lastContact?.FriendNameSurname;


            return View();
        }
        
        [Route("/admin/GetPhoneTypePieData")]
        public JsonResult GetPhoneTypePieData()
        {
            try
            {
                Dictionary<string, int> model = new Dictionary<string, int>();
                var data = _memberPhoneManager.GetAll().Data;
                foreach (var item in data)
                {
                    var count = 1;
                    if (!model.ContainsKey(item.PhoneType.Name))
                    {
                        model.Add(item.PhoneType.Name, count);
                    }
                    else
                    {
                        model[item.PhoneType.Name]++;
                    }
                }
                return Json(new { isSuccess = true, message = "Veriler geldi", types = model.Keys.ToArray(), points = model.Values.ToArray() });

            }
            catch (Exception ex)
            {
                return Json(new { isSuccess = false, message = "Veriler getirilemedi!" });
            }
        }

        [HttpGet]
        [Route("uyeler")]
        public IActionResult MemberIndex()
        {
            try
            {
                var data = _memberManager.GetAll().Data;

                return View(data);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Beklenmedik bir hata oluştu! " + ex.Message);
                return View();
            }
        }
    }
}
