using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using EJR_Profile.Models;

namespace EJR_Profile.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<EJR_Profile.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "EJR_Profile.Models.ApplicationDbContext";
        }
        //private ApplicationDbContext context = new ApplicationDbContext();

        protected override void Seed(ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            var store = new UserStore<ApplicationUser>(context);
            UserManager<ApplicationUser> manager = new UserManager<ApplicationUser>(store);

            // Make sure the desired roles exist...
            var roleManager = new Microsoft.AspNet.Identity
                .RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }
            if (!context.Roles.Any(r => r.Name == "Moderator"))
            {
                roleManager.Create(new IdentityRole { Name = "Moderator" });
            }

            //
            // Comment out these two lines to stop debugger
            //
            //if (System.Diagnostics.Debugger.IsAttached == false)
            //    System.Diagnostics.Debugger.Launch();

            // Now make sure key people exist...

            // Add myself as a user and admin
            NewUserAndRole(context, manager,
                "Eric", "Ruff", "Eric", "ejames.ruff@gmail.com", "Admin", "Eric7777!");

            // Add others as a users and moderators
            NewUserAndRole(context, manager,
                "Andrew", "Jensen", "AJ", "ajensen@coderfoundry.com", "Moderator", "aBC&123");

            NewUserAndRole(context, manager,
                "Antonio", "Raynor", "Antonio", "araynor@coderfoundry.com", "Moderator", "aBC&123");

            NewUserAndRole(context, manager,
                "Bobby", "Davis", "Bobby", "bdavis@coderfoundry.com", "Moderator", "aBC&123");

            NewUserAndRole(context, manager,
                "T. J.", "Jones", "T. J.", "tjones@coderfoundry.com", "Moderator", "aBC&123");

            NewUserAndRole(context, manager,
                "Thomas", "Parish", "Thomas", "tparrish@coderfoundry.com", "Moderator", "aBC&123");
        }

        private void NewUserAndRole(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            string fName, string lName, string dName, string email, string role, string pWord)
        {
            if (!context.Users.Any(u => u.Email == email))
            {
                ApplicationUser au = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FirstName = fName,
                    LastName = lName,
                    DisplayName = dName
                };
                userManager.Create(au, pWord);
                userManager.AddToRole(au.Id, role);
            }
        }
    }
}
