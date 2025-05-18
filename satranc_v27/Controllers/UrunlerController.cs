using System.Data.Entity;
using satranc_v27.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;



namespace satranc_v27.Controllers
{
    public class UrunlerController : Controller
    {
        // GET: Urunler
        satranc_v26Entities db = new satranc_v26Entities();

        public ActionResult Index(string arama, int? kategoriId)
        {
            var sorgu = db.Urunler
                .Include(x => x.Kategoriler)
                .Where(x => x.aktif == true);

            if (!string.IsNullOrEmpty(arama))
            {
                sorgu = sorgu.Where(u => u.urun_adi.Contains(arama));
            }

            if (kategoriId.HasValue)
            {
                sorgu = sorgu.Where(u => u.kategori_id == kategoriId.Value);
            }

            // Tüm kategorileri ViewBag ile gönder
            ViewBag.Kategoriler = db.Kategoriler.ToList();

            return View(sorgu.ToList());
        }


        // GET: Urunler/UrunEkle
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
            return RedirectToAction("Index");
        }

        // GET: Urunler/UrunDuzenle/5
        [HttpGet]
        public ActionResult UrunDuzenle(int id)
        {
            var urun = db.Urunler.Find(id);
            if (urun == null)
            {
                return HttpNotFound();
            }
            return View(urun);
        }

        // POST: Urunler/UrunDuzenle/5
        [HttpPost]
        public ActionResult UrunDuzenle(Urunler guncellenenUrun)
        {
           

            var urun = db.Urunler.Find(guncellenenUrun.urun_id);
            if (urun == null)
            {
                return HttpNotFound();
            }

            urun.urun_adi = guncellenenUrun.urun_adi;
            urun.kategori_id = guncellenenUrun.kategori_id;
            urun.fiyat = guncellenenUrun.fiyat;
            urun.stok_miktari = guncellenenUrun.stok_miktari;
            urun.aciklama = guncellenenUrun.aciklama;
            urun.gorsel_url = guncellenenUrun.gorsel_url;
           
            urun.aktif = guncellenenUrun.aktif;

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Urunler/UrunSil/5
        public ActionResult UrunSil(int id)
        {
            var urun = db.Urunler.Find(id);
            if (urun == null)
            {
                return HttpNotFound();
            }

            urun.aktif = false;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

       
    }
}