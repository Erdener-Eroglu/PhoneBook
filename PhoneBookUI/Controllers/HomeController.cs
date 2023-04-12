using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhoneBookBusinessLayer.InterfacesOfManagers;
using PhoneBookEntityLayer.Entities;
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
                if(!ModelState.IsValid)
                {
                    //Hata mesajı yazmadık
                    return View();
                }
                //1) Aynı telefondan var mı
                var samePhone = _memberPhoneManager.GetByConditions(x =>
                x.MemberId == model.MemberId && x.Phone == model.Phone).Data;
                if(samePhone != null)
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
    }
}