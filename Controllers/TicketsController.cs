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
using static bwarrickBugTracker.Models.Helpers.NotificationHelper;
using System.Net.Mail;
using System.Configuration;
using System.Threading.Tasks;

namespace bwarrickBugTracker.Controllers
{
    [Authorize]
    public class TicketsController : Universal
    {
        //private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Tickets 
        [Authorize]
        public ActionResult Index()
        {
            ViewBag.UserTimeZone = db.Users.Find(User.Identity.GetUserId()).TimeZone;
            var user = db.Users.Find(User.Identity.GetUserId());
            var projects = db.Projects.Where(p => p.Users.Any(u => u.Id == user.Id)).ToList();
            var tickets = db.Tickets.Include(t => t.AssignToUser).Include(t => t.OwnerUser).Include(t => t.Project).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType);
            if (User.IsInRole("Admin"))
            {
                return View(tickets.ToList());
            }
            else if (User.IsInRole("ProjectManager"))
            {
                foreach (var item in projects)
                {
                    return View(item.Tickets);
                }
            }
            else if (User.IsInRole("Submitter"))
            {
                return View(tickets.Where(t => t.OwnerUserId == user.Id).ToList());
            }
            else if (User.IsInRole("Developer"))
            {
                return View(tickets.Where(t => t.AssignToUserId == user.Id).ToList());
            }
            return RedirectToAction("Index", "Home");
        }

        // GET: Tickets/Details/5

        public ActionResult Details(int? id)
        {
            Ticket ticket = db.Tickets.Find(id);
            ProjectAssignHelper helper = new ProjectAssignHelper();
            var user = db.Users.Find(User.Identity.GetUserId());
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            if (ticket == null)
            {
                return HttpNotFound();
            }
            if (User.IsInRole("Admin"))
            {
               
                return View(ticket);
            }
            else if (User.IsInRole("ProjectManager") && helper.IsUserOnProject(user.Id, ticket.ProjectId))
            {
                return View(ticket);
            }
            else if (User.IsInRole("Developer") && ticket.AssignToUserId == user.Id)
            {
                return View(ticket);
            }
            else if (User.IsInRole("Submitter") && ticket.OwnerUserId == user.Id)
            {
                return View(ticket);
            }
            
            return RedirectToAction("Index", "Home");
        }

        //public ActionResult Notifications(int? id)
        //{
        //    Ticket ticket = db.Tickets.Find(id);
        //    NotificationEmail notificationEmail = new NotificationEmail();
        //    var notifications = db.NotificationEmails.Where(n => n.TicketId == ticket.Id);

        //    foreach (var notification in notifications)
        //    {
        //        db.NotificationEmails.Remove(notification);
        //    }
        //    db.SaveChanges();
        //    return RedirectToAction("Details", new { id = ticket.Id });
        //}

        public ActionResult Notifications(int? id)
        {
            Ticket ticket = db.Tickets.Find(id);
            NotificationEmail notificationEmail = new NotificationEmail();
            var notifications = db.NotificationEmails.Where(n => n.TicketId == ticket.Id);

            foreach (var notification in notifications)
            {
                notification.UnRead = false;
            }
            db.SaveChanges();
            return RedirectToAction("Details", new { id = ticket.Id });
        }

        // GET: Tickets/Create
        [Authorize(Roles = "Submitter")]
        public ActionResult Create()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            ProjectAssignHelper helper = new ProjectAssignHelper();
            //ViewBag.AssignToUserId = new SelectList(db.Users, "Id", "FirstName");
            //ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "FirstName");
            ViewBag.ProjectId = new SelectList(helper.ListUserProjects(user.Id), "Id", "Title");
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name");
            //ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name");
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name");

            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Submitter")]
        [ValidateAntiForgeryToken]

