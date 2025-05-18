using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using satranc_v27.Models.Entity;

namespace satranc_v27.Controllers
{
    public class KategorilerController : Controller
    {
        satranc_v26Entities db = new satranc_v26Entities();

        // GET: Kategoriler
        public ActionResult Index()
        {
            var kategoriler = db.Kategoriler.ToList();
            return View(kategoriler);
        }

        // GET: Kategoriler/KategoriEkle
        [HttpGet]
        public ActionResult KategoriEkle()
        {
            return View();
        }

        // POST: Kategoriler/KategoriEkle
        [HttpPost]
        public ActionResult KategoriEkle(Kategoriler kategori)
        {
            kategori.aktif = true;
            db.Kategoriler.Add(kategori);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Kategoriler/KategoriDuzenle/5
        [HttpGet]
        public ActionResult KategoriDuzenle(int id)
        {
            var kategori = db.Kategoriler.Find(id);
            if (kategori == null)
                return HttpNotFound();

            return View(kategori);
        }

        // POST: Kategoriler/KategoriDuzenle/5
        [HttpPost]
        public ActionResult KategoriDuzenle(Kategoriler kategoriGuncel)
        {
            var kategori = db.Kategoriler.Find(kategoriGuncel.kategori_id);
            if (kategori == null)
                return HttpNotFound();

            kategori.kategori_adi = kategoriGuncel.kategori_adi;
            kategori.aciklama = kategoriGuncel.aciklama;
            kategori.aktif = kategoriGuncel.aktif;

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Kategoriler/KategoriSil/5
        public ActionResult KategoriSil(int id)
        {
            var kategori = db.Kategoriler.Find(id);
            if (kategori == null)
                return HttpNotFound();

            kategori.aktif = false;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}