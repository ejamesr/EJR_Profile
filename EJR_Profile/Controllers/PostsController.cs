using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EJR_Profile.Models;
using System.IO;
using PagedList;
using PagedList.Mvc;

namespace EJR_Profile.Controllers
{
    [RequireHttps]
    public class PostsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Posts
        public ActionResult Index(int? page)
        {
            int pageSize = 3;
            int pageNumber = page ?? 1;

            // Limit the number of pages to max possible
            // Find all the entries not deleted
            var posts = db.Posts.Where(d => d.Deleted == false);
            int nEntries = posts.Count();
            ViewBag.nEntries = nEntries;            // Let view page know how many pages there are
            var maxPage = (nEntries + pageSize - 1) / pageSize;
            maxPage = maxPage < 2 ? 1 : maxPage;
            if (pageNumber > maxPage)
                pageNumber = maxPage;
            ViewBag.PageNumber = pageNumber;
            return View(posts
                .OrderByDescending(c => c.Created)
                .ToPagedList(pageNumber, pageSize));
        }

        // POST: Posts
        // Come here when user initiates search
        [HttpPost]
        public ActionResult Index(string searchStr, int? id, int? page)
        {
            if (String.IsNullOrEmpty(searchStr))
                return Redirect("Index");

            int pageSize = 3;
            int pageNumber = page ?? 1;

            // OK, search everything!
            var results = db.Posts.Where(p => p.Deleted == false &&
                                            (p.Body.Contains(searchStr)
                                            || p.MediaCaption.Contains(searchStr)
                                            || p.Title.Contains(searchStr)))
                    .Union(db.Posts.Where(p => p.Comments
                            .Any(c => c.Deleted == false &&
                                            (c.DisplayName.Contains(searchStr)
                                            || c.Body.Contains(searchStr)
                                            || c.Author.Email.Contains(searchStr)
                                            || c.UpdateReason.Contains(searchStr)))));

            int nEntries = results.Count();
            ViewBag.nEntries = nEntries;            // Let view page know how many pages there are
            var maxPage = (nEntries + pageSize - 1) / pageSize;
            maxPage = maxPage < 2 ? 1 : maxPage;
            if (pageNumber > maxPage)
                pageNumber = maxPage;
            ViewBag.PageNumber = pageNumber;


            return View(results
                .OrderByDescending(c => c.Created)
                .ToPagedList(1, 3));
        }


        // GET: Posts/Details/5
        public ActionResult Details(int? id, int? page)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            ViewBag.PageNumber = page;
            return View(post);
        }

        // GET: Posts/Create
        [Authorize(Roles="Admin")]
        public ActionResult Create(int? page)
        {
            ViewBag.PageNumber = page;
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles="Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,MediaCaption,Body,MediaURL,Published")] Post post, HttpPostedFileBase fileUpload)
        {
            // Set Created date...
            if (ModelState.IsValid)
            {
                if (post.Body.Length < 20)
                {
                    ViewBag.Message = "Invalid Body - Must be at least 20 chars in length!";
                    return View(post);
                }

                // restrict the valid file formats to images only
                if (fileUpload != null && fileUpload.ContentLength > 0)
                {
                    if (!fileUpload.ContentType.Contains("image"))
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.UnsupportedMediaType);
                    }
                    var fileName = Path.GetFileName(fileUpload.FileName);
                    var p = Path.Combine(Server.MapPath("~/img/blog/"), fileName);
                    fileUpload.SaveAs(p);
                    post.MediaURL = "~/img/blog/" + fileName;
                }
                post.Created = DateTime.UtcNow;
                db.Posts.Add(post);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(post);
        }

        // GET: Posts/Edit/5
        [Authorize(Roles="Admin")]
        public ActionResult Edit(int? id, int? page)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            ViewBag.PageNumber = page;
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Created,Title,MediaCaption,Body,MediaURL,Published")] Post post, 
            HttpPostedFileBase fileUpload, int? origPage)
        {
            // Set Updated date...
            post.Updated = DateTime.UtcNow;

            if (ModelState.IsValid)
            {
                if (post.Body.Length < 20)
                {
                    ViewBag.Message = "Invalid Body - Must be at least 20 chars in length!";
                    return View(post);
                }

                // restrict the valid file formats to images only
                if (fileUpload != null && fileUpload.ContentLength > 0)
                {
                    // We are replacing the previous file...
                    if (!fileUpload.ContentType.Contains("image"))
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.UnsupportedMediaType);
                    }
                    var fileName = Path.GetFileName(fileUpload.FileName);
                    var p = Path.Combine(Server.MapPath("~/img/blog/"), fileName);
                    fileUpload.SaveAs(p);
                    post.MediaURL = "~/img/blog/" + fileName;
                }

                db.Entry(post).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { page = origPage });
            }
            return View(post);
        }

        // GET: Posts/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id, int? page)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            ViewBag.PageNumber = page;
            return View(post);
        }

        // POST: Posts/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? id, int? origPage)
        {
            Post post = db.Posts.Find(id);
            post.Deleted = true;
            db.Entry(post).State = EntityState.Modified;
            db.SaveChanges();
            //db.Posts.Remove(post);
            //db.SaveChanges();
            return RedirectToAction("Index", new { page = origPage });
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