        //,Updated
        //,AssignToUserId
        //TicketStatusId,
        public ActionResult Create([Bind(Include = "Id,Title,Description,Created,ProjectId,TicketTypeId,TicketPriorityId,OwnerUserId")] Ticket ticket)
        {


            if (ModelState.IsValid)
            {
                ticket.TicketStatusId = 1;
                ticket.OwnerUserId = User.Identity.GetUserId();
                ticket.Created = DateTimeOffset.UtcNow;
                db.Tickets.Add(ticket);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ProjectAssignHelper helper = new ProjectAssignHelper();
            //ViewBag.AssignToUserId = new SelectList(db.Users, "Id", "FirstName");
            //ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "FirstName");
            ViewBag.ProjectId = new SelectList(helper.ListUserProjects(User.Identity.GetUserId()), "Id", "Title");
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name");
            //ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name");
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name");
            return View(ticket);
        }

        // GET: Tickets/Edit
        [Authorize]
        public ActionResult Edit(int? id)
        {
            Ticket ticket = db.Tickets.Find(id);
            
            ProjectAssignHelper assignhelper = new ProjectAssignHelper();
            var user = db.Users.Find(User.Identity.GetUserId());
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            if (ticket == null)
            {
                return HttpNotFound();
            }
            UserRoleHelper helper = new UserRoleHelper();
            var developers = helper.UsersInRole("Developer");
            var devsOnTicketProj = developers.Where(d => d.Projects.Any(p => p.Id == ticket.ProjectId));
            ViewBag.AssignToUserId = new SelectList(devsOnTicketProj, "Id", "FirstName", ticket.AssignToUserId);
            ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "FirstName", ticket.OwnerUserId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Title", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);

            if (User.IsInRole("Admin"))
            {
                return View(ticket);
            }
            else if (User.IsInRole("ProjectManager") && assignhelper.IsUserOnProject(user.Id, ticket.ProjectId))
            {
                return View(ticket);
            }
            else if (User.IsInRole("Developer") && ticket.AssignToUserId == user.Id)
            {
                return View(ticket);
            }
            else if (User.IsInRole("Submitter") && ticket.OwnerUserId == user.Id)
            {
                return View(ticket);
            }
            return RedirectToAction("Index", "Home");
        }

        //// GET: Tickets/Edit- DEV & SUB
        //[Authorize(Roles ="Developer, Submitter")]
        //public ActionResult DevEdit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Ticket ticket = db.Tickets.Find(id);
        //    if (ticket == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    UserRoleHelper helper = new UserRoleHelper();
        //    var developers = helper.UsersInRole("Developer");
        //    var devsOnTicketProj = developers.Where(d => d.Projects.Any(p => p.Id == ticket.ProjectId));
        //    ViewBag.AssignToUserId = new SelectList(devsOnTicketProj, "Id", "FirstName", ticket.AssignToUserId);
        //    ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
        //    ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
        //    ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
        //    ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "FirstName", ticket.OwnerUserId);
        //    ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Title", ticket.ProjectId);           
        //    return View(ticket);
        //}


        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Description,Created,Updated,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,OwnerUserId,AssignToUserId")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.Find(User.Identity.GetUserId());
                ticket.Updated = DateTimeOffset.UtcNow;
                HistoryHelper helper = new HistoryHelper();
                NotificationHelper notificationHelper = new NotificationHelper();
                db.Entry(ticket).State = EntityState.Modified;
                TicketHistory ticketHistory = new TicketHistory();
                Ticket oldTicket = db.Tickets.AsNoTracking().First(t => t.Id == ticket.Id);
                if (oldTicket.AssignToUserId != ticket.AssignToUserId)
                {
                    helper.AssignChange(ticket, user.Id);
                }
                if (oldTicket.TicketType != ticket.TicketType)
                {
                    helper.TypeChange(ticket, user.Id);
                }
                if (oldTicket.Title != ticket.Title)
                {
                    helper.TitleChange(ticket, user.Id);
                }
                if (oldTicket.Description != ticket.Description)
                {
                    helper.DescriptionChange(ticket, user.Id);
                }
                if (oldTicket.TicketPriorityId != ticket.TicketPriorityId)
                {
                    helper.PriorityChange(ticket, user.Id);
                }
                if (oldTicket.TicketStatusId != ticket.TicketStatusId)
                {
                    helper.StatusChange(ticket, user.Id);
                }
                db.SaveChanges();

                notificationHelper.Notify(ticket.Id, ticket.AssignToUserId, "Ticket Update Alert", "An update has been made to " + ticket.Title, true, true);

                return RedirectToAction("Index");
            }
            ViewBag.AssignToUserId = new SelectList(db.Users, "Id", "FirstName", ticket.AssignToUserId);
            ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "FirstName", ticket.OwnerUserId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Title", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        //// GET: Tickets/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Ticket ticket = db.Tickets.Find(id);
        //    if (ticket == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(ticket);
        //}

        //// POST: Tickets/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Ticket ticket = db.Tickets.Find(id);
        //    db.Tickets.Remove(ticket);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        // POST: TicketComments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateComment([Bind(Include = "Id,Body,Created,TicketId,AuthorId")] TicketComment ticketComment)
        {
            if (ModelState.IsValid)
            {
                HistoryHelper helper = new HistoryHelper();      
                var user = db.Users.Find(User.Identity.GetUserId());
                ticketComment.AuthorId = user.Id;
                ticketComment.Created = DateTimeOffset.Now;
                helper.CommentAdd(ticketComment, user.Id);
                db.TicketComments.Add(ticketComment);
                db.SaveChanges();               
                return RedirectToAction("CommentNotification", "Tickets", new { id = ticketComment.TicketId });
            }
            
            ViewBag.AuthorId = new SelectList(db.Users, "Id", "FirstName", ticketComment.AuthorId);
            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title", ticketComment.TicketId);
            return View(ticketComment);
        }

