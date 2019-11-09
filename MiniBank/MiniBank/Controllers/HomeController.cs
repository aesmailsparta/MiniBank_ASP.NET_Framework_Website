using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MiniBank.Controllers
{
    public class HomeController : Controller
    {
        public static List<Account> accounts = new List<Account>();
        [Authorize]
        // GET: Home
        public ActionResult Index()
        {
            List<(string AccountNumber, string AccountName, decimal? Amount, decimal? MonthOutgoing, decimal? MonthIncoming, decimal? MonthNet)> AccountsOverview = new List<(string AccountNumber, string AccountName, decimal? Amount, decimal? MonthOutgoing, decimal? MonthIncoming, decimal? MonthNet)>();
            int UserID = GetUserID();
            using (var dbc = new MiniBankEntities())
            {
                string Name = dbc.Users.Where(u => u.UserID == UserID).Select(u => u.FirstName).FirstOrDefault();
                ViewBag.UserName = Name;
            }
            using (var dbc = new MiniBankEntities())
            {

                accounts = dbc.Accounts.Where(a => a.CustomerID == UserID).ToList();
                ViewBag.OpenAccounts = accounts.Count();
                var date = DateTime.Now;
                var startDate = new DateTime(date.Year, date.Month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);

                foreach (var item in accounts)
                {
                    var monthsOutgoing = dbc.Bank_Transaction
                        .Join(dbc.TransactionTypes, bt => bt.TransactionType, t => t.TypeID, (bt, t) => new { bt, t })
                        .Where(btt => btt.bt.AccountID == item.AccountID && btt.t.IsDebit == true && (btt.bt.TransactionDate >= startDate && btt.bt.TransactionDate <= endDate))
                        .Sum(btt => btt.bt.Amount);

                    var monthsIncoming = dbc.Bank_Transaction
                        .Join(dbc.TransactionTypes, bt => bt.TransactionType, t => t.TypeID, (bt, t) => new { bt, t })
                        .Where(btt => btt.bt.AccountID == item.AccountID && btt.t.IsDebit == false && (btt.bt.TransactionDate >= startDate && btt.bt.TransactionDate <= endDate))
                        .Sum(btt => btt.bt.Amount);

                    monthsOutgoing = monthsOutgoing == null ? 0 : monthsOutgoing;
                    monthsIncoming = monthsIncoming == null ? 0 : monthsIncoming;
                    var monthNet = monthsIncoming - monthsOutgoing;

                    var accountInfo = (
                        item.AccountNumber,
                        item.AccountName,
                        item.Amount,
                        monthsOutgoing,
                        monthsIncoming,
                        monthNet == null ? 0 : monthNet
                    );

                    AccountsOverview.Add(accountInfo);
                }

                //Pay someone

                //Set up a direct debit
            }
            ViewBag.AccountsOverview = AccountsOverview;
            //ViewBag.pass = Crypto.Hash("password1234");
            return View();
        }

        [Authorize]
        public ActionResult Dashboard()
        {
            int UserID = GetUserID();

            using (var dbc = new MiniBankEntities())
            {
                accounts = dbc.Accounts.Where(a => a.CustomerID == UserID).ToList();

                ViewBag.MyAccounts = accounts;

                var transactions = (from t in dbc.Bank_Transaction
                                    join a in dbc.Accounts
                                    on t.AccountID equals a.AccountID
                                    join tt in dbc.TransactionTypes
                                    on t.TransactionType equals tt.TypeID
                                    where a.CustomerID == UserID
                                    select new { t.TransactionID, t.TransactionDate, t.Reference, t.Amount, t.NewBalance, t.PreviousBalance, a.AccountNumber, tt.TypeName, tt.IsDebit }).OrderByDescending(t => t.TransactionID).ToList();

                var ParsedTransactionsList = new List<(int TranscationID, DateTime? TransactionDate, string Reference, decimal? Amount, decimal? NewBalance, decimal? PreviousBalance, string AccountNumber, string TypeName, bool IsDebit)>();

                foreach (var t in transactions)
                {
                    var transaction =
                    (
                        t.TransactionID,
                        t.TransactionDate,
                        t.Reference,
                        t.Amount,
                        t.NewBalance,
                        t.PreviousBalance,
                        t.AccountNumber,
                        t.TypeName,
                        t.IsDebit
                    );

                    ParsedTransactionsList.Add(transaction);

                }

                ViewBag.MyTransactions = ParsedTransactionsList;
            }

            return View();
        }

        [Authorize]
        public ActionResult Withdraw()
        {

            return View();
        }

        [Authorize]
        public ActionResult Transfer()
        {
            return View();
        }

        [Authorize]
        public ActionResult Settings()
        {
            return View();
        }

        [Authorize]
        public ActionResult Account(string id)
        {
            if (id != null)
            {
                Account account = null;
                int UserID = GetUserID();
                using (var dbc = new MiniBankEntities())
                {
                    account = dbc.Accounts.Where(a => a.AccountNumber == id && a.CustomerID == UserID).FirstOrDefault();

                    if (account != null)
                    {
                        var startDate = DateTime.Now.AddMonths(-3);
                        startDate = new DateTime(startDate.Year, startDate.Month, 1);

                        var transactions = (from t in dbc.Bank_Transaction
                                            join tt in dbc.TransactionTypes
                                            on t.TransactionType equals tt.TypeID
                                            where t.AccountID == account.AccountID && t.TransactionDate >= startDate
                                            select new { t.TransactionID, t.TransactionDate, t.Reference, t.Amount, t.NewBalance, t.PreviousBalance, tt.TypeName, tt.IsDebit }).OrderByDescending(t => t.TransactionID).ToList();

                        var ParsedTransactionsList = new List<(int TranscationID, DateTime? TransactionDate, string Reference, decimal? Amount, decimal? NewBalance, decimal? PreviousBalance, string TypeName, bool IsDebit)>();

                        foreach (var t in transactions)
                        {
                            var transaction =
                            (
                                t.TransactionID,
                                t.TransactionDate,
                                t.Reference,
                                t.Amount,
                                t.NewBalance,
                                t.PreviousBalance,
                                t.TypeName,
                                t.IsDebit
                            );

                            ParsedTransactionsList.Add(transaction);

                        }

                        ViewBag.MyTransactions = ParsedTransactionsList;

                        return View(account);
                    }
                    else
                    {
                        return RedirectToAction("Dashboard", "Home");
                    }
                }
            }
            else
            {
                return RedirectToAction("Dashboard", "Home");
            }
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