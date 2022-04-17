using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TN_academic.Models;

namespace TN_academic.Controllers
{
    public class AboutController : Controller
    {
        private TN_academic_DBEntities db = new TN_academic_DBEntities();
        // GET: About
        public ActionResult Index()
        {
            return View(db.AboutUs.ToList());
        }
    }
}