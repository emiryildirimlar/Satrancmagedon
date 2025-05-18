using satranc_v27.Models.Entity;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;

public class SaticilarController : Controller
{
    satranc_v26Entities db = new satranc_v26Entities();
    // GET: Satici/Login
    public ActionResult Login()
    {
        return View();
    }

    // POST: Satici/Login
    [HttpPost]
    public ActionResult Login(string email, string sifre)
    {
        var satici = db.Saticilar
                       .FirstOrDefault(x => x.email == email && x.sifre == sifre && x.aktif == true);

        if (satici != null)
        {
            // Login başarılı, oturum aç
            Session["satici_id"] = satici.satici_id;
            Session["satici_ad"] = satici.ad;
            return RedirectToAction("Panel", "Saticilar"); // girişten sonra yönleneceği yer
        }
        else
        {
            ViewBag.Mesaj = "Hatalı e-posta veya şifre";
            return View();
        }
    }
        public ActionResult Panel()
        {
            if (Session["satici_id"] == null)
                return RedirectToAction("Login");

            int saticiId = Convert.ToInt32(Session["satici_id"]);

            var urunler = db.Urunler
                            .Where(u => u.satici_id == saticiId && u.aktif == true)
                            .ToList();

            return View(urunler);
        }

    // GET: Saticilar/UrunDuzenle/5
    [HttpGet]
    public ActionResult UrunDuzenle(int id)
    {
        var urun = db.Urunler.Find(id);

        // Giriş yapan satıcının ID'sini al (örnek: Session kullanıyorsan)
        int? saticiId = Session["satici_id"] as int?;
        if (urun == null || urun.satici_id != saticiId)
        {
            return HttpNotFound();
        }

        return View(urun);
    }

    // POST: Saticilar/UrunDuzenle/5
    [HttpPost]
    public ActionResult UrunDuzenle(Urunler guncellenenUrun)
    {
        var urun = db.Urunler.Find(guncellenenUrun.urun_id);
        int? saticiId = Session["satici_id"] as int?;

        if (urun == null || urun.satici_id != saticiId)
        {
            return HttpNotFound();
        }

        // Güncelleme işlemleri
        urun.urun_adi = guncellenenUrun.urun_adi;
        urun.kategori_id = guncellenenUrun.kategori_id;
        urun.fiyat = guncellenenUrun.fiyat;
        urun.stok_miktari = guncellenenUrun.stok_miktari;
        urun.aciklama = guncellenenUrun.aciklama;
        urun.gorsel_url = guncellenenUrun.gorsel_url;
        urun.aktif = guncellenenUrun.aktif;

        db.SaveChanges();

        return RedirectToAction("Panel");
    }

    public ActionResult UrunSil(int id)
    {
        var urun = db.Urunler.Find(id);
        if (urun == null)
        {
            return HttpNotFound();
        }

        urun.aktif = false;
        db.SaveChanges();
        return RedirectToAction("Panel");
    }


    [HttpGet]
    public ActionResult UrunEkle()
    {
        return View();
    }

    // POST: Urunler/UrunEkle
    [HttpPost]
    public ActionResult UrunEkle(Urunler urun)
    {

        urun.eklenme_tarihi = DateTime.Now;
        urun.aktif = true;
        db.Urunler.Add(urun);
        db.SaveChanges();
        return RedirectToAction("Panel");
    }

    public ActionResult Siparislerim()
    {
        if (Session["satici_id"] == null)
            return RedirectToAction("Login");

        int saticiId = Convert.ToInt32(Session["satici_id"]);

        var urunIdListesi = db.Urunler
                              .Where(u => u.satici_id == saticiId && u.aktif == true)
                              .Select(u => u.urun_id)
                              .ToList();

        var siparisIdListesi = db.SiparisDetaylari
                                 .Where(d => d.urun_id.HasValue && urunIdListesi.Contains(d.urun_id.Value))
                                 .Select(d => d.siparis_id)
                                 .Distinct()
                                 .ToList();

        var siparisler = db.Siparisler
                           .Where(s => siparisIdListesi.Contains(s.siparis_id))
                           .ToList();

        return View(siparisler);
    }


    public ActionResult SiparisSil(int id)
    {
        var siparis = db.Siparisler.Find(id);
        if (siparis == null)
        {
            return HttpNotFound();
        }

        // Dilersen buraya yetki kontrolü de eklersin (satici_id == Session'daki id mi diye)
        db.Siparisler.Remove(siparis);
        db.SaveChanges();
        return RedirectToAction("Siparislerim");
    }

    [HttpGet]
    public ActionResult SiparisDuzenle(int id)
    {
        var siparis = db.Siparisler.Find(id);
        if (siparis == null)
        {
            return HttpNotFound();
        }

        // Dilersen giriş yapan satıcının kontrolünü yap
        return View(siparis);
    }

    [HttpPost]
    public ActionResult SiparisDuzenle(Siparisler guncellenenSiparis)
    {
        var siparis = db.Siparisler.Find(guncellenenSiparis.siparis_id);
        if (siparis == null)
        {
            return HttpNotFound();
        }

        siparis.odeme_durumu = guncellenenSiparis.odeme_durumu;
        siparis.kargo_durumu = guncellenenSiparis.kargo_durumu;
        siparis.teslimat_adresi = guncellenenSiparis.teslimat_adresi;
        // Diğer alanları da eklersin gerekirse

        db.SaveChanges();
        return RedirectToAction("Siparislerim");
    }

    public ActionResult Logout()
    {
        // Oturumdaki verileri temizle
        Session.Clear();
        Session.Abandon();

        // Giriş ekranına yönlendir
        return RedirectToAction("Login", "Saticilar");
    }
}

