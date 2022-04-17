using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using TN_academic.ViewModels;

namespace TN_academic.Controllers
{
    public class BaseInstructorController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var session = (UserModel)Session[Common.CommonConstants.USER_LOGIN_MODEL];
            if (session == null || !session.Role.Equals("2"))
            {
                TempData.Add(Common.CommonConstants.ACCESS_DENIED, true);
                filterContext.Result = new RedirectToRouteResult(new
                    RouteValueDictionary(new { controller = "Home", action = "Index" }));
            }
            base.OnActionExecuting(filterContext);
        }
    }
}