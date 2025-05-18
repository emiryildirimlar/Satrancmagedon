using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web.Mvc;
using satranc_v27.Models.Entity;

namespace satranc_v27.Controllers
{
    public class SepetController : BaseController
    {
        // Sepet ürünü ekleme
        [HttpPost]
        [Authorize]
        public ActionResult Ekle(int urunId, int miktar)
        {
            if (Session["AliciId"] == null)
            {
                return RedirectToAction("Login", "Alicilar");
            }

            int aliciId = (int)Session["AliciId"];

            Sepet yeniSepet = new Sepet
            {
                alici_id = aliciId,
                urun_id = urunId,
                miktar = miktar,
                ekleme_tarihi = DateTime.Now
            };

            db.Sepet.Add(yeniSepet);
            db.SaveChanges();

            return RedirectToAction("Listele", "Sepet");
        }
        [ChildActionOnly]
        public ContentResult SepetAdet()
        {
            int sepetAdet = 0;

            if (Session["AliciId"] != null)
            {
                int aliciId = (int)Session["AliciId"];
                sepetAdet = db.Sepet
                              .Where(s => s.alici_id == aliciId)
                              .Sum(s => (int?)s.miktar) ?? 0;
            }

            string badgeHtml = $"<span class=\"position-absolute top-0 start-100 translate-middle badge rounded-pill bg-danger\">{sepetAdet}</span>";

            return Content(badgeHtml);
        }
        // Sepeti listele
        public ActionResult Listele()
        {
            if (Session["AliciId"] == null)
            {
                return RedirectToAction("Login", "Alicilar");
            }

            int aliciId = Convert.ToInt32(Session["AliciId"]);

            var sepetUrunleri = db.Sepet
                                  .Include(s => s.Urunler)
                                  .Where(s => s.alici_id == aliciId)
                                  .ToList();

            // Toplam tutarı ViewBag'e ata
            ViewBag.ToplamTutar = sepetUrunleri.Sum(s => s.miktar * s.Urunler.fiyat);

            return View(sepetUrunleri);
        }

        [HttpPost]
        public ActionResult Sil(int sepetId)
        {
            var sepet = db.Sepet.Find(sepetId);
            if (sepet != null)
            {
                db.Sepet.Remove(sepet);
                db.SaveChanges();
            }
            return RedirectToAction("Listele");
        }

        [HttpPost]
        public ActionResult MiktarDegistir(int sepetId, int artis)
        {
            var sepet = db.Sepet.Find(sepetId);
            if (sepet != null)
            {
                sepet.miktar += artis;
                if (sepet.miktar <= 0)
                {
                    db.Sepet.Remove(sepet);
                }
                db.SaveChanges();
            }
            return RedirectToAction("Listele");
        }

        [HttpPost]
        public ActionResult Tamamla()
        {
            int? aliciId = Convert.ToInt32(Session["AliciId"]);
            if (aliciId == null) return RedirectToAction("Giris", "Login");

            var sepetUrunler = db.Sepet
                                 .Include(s => s.Urunler)
                                 .Where(x => x.alici_id == aliciId)
                                 .ToList();

            if (!sepetUrunler.Any())
                return RedirectToAction("Listele");

            decimal toplamTutar = sepetUrunler.Sum(x => x.miktar * x.Urunler.fiyat);

            var alici = db.Alicilar.FirstOrDefault(x => x.alici_id == aliciId);
            string adres = alici?.adres ?? "Adres bulunamadı";

            var yeniSiparis = new Siparisler
            {
                alici_id = aliciId.Value,
                siparis_tarihi = DateTime.Now,
                toplam_tutar = toplamTutar,
                odeme_durumu = "Kapıda Ödeme",
                kargo_durumu = "Hazırlanıyor",
                teslimat_adresi = adres
            };

            db.Siparisler.Add(yeniSiparis);
            db.SaveChanges(); // siparis_id oluşması için önce kaydet

            // 🆕 Sipariş Detaylarını ekle
            foreach (var item in sepetUrunler)
            {
                if (item.Urunler.stok_miktari >= item.miktar)
                {
                    db.SiparisDetaylari.Add(new SiparisDetaylari
                    {
                        siparis_id = yeniSiparis.siparis_id,
                        urun_id = item.urun_id,
                        miktar = item.miktar,
                        birim_fiyat = item.Urunler.fiyat
                    });

                    item.Urunler.stok_miktari -= item.miktar;
                }
                else
                {
                    TempData["mesaj"] = $"{item.Urunler.urun_adi} ürünü için yeterli stok yok.";
                    return RedirectToAction("Listele");
                }
            }

            db.SaveChanges(); // hem detaylar hem stok için

            db.Sepet.RemoveRange(sepetUrunler);
            db.SaveChanges();

            TempData["mesaj"] = "Siparişiniz başarıyla oluşturuldu.";
            return RedirectToAction("Index", "Alicilar");
        }




    }
}