        public ActionResult CommentNotification(int? id)
        {
            var ticketComment = db.TicketComments.Find(id);
            NotificationHelper notificationHelper = new NotificationHelper();
            notificationHelper.Notify(ticketComment.TicketId, ticketComment.Ticket.AssignToUser.Id, "Ticket Update Alert", "An comment has been added to " + ticketComment.Ticket.Title, true, true);
            return RedirectToAction("Details", "Tickets", new { id = ticketComment.TicketId });
        }

        // GET: Comments/Edit/5
        
        public ActionResult EditComment(int? id)
        {
            ProjectAssignHelper assignhelper = new ProjectAssignHelper();
            var user = db.Users.Find(User.Identity.GetUserId());
            TicketComment ticketComment = db.TicketComments.Find(id);
            Ticket ticket = ticketComment.Ticket;
            if ((User.IsInRole("Admin") || (ticketComment.AuthorId == user.Id ) || (User.IsInRole("ProjectManager") && assignhelper.IsUserOnProject(user.Id, ticket.ProjectId))))
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                if (ticketComment == null)
                {
                    return HttpNotFound();
                }
                return View(ticketComment);
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

        }

        // POST: Comments/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditComment([Bind(Include = "Id,AuthorId,Body,Created,Updated,TicketId")] TicketComment ticketComment)
        {
            ProjectAssignHelper assignhelper = new ProjectAssignHelper();
            Ticket ticket = ticketComment.Ticket;
            var user = db.Users.Find(User.Identity.GetUserId());
            if ((User.IsInRole("Admin") || (ticketComment.AuthorId == user.Id) || (User.IsInRole("ProjectManager") && assignhelper.IsUserOnProject(user.Id, ticket.ProjectId))))
            {
                if (ModelState.IsValid)
                {
                    db.Entry(ticketComment).State = EntityState.Modified;
                    ticketComment.Updated = DateTime.Now;
                    db.SaveChanges();
                    return RedirectToAction("Details", "Tickets", new { id = ticketComment.TicketId});
                }

                return View(ticket);
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        // POST: TicketComments/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteComment(int id)
        {
            TicketComment ticketComment = db.TicketComments.Find(id);
            NotificationHelper notificationHelper = new NotificationHelper();
            notificationHelper.Notify(ticketComment.TicketId, ticketComment.Ticket.AssignToUserId, "Ticket Comment Alert", "A comment has been deleted from " + ticketComment.Ticket.Title, true, true);
            db.TicketComments.Remove(ticketComment);
            db.SaveChanges();
            return RedirectToAction("Details", "Tickets", new { id = ticketComment.TicketId});
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateAttachment([Bind(Include = "Id,TicketId,Description")] TicketAttachment ticketAttachment, HttpPostedFileBase attachment)
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            if (attachment != null)
            {
                var ext = Path.GetExtension(attachment.FileName).ToLower();
                if (ext != ".png" && ext != ".jpg" && ext != ".jpeg" && ext != ".gif" && ext != ".bmp" && ext != ".pdf")
                    ModelState.AddModelError("image", "Invalid Format");
            }
            if (ModelState.IsValid)
            {
                if (attachment != null)
                {            
                    ticketAttachment.Created = DateTimeOffset.Now;
                    ticketAttachment.AuthorId = user.Id;
                    var filePath = "/Content/Attachments/";
                    var absPath = Server.MapPath("~" + filePath);
                    ticketAttachment.FileUrl = filePath + attachment.FileName;
                    attachment.SaveAs(Path.Combine(absPath, attachment.FileName));
                    db.TicketAttachments.Add(ticketAttachment);
                    
                }
                db.SaveChanges();
            }
           
            return RedirectToAction("Details", "Tickets", new { id = ticketAttachment.TicketId });
        }


        // POST: Attachments/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteAttachment(int id)
        {
            TicketAttachment ticketAttachment = db.TicketAttachments.Find(id);
            ProjectAssignHelper assignhelper = new ProjectAssignHelper();
            Ticket ticket = ticketAttachment.Ticket;
            var user = db.Users.Find(User.Identity.GetUserId());
            if ((User.IsInRole("Admin") || (ticketAttachment.AuthorId == user.Id) || (User.IsInRole("ProjectManager") && assignhelper.IsUserOnProject(user.Id, ticket.ProjectId))))
            {
                
                var filePath = "/Content/Attachments/";
                var absPath = Server.MapPath("~" + filePath);
                db.TicketAttachments.Remove(ticketAttachment);
                db.SaveChanges();
            }
            return RedirectToAction("Details", "Tickets", new { id = ticketAttachment.TicketId });
        }


     

    protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
