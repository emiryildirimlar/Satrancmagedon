using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using satranc_v27.Models.Entity;

namespace satranc_v27.Controllers
{
    public class SiparisDetayController : Controller
    {
        satranc_v26Entities db = new satranc_v26Entities();

        // GET: SiparisDetay
        public ActionResult Index()
        {
            var detaylar = db.SiparisDetaylari
                .Include(d => d.Siparisler)
                .Include(d => d.Urunler)
                .ToList();
            return View(detaylar);
        }

        // GET: SiparisDetay/Ekle
        [HttpGet]
        public ActionResult Ekle()
        {
            ViewBag.Siparisler = new SelectList(db.Siparisler.ToList(), "siparis_id", "siparis_id");
            ViewBag.Urunler = new SelectList(db.Urunler.ToList(), "urun_id", "urun_adi");
            return View();
        }

        // POST: SiparisDetay/Ekle
        [HttpPost]
        public ActionResult Ekle(SiparisDetaylari detay)
        {
            if (ModelState.IsValid)
            {
                db.SiparisDetaylari.Add(detay);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Siparisler = new SelectList(db.Siparisler.ToList(), "siparis_id", "siparis_id", detay.siparis_id);
            ViewBag.Urunler = new SelectList(db.Urunler.ToList(), "urun_id", "urun_adi", detay.urun_id);
            return View(detay);
        }

        // GET: SiparisDetay/Duzenle/5
        [HttpGet]
        public ActionResult Duzenle(int id)
        {
            var detay = db.SiparisDetaylari.Find(id);
            if (detay == null)
            {
                return HttpNotFound();
            }

            ViewBag.Siparisler = new SelectList(db.Siparisler.ToList(), "siparis_id", "siparis_id", detay.siparis_id);
            ViewBag.Urunler = new SelectList(db.Urunler.ToList(), "urun_id", "urun_adi", detay.urun_id);
            return View(detay);
        }

        // POST: SiparisDetay/Duzenle/5
        [HttpPost]
        public ActionResult Duzenle(SiparisDetaylari guncellenen)
        {
            var detay = db.SiparisDetaylari.Find(guncellenen.detay_id);
            if (detay == null)
            {
                return HttpNotFound();
            }

            detay.siparis_id = guncellenen.siparis_id;
            detay.urun_id = guncellenen.urun_id;
            detay.miktar = guncellenen.miktar;
            detay.birim_fiyat = guncellenen.birim_fiyat;

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: SiparisDetay/Sil/5
        public ActionResult Sil(int id)
        {
            var detay = db.SiparisDetaylari.Find(id);
            if (detay == null)
            {
                return HttpNotFound();
            }

            db.SiparisDetaylari.Remove(detay);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
