using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using satranc_v27.Models.Entity;

namespace satranc_v27.Controllers
{
    public class SiparislerController : Controller
    {
        satranc_v26Entities db = new satranc_v26Entities();

        // GET: Siparisler
        public ActionResult Index()
        {
            var siparisler = db.Siparisler
                .Include(x => x.Alicilar) // alici_id kullanıcı tablosuna bağlıysa
                .Where(x => x.odeme_durumu != "İptal") // örnek filtre, isteğe göre değiştir
                .ToList();

            return View(siparisler);
        }

        // GET: Siparisler/SiparisEkle
        [HttpGet]
        public ActionResult SiparisEkle()
        {
            ViewBag.Alicilar = new SelectList(db.Alicilar.ToList(), "alici_id", "ad");
            return View();
        }

        // POST: Siparisler/SiparisEkle
        [HttpPost]
        public ActionResult SiparisEkle(Siparisler siparis)
        {
            siparis.siparis_tarihi = DateTime.Now;
            db.Siparisler.Add(siparis);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Siparisler/SiparisDuzenle/5
        [HttpGet]
        public ActionResult SiparisDuzenle(int id)
        {
            var siparis = db.Siparisler.Find(id);
            if (siparis == null)
            {
                return HttpNotFound();
            }

            ViewBag.Alicilar = new SelectList(db.Alicilar.ToList(), "alici_id", "ad", siparis.alici_id);
            return View(siparis);
        }

        // POST: Siparisler/SiparisDuzenle/5
        [HttpPost]
        public ActionResult SiparisDuzenle(Siparisler guncellenen)
        {
            var siparis = db.Siparisler.Find(guncellenen.siparis_id);
            if (siparis == null)
            {
                return HttpNotFound();
            }

            siparis.alici_id = guncellenen.alici_id;
            siparis.siparis_tarihi = guncellenen.siparis_tarihi;
            siparis.toplam_tutar = guncellenen.toplam_tutar;
            siparis.odeme_durumu = guncellenen.odeme_durumu;
            siparis.kargo_durumu = guncellenen.kargo_durumu;
            siparis.teslimat_adresi = guncellenen.teslimat_adresi;

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Siparisler/SiparisSil/5
        public ActionResult SiparisSil(int id)
        {
            var siparis = db.Siparisler.Find(id);
            if (siparis == null)
            {
                return HttpNotFound();
            }

            
            siparis.odeme_durumu = "İptal";

            

            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}