namespace bwarrickBugTracker.Migrations
{
    using bwarrickBugTracker.Models;
    using bwarrickBugTracker.Models.CodeFirst;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<bwarrickBugTracker.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(bwarrickBugTracker.Models.ApplicationDbContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(context));
            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }
            if (!context.Roles.Any(r => r.Name == "ProjectManager"))
            {
                roleManager.Create(new IdentityRole { Name = "ProjectManager" });
            }
            if (!context.Roles.Any(r => r.Name == "Developer"))
            {
                roleManager.Create(new IdentityRole { Name = "Developer" });
            }
            if (!context.Roles.Any(r => r.Name == "Submitter"))
            {
                roleManager.Create(new IdentityRole { Name = "Submitter" });
            }
            
            var userManager = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(context));
            if (!context.Users.Any(u => u.Email == "blwarrick1107@gmail.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "blwarrick1107@gmail.com",
                    Email = "blwarrick1107@gmail.com",
                    FirstName = "Bri",
                    LastName = "Warrick",
                }, "briLeigh7!");
            }
            if (!context.Users.Any(u => u.Email == "rchapman@coderfoundry.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "rchapman@coderfoundry.com",
                    Email = "rchapman@coderfoundry.com",
                    FirstName = "Ryan",
                    LastName = "Chapman",
                }, "Mod123!");
            }
            if (!context.Users.Any(u => u.Email == "mjaang@coderfoundry.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "mjaang@coderfoundry.com",
                    Email = "mjaang@coderfoundry.com",
                    FirstName = "Mark",
                    LastName = "Jaang",
                }, "Teach123!");
            }
            if (!context.Users.Any(u => u.Email == "admin@demo.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "admin@demo.com",
                    Email = "admin@demo.com",
                    FirstName = "Admin",
                    LastName = "Demo",
                }, "AdminDemo123!");
            }
            if (!context.Users.Any(u => u.Email == "projectmanager@demo.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "projectmanager@demo.com",
                    Email = "projectmanager@demo.com",
                    FirstName = "ProjectManager",
                    LastName = "Demo",
                }, "ProjectManager123!");
            }
            if (!context.Users.Any(u => u.Email == "developer@demo.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "developer@demo.com",
                    Email = "developer@demo.com",
                    FirstName = "Developer",
                    LastName = "Demo",
                }, "DeveloperDemo123!");
            }
            if (!context.Users.Any(u => u.Email == "submitter@demo.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "submitter@demo.com",
                    Email = "submitter@demo.com",
                    FirstName = "Submitter",
                    LastName = "Demo",
                }, "SubmitterDemo123!");
            }
            var userId = userManager.FindByEmail("blwarrick1107@gmail.com").Id;
            userManager.AddToRole(userId, "Admin");
            var adminId = userManager.FindByEmail("rchapman@coderfoundry.com").Id;
            userManager.AddToRole(adminId, "Admin");
            var admin2Id = userManager.FindByEmail("mjaang@coderfoundry.com").Id;
            userManager.AddToRole(admin2Id, "Admin");

            //Demo Users
            var adminDemoId = userManager.FindByEmail("admin@demo.com").Id;
            userManager.AddToRole(adminDemoId, "Admin");
            var projectmanagerDemoId = userManager.FindByEmail("projectmanager@demo.com").Id;
            userManager.AddToRole(projectmanagerDemoId, "ProjectManager");
            var developerDemoId = userManager.FindByEmail("developer@demo.com").Id;
            userManager.AddToRole(developerDemoId, "Developer");
            var submitterDemoId = userManager.FindByEmail("submitter@demo.com").Id;
            userManager.AddToRole(submitterDemoId, "Submitter");

            //Priorities
            if (!context.TicketPriorities.Any(p => p.Name == "Low"))
            {
                var priority = new TicketPriority();
                priority.Name = "Low";
                context.TicketPriorities.Add(priority);
            }

            if (!context.TicketPriorities.Any(p => p.Name == "Medium"))
            {
                var priority = new TicketPriority();
                priority.Name = "Medium";
                context.TicketPriorities.Add(priority);
            }

            if (!context.TicketPriorities.Any(p => p.Name == "High"))
            {
                var priority = new TicketPriority();
                priority.Name = "High";
                context.TicketPriorities.Add(priority);
            }

            if (!context.TicketPriorities.Any(p => p.Name == "Urgent"))
            {
                var priority = new TicketPriority();
                priority.Name = "Urgent";
                context.TicketPriorities.Add(priority);
            }

            //Statuses
            if (!context.TicketStatuses.Any(p => p.Name == "Unassigned"))
            {
                var status = new TicketStatus();
                status.Name = "Unassigned";
                context.TicketStatuses.Add(status);
            }

            if (!context.TicketStatuses.Any(p => p.Name == "Assigned"))
            {
                var status = new TicketStatus();
                status.Name = "Assigned";
                context.TicketStatuses.Add(status);
            }

            if (!context.TicketStatuses.Any(p => p.Name == "In Progress"))
            {
                var status = new TicketStatus();
                status.Name = "In Progress";
                context.TicketStatuses.Add(status);
            }

            if (!context.TicketStatuses.Any(p => p.Name == "Complete"))
            {
                var status = new TicketStatus();
                status.Name = "Complete";
                context.TicketStatuses.Add(status);
            }

            //Types
            if (!context.TicketTypes.Any(p => p.Name == "Hardware"))
            {
                var type = new TicketType();
                type.Name = "Hardware";
                context.TicketTypes.Add(type);
            }

            if (!context.TicketTypes.Any(p => p.Name == "Software"))
            {
                var type = new TicketType();
                type.Name = "Software";
                context.TicketTypes.Add(type);
            }
        }
    }
   
}
