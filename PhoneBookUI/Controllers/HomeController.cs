using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhoneBookBusinessLayer.InterfacesOfManagers;
using PhoneBookEntityLayer.ViewModels;
using PhoneBookUI.Models;
using System.Diagnostics;

namespace PhoneBookUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IPhoneTypeManager _phoneTypeManager;
        private readonly IMemberPhoneManager _memberPhoneManager;

        public HomeController(ILogger<HomeController> logger, IPhoneTypeManager phoneTypeManager, IMemberPhoneManager memberPhoneManager)
        {
            _logger = logger;
            _phoneTypeManager = phoneTypeManager;
            _memberPhoneManager = memberPhoneManager;
        }

        public IActionResult Index()
        {
            //eğer giriş yapmış ise giriş yapan kullanıcının rehberini mosel olarak sayfaya gönderelim.
            if (HttpContext.User.Identity?.Name != null)
            {
                var userEmail = HttpContext.User.Identity?.Name;
                var data = _memberPhoneManager.GetAll(x => x.MemberId == userEmail).Data;
                return View(data);
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        [Authorize] //Authorize login olmadan sayfaya erişimi önler.
        public IActionResult AddPhone()
        {
            try
            {
                ViewBag.PhoneTypes = _phoneTypeManager.GetAll().Data; //NOT: isRemoved viewmodelin içine eklensin.
                MemberPhoneViewModel model = new MemberPhoneViewModel()
                {
                    MemberId = HttpContext.User.Identity?.Name
                };
                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.PhoneTypes = new List<PhoneTypeViewModel>();
                ModelState.AddModelError("", "Beklenmedik bir hata oluştu" + ex.Message);
                return View();
            }
        }

        [HttpPost]
        [Authorize]
        public IActionResult AddPhone(MemberPhoneViewModel model)
        {
            try
            {
                ViewBag.PhoneTypes = _phoneTypeManager.GetAll().Data;
                if (!ModelState.IsValid)
                {
                    //Hata mesajı yazmadık
                    return View();
                }
                //1) Aynı telefondan var mı
                var samePhone = _memberPhoneManager.GetByConditions(x =>
                x.MemberId == model.MemberId && x.Phone == model.Phone).Data;
                if (samePhone != null)
                {
                    ModelState.AddModelError("", $"Bu telefon {samePhone.PhoneType.Name} türünde zaten eklenmiştir.");
                    return View(model);
                }
                //2) Telefonu ekle
                //Diğer seçeneğinin senaryosunu yarın yazacağız.
                model.CreatedDate = DateTime.Now;
                model.IsRemoved = false;
                if (_memberPhoneManager.Add(model).IsSuccess == false)
                {
                    ViewBag.PhoneTypes = new List<PhoneTypeViewModel>();
                    ModelState.AddModelError("", "Ekleme başarısız Tekrar deneyiniz!");
                    return View(model);
                }
                TempData["AddPhoneSuccessMsg"] = $"Yeni Numara Rehbere Eklendi";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ViewBag.PhoneTypes = new List<PhoneTypeViewModel>();
                ModelState.AddModelError("", "Beklenmedik bir hata oluştu" + ex.Message);
                return View();
            }
        }


        [Authorize]
        public IActionResult DeletePhone(int id)
        {
            try
            {
                if (id <= 0)
                {
                    TempData["DeleteFailedMsg"] = "Id değeri düzgün değil";
                    return RedirectToAction("Index", "Home");
                }
                var phone = _memberPhoneManager.GetById(id).Data;
                if (phone == null)
                {
                    TempData["DeleteFailedMsg"] = "Kayıt bulunamadığı için silme başarısızdır.";
                    return RedirectToAction("Index", "Home");

                }
                if (!_memberPhoneManager.Delete(phone).IsSuccess)
                {
                    TempData["DeleteFailedMsg"] = "Silme Başarısızdır!";
                    return RedirectToAction("Index", "Home");

                }
                TempData["DeleteSuccessMsg"] = "Telefon rehberden silindi";
                return RedirectToAction("Index", "Home");

            }
            catch (Exception ex)
            {

                TempData["DeleteFailedMsg"] = "Beklenmedik bir hata oldu !" + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public JsonResult PhoneDelete([FromBody] int id)
        {
            try
            {
                if (id <= 0)
                {
                    return Json(new { isSuccess = false, message = "Id değeri düzgün değil" });

                }
                var phone = _memberPhoneManager.GetById(id).Data;
                if (phone == null)
                {
                    return Json(new { isSuccess = false, message = "Kayıt bulunamadığı için silme başarısızdır." });
                }
                //hard delete
                if (!_memberPhoneManager.Delete(phone).IsSuccess)
                {
                    return Json(new { isSuccess = false, message = "Silme Başarısızdır!" });
                }
                var userEmail = HttpContext.User.Identity?.Name;
                var data = _memberPhoneManager.GetAll(x => x.MemberId == userEmail).Data;
                return Json(new { isSuccess = true, message = "Telefon rehberden silindi", phones = data });


            }
            catch (Exception ex)
            {

                return Json(new { isSuccess = false, message = $"Beklenmedik bir hata oluştu! {ex.Message}" });
            }
        }

        [HttpGet]
        [Authorize]
        public IActionResult EditPhone(int id)
        {
            try
            {
                //zaman azaldığı için buraya if yazıp id kontrol edilmedi
                var phone = _memberPhoneManager.GetById(id).Data;
                return View(phone);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Beklenmedik bir hata! " + ex.Message);
                return View();
            }
        }

        [HttpPost]
        [Authorize]
        public IActionResult EditPhone(MemberPhoneViewModel model)
        {
            try
            {
                //zaman azaldığı için buraya if yazıp id kontrol edilmedi
                var phone = _memberPhoneManager.GetById(model.Id).Data;
                phone.Phone = model.Phone;
                phone.FriendNameSurname = model.FriendNameSurname;
                _memberPhoneManager.Update(phone);
                return RedirectToAction("Index","Home");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Beklenmedik bir hata! " + ex.Message);
                return View();
            }
        }
    }
}