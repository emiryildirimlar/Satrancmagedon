using satranc_v27.Models.Entity;
using System.Collections.Generic;
using System.Data.Entity;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;


namespace satranc_v27.Controllers
{
    public class AlicilarController : Controller
    {
        satranc_v26Entities db = new satranc_v26Entities(); // Veritabanı context'iniz

        [Authorize]
        public ActionResult Index(int? kategoriId, string arama)
        {
            var urunler = db.Urunler
     .Include(u => u.Kategoriler)
     .Include(u => u.Saticilar) 
     .Where(u => u.aktif == true);

            if (kategoriId.HasValue)
            {
                urunler = urunler.Where(u => u.kategori_id == kategoriId.Value);
            }

            if (!string.IsNullOrEmpty(arama))
            {
                urunler = urunler.Where(u => u.urun_adi.Contains(arama)); // ürün adında arama
            }
            ViewBag.Kategoriler = db.Kategoriler.ToList();
            ViewBag.AktifKategoriId = kategoriId; 
            return View(urunler.ToList());
        }
        [Authorize]
        public ActionResult UrunDetay(int id)
        {
            var urun = db.Urunler.Find(id);
            if (urun == null)
            {
                return HttpNotFound();
            }
            return View(urun);
        }

        // Alıcı Giriş Sayfası
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        // Giriş Kontrol
        [HttpPost]
        public ActionResult Login(string email, string sifre)
        {
            var alici = db.Alicilar.FirstOrDefault(x => x.email == email && x.sifre == sifre && x.aktif == true);
            if (alici != null)
            {
                FormsAuthentication.SetAuthCookie(alici.email, false);
                Session["AliciEmail"] = alici.email;
                Session["AliciAdSoyad"] = alici.ad + " " + alici.soyad;
                Session["AliciId"] = alici.alici_id;
                return RedirectToAction("Index", "Alicilar"); // Giriş sonrası yönlendirme
            }
            else
            {
                ViewBag.Hata = "Hatalı e-posta veya şifre!";
                return View();
            }
        }

        // Kayıt Sayfası
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        // Kayıt İşlemi
        [HttpPost]
        public ActionResult Register(Alicilar alici)
        {
            if (ModelState.IsValid)
            {
                // Email kontrolü
                if (db.Alicilar.Any(x => x.email == alici.email))
                {
                    ViewBag.Hata = "Bu e-posta adresi zaten kayıtlı!";
                    return View(alici);
                }

                alici.kayit_tarihi = DateTime.Now;
                alici.aktif = true;

                db.Alicilar.Add(alici);
                db.SaveChanges();

                // Otomatik giriş yap
                FormsAuthentication.SetAuthCookie(alici.email, false);
                Session["AliciEmail"] = alici.email;
                Session["AliciAdSoyad"] = alici.ad + " " + alici.soyad;
                Session["AliciId"] = alici.alici_id;

                return RedirectToAction("Login", "Alicilar");
            }

            return View(alici);
        }

        // Çıkış
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("Login", "Alicilar");
        }

        // Alıcı Profil Sayfası
        [Authorize]
        public ActionResult Profile()
        {
            int aliciId = (int)Session["AliciId"];
            var alici = db.Alicilar.Find(aliciId);
            return View(alici);
        }

        // Profil Güncelleme
        [Authorize]
        [HttpPost]
        public ActionResult Profile(Alicilar updatedAlici)
        {
            if (ModelState.IsValid)
            {
                int aliciId = (int)Session["AliciId"];
                var alici = db.Alicilar.Find(aliciId);

                // Güncelleme işlemleri
                alici.ad = updatedAlici.ad;
                alici.soyad = updatedAlici.soyad;
                alici.telefon = updatedAlici.telefon;
                alici.adres = updatedAlici.adres;

                db.SaveChanges();

                Session["AliciAdSoyad"] = alici.ad + " " + alici.soyad;
                ViewBag.Basarili = "Profil bilgileriniz güncellendi!";
            }

            return View(updatedAlici);
        }
        [Authorize]
        public ActionResult Siparislerim()
        {
            int? aliciId = Session["AliciId"] as int?;
            if (aliciId == null)
            {
                return RedirectToAction("Login", "Alicilar");
            }

            var siparisler = db.Siparisler
                .Where(s => s.alici_id == aliciId)
                .OrderByDescending(s => s.siparis_tarihi)
                .ToList();

            return View(siparisler);
        }


    }
}