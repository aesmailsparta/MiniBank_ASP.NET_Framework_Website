using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MiniBank.Models;

namespace MiniBank.Controllers
{
    public class PayeesController : Controller
    {
        private MiniBankEntities db = new MiniBankEntities();

        [Authorize]
        // GET: Payees
        public ActionResult Index()
        {
            int UserID = GetUserID();
            List<PayeeDetails> payeeList = db.Payees
                .Join(db.Accounts, payee => payee.PayeeAccountID, account => account.AccountID, (pay, acc) => new PayeeDetails { account = acc, payee = pay }).Where(pa => pa.payee.PayeeUserID == UserID).ToList();
            return View(payeeList);
        }

        [Authorize]
        // GET: Payees/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Payee payee = db.Payees.Find(id);
            if (payee == null)
            {
                return HttpNotFound();
            }
            return View(payee);
        }

        [Authorize]
        // GET: Payees/Create
        public ActionResult Create()
        {
            return View();
        }

        [Authorize]
        // POST: Payees/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PayeeID,PayeeAccountID,PayeeUserID")] Payee payee)
        {
            if (ModelState.IsValid)
            {
                db.Payees.Add(payee);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(payee);
        }


        [Authorize]
        // GET: Payees/Delete/5
        public ActionResult Delete(int? id)
        {
            int UserID = GetUserID();
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            Payee payee = db.Payees.Where(p => p.PayeeID == id && p.PayeeUserID == UserID).FirstOrDefault();
            if (payee == null)
            {
                return RedirectToAction("Index");
            }
            return View(payee);
        }

        [Authorize]
        // POST: Payees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Payee payee = db.Payees.Find(id);
            db.Payees.Remove(payee);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [NonAction]
        public int GetUserID()
        {
            int id = 0;
            string email = User.Identity.Name;
            if (User.Identity.IsAuthenticated)
            {
                using (var dbc = new MiniBankEntities())
                {
                    id = dbc.Users.Where(u => u.Email == email).Select(u => u.UserID).FirstOrDefault();
                }
            }
            return id;
        }
    }
}