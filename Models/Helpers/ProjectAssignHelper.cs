using bwarrickBugTracker.Models.CodeFirst;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace bwarrickBugTracker.Models.Helpers
{
    public class ProjectAssignHelper
    {
        //private UserManager<ApplicationUser> userManager =
        //   new UserManager<ApplicationUser>(new UserStore<ApplicationUser>
        //       (new ApplicationDbContext()));

        private ApplicationDbContext db = new ApplicationDbContext();

        public bool IsUserOnProject(string userId, int projectId)
        {
            var project = db.Projects.Find(projectId);
            var userBool = project.Users.Any(u => u.Id == userId);
            return userBool;
        }

        public void AddUserToProject(string userId, int projectId)
        {
            var user = db.Users.Find(userId);
            var project = db.Projects.Find(projectId);
            project.Users.Add(user);
            db.SaveChanges();
        }

        public void RemoveUserFromProject(string userId, int projectId)
        {
            var user = db.Users.Find(userId);
            var project = db.Projects.Find(projectId);
            project.Users.Remove(user);
            db.SaveChanges();
        }

        public List<Project> ListUserProjects(string userId)
        {
            var user = db.Users.Find(userId);
            return user.Projects.Where(p => p.Active == true).ToList();
        }

        public ICollection<ApplicationUser> ListUsersOnProject(int projectId)
        {
            var project = db.Projects.Find(projectId);
            return project.Users.ToList();
            //List<ApplicationUser> resultList = new List<ApplicationUser>();
            //List<ApplicationUser> List = userManager.Users.ToList();
            //foreach (var user in List)
            //{
            //    if (IsUserOnProject(user.Id, projectId))
            //        resultList.Add(user);
            //}
            //return resultList;
        }

        public ICollection<ApplicationUser> ListUsersNotOnProject(int projectId)
        {
            return db.Users.Where(u => u.Projects.All(p => p.Id != projectId)).ToList();
            //List<ApplicationUser> resultList = new List<ApplicationUser>();
            //List<ApplicationUser> List = userManager.Users.ToList();
            //foreach (var user in List)
            //{
            //    if (!IsUserOnProject(user.Id, projectId))
            //        resultList.Add(user);
            //}
            //return resultList;
        }

        //ListUserProjects- access user, bring entire list of projects
        //ListUsersOnProject- 
        //ListUsersNotOnProject
    }
}