using System.Web;
using System.Web.Optimization;

namespace WebsiteGiayKingShoes
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/JS/js.js"));


            bundles.Add(new ScriptBundle("~/Admin/JS").Include(
                      "~/JS/Admin.js"));



            bundles.Add(new StyleBundle("~/Content/css").Include(

                      "~/CSS/css.css"));


            bundles.Add(new StyleBundle("~/Admin/css").Include(

                       "~/CSS/bootstrap.min.css",
                       "~/CSS/AdminCSS.css"));


        }
    }
}