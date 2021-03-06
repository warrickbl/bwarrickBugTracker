﻿using bwarrickBugTracker.Models;
using bwarrickBugTracker.Models.CodeFirst;
using bwarrickBugTracker.Models.Helpers;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace bwarrickBugTracker.Controllers
{
    [RequireHttps]
 
    public class HomeController : Universal
    {
        [Authorize]
        public ActionResult Index()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            List<Project> projects = new List<Project>();
            if (User.IsInRole("Admin") || User.IsInRole("ProjectManager"))
            {
                projects = db.Projects.ToList();
            }
            else
            {
                projects = user.Projects.ToList();
            }
            return View(projects);
        }

        [Authorize(Roles = ("Admin, ProjectManager"))]
        public ActionResult AllProjects(string userId)
        {
            return View(db.Projects.ToList());
        }
        [Authorize]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Landing()
        {
            return View();
        }

        public ActionResult HTMLError()
        {
            return View();
        }

        public ActionResult Error()
        {
            return View();
        }
    }
}