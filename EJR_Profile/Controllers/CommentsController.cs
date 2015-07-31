using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EJR_Profile.Models;

namespace EJR_Profile.Controllers
{
    public class CommentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Comments
        public ActionResult Index()
        {
            var comments = db.Comments.Include(c => c.Author).Include(c => c.Post);
            return View(comments.ToList());
        }

        // GET: Comments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // GET: Comments/Create
        [Authorize]
        public ActionResult Create(int? id, string anchor, int page, int? cid = null)    // 
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (cid == null)
            {
                // Creating comment for main post
                ViewBag.id = (int)id;
                ViewBag.anchor = anchor;
                ViewBag.page = page;

                Comment comment = new Comment();
                var user = db.Users.Where(curUser => curUser.Email == User.Identity.Name).First();
                if (user != null)
                {
                    comment.DisplayName = user.DisplayName;
                    comment.AuthorId = user.Id;
                    comment.Updated = null;
                }
                comment.ParentCommentId = null;
                comment.PostId = (int)id;
                comment.Level = 0;
                return View(comment);
            }
            else
            {
                // Creating nested comment
                ViewBag.id = id;
                ViewBag.anchor = anchor;
                ViewBag.page = page;

                Comment oldComment = db.Comments.Find(cid);
                Comment newComment = new Comment();
                var user = db.Users.Where(curUser => curUser.Email == User.Identity.Name).First();
                if (user != null)
                {
                    newComment.DisplayName = user.DisplayName;
                    newComment.AuthorId = user.Id;
                }
                newComment.Updated = null;
                newComment.ParentCommentId = oldComment.Id;
                newComment.PostId = oldComment.PostId;
                newComment.Level = oldComment.Level + 1 ?? 1;
                return View(newComment);
            }
        }

        // POST: Comments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,PostId,AuthorId,ParentCommentId,Level,Body,DisplayName,UpdateReason")] Comment comment,
            int? id, int? page, string anchor)
        {
            if (ModelState.IsValid)
            {
                comment.Created = DateTime.UtcNow;
                db.Comments.Add(comment);
                db.SaveChanges();

                                //            @Html.ActionLink("Details", "Details", "Posts", null, null,
                                //anchor, new { id = ViewBag.id, page = ViewBag.page }, null)


                var route = new System.Web.Routing.RouteValueDictionary();
                route.Add("id", id);
                route.Add("page", page);
                route.Add("anchor", "#"+anchor);

                // The next line keeps failing, work on it later
                // var url = UrlHelper.GenerateUrl("", "Details", "Posts","http","",anchor,
                //  route, null,Url.RequestContext, false);
                // return new RedirectResult(url);

                return RedirectToAction("Details", "Posts", route);
            }

            return View(comment);
        }

        // GET: Comments/Edit/5
        [Authorize(Roles="Admin, Moderator")]
        public ActionResult Edit(int? id, string anchor, int page, int? cid = null)
        {
            if (id == null || cid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // Creating comment for main post
            ViewBag.id = (int)id;
            ViewBag.anchor = anchor;
            ViewBag.page = page;

            Comment comment = db.Comments.Find(cid);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin, Moderator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Body,DisplayName,UpdateReason")] Comment comment,
            int? id, string anchor, int page, int? cid = null)
        {
            if (cid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var route = new System.Web.Routing.RouteValueDictionary();
            route.Add("id", id);
            route.Add("page", page);
            route.Add("anchor", "#" + anchor);

            if (ModelState.IsValid)
            {
                Comment orig = db.Comments.Find(cid);
                orig.Body = comment.Body;
                orig.DisplayName = comment.DisplayName;
                orig.UpdateReason = comment.UpdateReason;
                orig.Updated = DateTime.UtcNow;
                db.Entry(orig).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", "Posts", route);
            }

            return RedirectToAction("Details", "Posts", route);
        }

        // GET: Comments/Delete/5
        [Authorize(Roles = "Admin, Moderator")]
        public ActionResult Delete(int? id, string anchor, int page, int? cid = null)
        {
            if (id == null || cid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.id = (int)id;
            ViewBag.anchor = anchor;
            ViewBag.page = page;
            Comment comment = db.Comments.Find(cid);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // POST: Comments/Delete/5
        //[HttpPost, ActionName("Delete")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Moderator")]
        public ActionResult DeleteConfirmed(int? id, string anchor, int page, int? cid = null)
        {
            // Want to delete the comment (cid is its id, NOT id!)
            Comment comment = db.Comments.Find(cid);
            comment.Deleted = true;
            //db.Comments.Remove(comment);
            db.SaveChanges();

            var route = new System.Web.Routing.RouteValueDictionary();
            route.Add("id", id);
            route.Add("page", page);
            route.Add("anchor", "#" + anchor);

            // The next line keeps failing, work on it later
            // var url = UrlHelper.GenerateUrl("", "Details", "Posts","http","",anchor,
            //  route, null,Url.RequestContext, false);
            // return new RedirectResult(url);

            return RedirectToAction("Details", "Posts", route);
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
