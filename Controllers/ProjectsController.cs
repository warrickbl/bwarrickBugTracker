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
using bwarrickBugTracker.Models.Helpers;
using Microsoft.AspNet.Identity;

namespace bwarrickBugTracker.Controllers
{
    [Authorize]
    public class ProjectsController : Universal
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Projects
        [Authorize]
        public ActionResult Index(string userId)
        {
            ViewBag.UserTimeZone = db.Users.Find(User.Identity.GetUserId()).TimeZone;
            var user = db.Users.Find(User.Identity.GetUserId());
            ProjectAssignHelper helper = new ProjectAssignHelper();
            helper.ListUserProjects(user.Id);
            return View(helper.ListUserProjects(user.Id));
        }

        [Authorize(Roles = ("Admin, ProjectManager"))]
        public ActionResult AllProjects(string userId)
        {
            return View(db.Projects.ToList());
        }

        // GET: Projects/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            var user = db.Users.Find(User.Identity.GetUserId());
            ProjectAssignHelper helper = new ProjectAssignHelper();
            if (helper.IsUserOnProject(user.Id, project.Id) == true || User.IsInRole("Admin") || User.IsInRole("ProjectManager"))
            {
                return View(project);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        // GET: Projects/Create
        [Authorize(Roles = "Admin, ProjectManager")]
        public ActionResult Create()
        {

            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin, ProjectManager")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Created,Updated,Title,Description,AuthorId")] Project project)
        {
            if (ModelState.IsValid)
            {

                var user = db.Users.Find(User.Identity.GetUserId());
                project.AuthorId = user.FullName;
                project.Created = DateTimeOffset.UtcNow;
                db.Projects.Add(project);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(project);
        }

        // GET: Projects/Edit/5
        [Authorize(Roles = "Admin, ProjectManager")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin, ProjectManager")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Created,Updated,Title,Description,AuthorId")] Project project)
        {
            if (ModelState.IsValid)
            {
                project.Updated = DateTimeOffset.UtcNow;
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(project);
        }

        // GET: Projects/Delete/5
        [Authorize(Roles = "Admin, ProjectManager")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Delete/5
        [Authorize(Roles = "Admin, ProjectManager")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Project project = db.Projects.Find(id);
            db.Projects.Remove(project);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //GET EDITPROJECTUSERS
        [Authorize(Roles = "Admin, ProjectManager")]
        public ActionResult EditProjectUsers(int id)
        {
            var project = db.Projects.Find(id);
            ProjectUserViewModels projectuserVM = new ProjectUserViewModels();
            projectuserVM.Project = project;
            projectuserVM.SelectedUsers = project.Users.Select(u => u.Id).ToArray();
            projectuserVM.Users = new MultiSelectList(db.Users.ToList(), "Id", "FirstName", projectuserVM.SelectedUsers);
            return View(projectuserVM);
        }

        //POST EDITPROJECTUSERS
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, ProjectManager")]
        public ActionResult EditProjectUsers(ProjectUserViewModels model)
        {
            var project = db.Projects.Find(model.Project.Id);
            ProjectAssignHelper helper = new ProjectAssignHelper();

            foreach (var userId in db.Users.Select(r => r.Id).ToList())
            {
                helper.RemoveUserFromProject(userId, project.Id);
            }

            foreach (var userId in model.SelectedUsers)
            {
                helper.AddUserToProject(userId, project.Id);
            }
            return RedirectToAction("AllProjects");
        }


        //GET: EditProjectUsers(My version)
        //[Authorize(Roles ="Admin, ProjectManager")]
        //public ActionResult EditProjectUsers(int? projectId)
        //{

        //    var project = db.Projects.Find(projectId);
        //    var helper = new ProjectAssignHelper();
        //    ProjectUserViewModels model = new ProjectUserViewModels
        //    {
        //        Project = project,
        //        SelectedUsers = helper.ListUsersOnProject(projectId.Value).Select(p => p.Id).ToArray()
        //    };
        //    model.Users = new MultiSelectList(db.Users, "Id", "FullName", model.SelectedUsers);

        //    return View(model);      
        //}


        //POST: EditProjectUsers- My Version
        //[HttpPost]
        //[Authorize(Roles = "Admin, ProjectManager")]
        //public ActionResult EditProjectUsers(ProjectUserViewModels model, string userId)
        //{
        //    var project = db.Projects.Find(model.Project.Id);   
        //    ProjectAssignHelper helper = new ProjectAssignHelper();

        //    foreach (var user in model.SelectedUsers.ToList())
        //    {

        //        helper.RemoveUserFromProject(userId, project.Id);
        //    }
        //    if (model.SelectedUsers != null)
        //    {
        //        foreach (var user in model.SelectedUsers)
        //        {
        //            helper.AddUserToProject(userId, project.Id);
        //        }
        //        return RedirectToAction("Index");
        //    }
        //    return RedirectToAction("Index");
        //}


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
