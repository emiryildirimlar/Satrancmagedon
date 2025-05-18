using satranc_v27.Models.Entity;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace satranc_v27.Controllers
{
    public class AdminController : Controller
    {
        satranc_v26Entities db = new satranc_v26Entities(); // kendi veritabanı context'in

        // Admin Giriş Sayfası
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        // Giriş Kontrol
        [HttpPost]
        public ActionResult Login(string email, string sifre)
        {
            var admin = db.Admin.FirstOrDefault(x => x.email == email && x.sifre == sifre && x.aktif == true);
            if (admin != null)
            {
                FormsAuthentication.SetAuthCookie(admin.email, false);
                Session["AdminEmail"] = admin.email;
                Session["AdminAdSoyad"] = admin.ad + " " + admin.soyad;
                return RedirectToAction("Index", "Urunler"); // giriş sonrası yönlendirme

                
            }
            else
            {
                TempData["Error"] = "Hatalı e-posta veya şifre!";
                return RedirectToAction("Login");
            }
        }

        // Admin Paneli Ana Sayfası
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        // Çıkış
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("Login", "Admin");
        }
    }
}