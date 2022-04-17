using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TN_academic.Models;

namespace TN_academic.Controllers
{
    public class ContactsController : Controller
    {
        private TN_academic_DBEntities db = new TN_academic_DBEntities();
        public ActionResult Index()
        {
            var model = db.Contacts.FirstOrDefault();
            return View(model);
        }
    }
}