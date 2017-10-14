using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace bwarrickBugTracker.Models.CodeFirst
{
    public class Universal : Controller
    {
        public ApplicationDbContext db = new ApplicationDbContext();

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = db.Users.Find(User.Identity.GetUserId());
                var myNotifications = db.NotificationEmails.AsNoTracking().Where(n => n.RecipientId == user.Id).ToList();

                ViewBag.Notifications = myNotifications.Where(n => n.UnRead).Count();
                ViewBag.Message = myNotifications.Where(n => n.UnRead);
            }
            base.OnActionExecuting(filterContext);
        }

    }
}