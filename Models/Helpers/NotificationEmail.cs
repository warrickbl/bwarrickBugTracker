using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace bwarrickBugTracker.Models.Helpers
{
    public class NotificationEmail
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public bool IsNew { get; set; }
        public DateTimeOffset Sent { get; set; }
        public string RecipientId { get; set; }
        public bool UnRead { get; set; }

        public virtual ApplicationUser Recipient { get; set; }
    }
}