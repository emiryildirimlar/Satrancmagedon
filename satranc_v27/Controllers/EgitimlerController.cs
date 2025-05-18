using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using satranc_v27.Models.Entity; // model namespace'in

namespace satranc_v27.Controllers
{
    public class EgitimlerController : Controller
    {
        satranc_v26Entities db = new satranc_v26Entities();

        // GET: Egitimler
        public ActionResult Index()
        {
            var egitimler = db.Egitimler
                .Where(e => e.egitim_aktif == true)
                .ToList();

            return View(egitimler);
        }

        // GET: Egitimler/EgitimEkle
        [HttpGet]
        public ActionResult EgitimEkle()
        {
            return View();
        }

        // POST: Egitimler/EgitimEkle
        [HttpPost]
        public ActionResult EgitimEkle(Egitimler egitim)
        {
            egitim.egitim_aktif = true;
            egitim.egitim_tarihi = DateTime.Now;

            db.Egitimler.Add(egitim);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET: Egitimler/EgitimDuzenle/5
        [HttpGet]
        public ActionResult EgitimDuzenle(int id)
        {
            var egitim = db.Egitimler.Find(id);
            if (egitim == null)
            {
                return HttpNotFound();
            }
            return View(egitim);
        }

        // POST: Egitimler/EgitimDuzenle/5
        [HttpPost]
        public ActionResult EgitimDuzenle(Egitimler guncellenenEgitim)
        {
            var egitim = db.Egitimler.Find(guncellenenEgitim.egitim_id);
            if (egitim == null)
            {
                return HttpNotFound();
            }

            egitim.egitim_adi = guncellenenEgitim.egitim_adi;
            egitim.egitim_aciklama = guncellenenEgitim.egitim_aciklama;
            egitim.egitim_tarihi = guncellenenEgitim.egitim_tarihi;
            egitim.egitim_suresi = guncellenenEgitim.egitim_suresi;
            egitim.egitim_fiyati = guncellenenEgitim.egitim_fiyati;
            egitim.egitim_gorsel_url = guncellenenEgitim.egitim_gorsel_url;
            egitim.egitim_aktif = guncellenenEgitim.egitim_aktif;

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Egitimler/EgitimSil/5
        public ActionResult EgitimSil(int id)
        {
            var egitim = db.Egitimler.Find(id);
            if (egitim == null)
            {
                return HttpNotFound();
            }

            egitim.egitim_aktif = false;
            db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}