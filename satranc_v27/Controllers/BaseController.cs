using System.Linq;
using System.Web.Mvc;
using satranc_v27.Models.Entity;

namespace satranc_v27.Controllers
{
    public class BaseController : Controller
    {
        protected satranc_v26Entities db = new satranc_v26Entities();

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (System.Web.HttpContext.Current.Session["AliciId"] != null)
            {
                int aliciId = (int)System.Web.HttpContext.Current.Session["AliciId"];

                int sepetAdet = db.Sepet
                                  .Where(s => s.alici_id == aliciId)
                                  .Sum(s => (int?)s.miktar) ?? 0;

                ViewBag.sepetAdet = sepetAdet;
            }
            else
            {
                ViewBag.sepetAdet = 0;
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
