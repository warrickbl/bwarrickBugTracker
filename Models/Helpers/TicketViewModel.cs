using bwarrickBugTracker.Models.CodeFirst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace bwarrickBugTracker.Models.Helpers
{
    public class TicketViewModel
    {
        public Ticket Ticket { get; set; }
        public Project Project { get; set; }
        public SelectList Projects { get; set; } 
        public int SelectedProject { get; set; }
        public int[] TicketPriority { get; set; }
        public SelectList TicketPriorities { get; set; }
        public int[] TicketType { get; set; }
        public SelectList TicketTypes { get; set; }

    }
}