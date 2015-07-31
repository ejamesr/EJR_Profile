using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EJR_Profile.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {
        public ActionResult AboutMe()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Blog()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult ComingSoon()
        {
            return View();
        }

        public ActionResult ContactMe()
        {
            ViewBag.Message = "My contact page.";

            return View();
        }

        public ActionResult Gazelle()
        {
            return View();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IP()
        {
            return View();
        }

        public ActionResult JS_Exercises()
        {
            return View();
        }

        public ActionResult NumberGun()
        {
            return View();
        }

        public ActionResult Other()
        {
            return View();
        }

        public ActionResult Performance()
        {
            return View();
        }

        public ActionResult Portfolio()
        {
            return View();
        }

        public ActionResult PowerQuest()
        {
            return View();
        }

        public ActionResult Resume()
        {
            return View();
        }

        public ActionResult TestPerformance()
        {
            return View();
        }

    }
}