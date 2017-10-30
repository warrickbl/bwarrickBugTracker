using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using bwarrickBugTracker.Models;
using bwarrickBugTracker.Models.CodeFirst;
using Microsoft.AspNet.Identity;
using bwarrickBugTracker.Models.Helpers;
using System.IO;
using System.Data.Entity.Infrastructure;

namespace bwarrickBugTracker.Models.Helpers
{
    public class HistoryHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public void AssignChange(Ticket ticket, string userId)
        {
            TicketHistory ticketHistory = new TicketHistory();
            Ticket oldTicket = db.Tickets.AsNoTracking().First(t => t.Id == ticket.Id);
            if(oldTicket.AssignToUserId != null)
            { 
                ticketHistory.OldValue = oldTicket.AssignToUser.FullName;
                ticketHistory.NewValue = db.Users.Find(ticket.AssignToUserId).FullName;
            }
            else
            {
                ticketHistory.OldValue = null;
                ticketHistory.NewValue = db.Users.Find(ticket.AssignToUserId).FullName;
            }
            ticketHistory.TicketId = ticket.Id;
            ticketHistory.Property = "Assign To UserId";
            ticketHistory.Created = DateTimeOffset.UtcNow;
            ticketHistory.AuthorId = userId;
            db.TicketHistories.Add(ticketHistory);
            db.SaveChanges();
        }

        public void TypeChange(Ticket ticket, string userId)
        {
            TicketHistory ticketHistory = new TicketHistory();
            Ticket oldTicket = db.Tickets.AsNoTracking().First(t => t.Id == ticket.Id);
            ticketHistory.OldValue = oldTicket.TicketType.Name;
            ticketHistory.NewValue = db.TicketTypes.Find(ticket.TicketTypeId).Name;
            ticketHistory.TicketId = ticket.Id;
            ticketHistory.Property = "Ticket Type";
            ticketHistory.Created = DateTimeOffset.UtcNow;
            ticketHistory.AuthorId = userId;
            db.TicketHistories.Add(ticketHistory);
            db.SaveChanges();
        }

        public void TitleChange(Ticket ticket, string userId)
        {
            TicketHistory ticketHistory = new TicketHistory();
            Ticket oldTicket = db.Tickets.AsNoTracking().First(t => t.Id == ticket.Id);
            ticketHistory.OldValue = oldTicket.Title;
            ticketHistory.NewValue = ticket.Title;
            ticketHistory.TicketId = ticket.Id;
            ticketHistory.Property = "Title";
            ticketHistory.Created = DateTimeOffset.UtcNow;
            ticketHistory.AuthorId = userId;
            db.TicketHistories.Add(ticketHistory);
            db.SaveChanges();
        }

        public void DescriptionChange(Ticket ticket, string userId)
        {
            TicketHistory ticketHistory = new TicketHistory();
            Ticket oldTicket = db.Tickets.AsNoTracking().First(t => t.Id == ticket.Id);
            ticketHistory.OldValue = oldTicket.Description;
            ticketHistory.NewValue = ticket.Description;
            ticketHistory.TicketId = ticket.Id;
            ticketHistory.Property = "Description";
            ticketHistory.Created = DateTimeOffset.UtcNow;
            ticketHistory.AuthorId = userId;
            db.TicketHistories.Add(ticketHistory);
            db.SaveChanges();
        }

        public void PriorityChange(Ticket ticket, string userId)
        {
            TicketHistory ticketHistory = new TicketHistory();
            Ticket oldTicket = db.Tickets.AsNoTracking().First(t => t.Id == ticket.Id);
            ticketHistory.OldValue = oldTicket.TicketPriority.Name;
            ticketHistory.NewValue = db.TicketPriorities.Find(ticket.TicketPriorityId).Name;
            ticketHistory.TicketId = ticket.Id;
            ticketHistory.Property = "Ticket Priority";
            ticketHistory.Created = DateTimeOffset.UtcNow;
            ticketHistory.AuthorId = userId;
            db.TicketHistories.Add(ticketHistory);
            db.SaveChanges();
        }

        public void StatusChange(Ticket ticket, string userId)
        {
            TicketHistory ticketHistory = new TicketHistory();
            Ticket oldTicket = db.Tickets.AsNoTracking().First(t => t.Id == ticket.Id);
            ticketHistory.OldValue = oldTicket.TicketStatus.Name;
            ticketHistory.NewValue = db.TicketStatuses.Find(ticket.TicketStatusId).Name;
            ticketHistory.TicketId = ticket.Id;
            ticketHistory.Property = "Ticket Status";
            ticketHistory.Created = DateTimeOffset.UtcNow;
            ticketHistory.AuthorId = userId;
            db.TicketHistories.Add(ticketHistory);
            db.SaveChanges();
        }

        public void CommentAdd(TicketComment ticketComment, string userId)
        {
            TicketHistory ticketHistory = new TicketHistory();        
            ticketHistory.OldValue = "No Comment";
            ticketHistory.NewValue = ticketComment.Id.ToString();
            ticketHistory.TicketId = ticketComment.TicketId;
            ticketHistory.Property = "Comment added";
            ticketHistory.Created = DateTimeOffset.UtcNow;
            ticketHistory.AuthorId = userId;
            db.TicketHistories.Add(ticketHistory);
            db.SaveChanges();
        }

        public void AttachmentAdd(TicketAttachment ticketAttachment, string userId)
        {
            TicketHistory ticketHistory = new TicketHistory();
            ticketHistory.OldValue = "No Attachment";
            ticketHistory.NewValue = ticketAttachment.Id.ToString();
            ticketHistory.TicketId = ticketAttachment.TicketId;
            ticketHistory.Property = "Attachment added";
            ticketHistory.Created = DateTimeOffset.UtcNow;
            ticketHistory.AuthorId = userId;
            db.TicketHistories.Add(ticketHistory);
            db.SaveChanges();
        }
    }
}