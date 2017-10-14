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
using System.Web.Services.Description;
using static bwarrickBugTracker.EmailService;

namespace bwarrickBugTracker.Models.Helpers
{
    public class NotificationHelper
    {
        ApplicationDbContext db = new ApplicationDbContext();
        NotificationEmail Notification = new NotificationEmail();

        public void Notify( int ticketId, string userId, string subject, string message, bool sendEmail, bool unRead)
        {
            var notification = new NotificationEmail
            {
                Subject = subject,
                Message = message,
                IsNew = true,
                RecipientId = userId,
                Sent = DateTimeOffset.UtcNow,
                TicketId = ticketId,
                UnRead = unRead,
            };

            db.NotificationEmails.Add(notification);
            db.SaveChanges();

            if (sendEmail)
            {
                try
                {
                    var user = db.Users.Find(userId);

                    var from = "BWarrickBugTracker<BugTrackerAlert@email.com>";
                    var email = new MailMessage(from, user.Email)
                    {
                        Subject = subject,
                        Body = message,
                    };

                    var svc = new PersonalEmail();
                    svc.Send(email);
                }
                catch (Exception ex)
                {
                }
            }
        }
    }
}