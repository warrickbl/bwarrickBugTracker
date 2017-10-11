using bwarrickBugTracker.Models.CodeFirst;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

//namespace bwarrickBugTracker.Models.Helpers
//{
//    public class NotificationHelper
//    {
//        private ApplicationDbContext db = new ApplicationDbContext();
//        [HttpPost]
//        [AllowAnonymous]
//        [ValidateAntiForgeryToken]
//        public async Task<ActionResult> Notify(Ticket ticket, string userId)
//        {

//            var user = db.Tickets.Find(ticket.AssignToUserId).AssignToUser.Email;
//                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
//                // Send an email with this link
         
//                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id}, protocol: Request.Url.Scheme);
//                await UserManager.SendEmailAsync(user, "Ticket Assignment", "You have been assigned to a new ticket. Please visit your BugTracker at your earliest convenience." + callbackUrl + "\">here</a>");
//                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            

//            // If we got this far, something failed, redisplay form
//            return View(model);
//        }
//    }
//}