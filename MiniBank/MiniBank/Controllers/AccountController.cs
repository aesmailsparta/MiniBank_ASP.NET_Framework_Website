using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using MiniBank.Models;

namespace MiniBank.Controllers
{
    public class AccountController : Controller
    {
        [Authorize]
        // GET: Account
        public ActionResult Transfer()
        {
            bool AccountsFound = false;
            int UserID = GetUserID();
            List<Account> myAccounts = GetUserBankAccounts(UserID);

            if (myAccounts.Count > 0)
            {
                AccountsFound = true;
            }


            List<Account> payeeAccounts = GetPayeeBankAccounts(UserID);

            ViewBag.myAccounts = myAccounts;
            ViewBag.myPayees = payeeAccounts;

            ViewBag.AccountsFound = AccountsFound;
            return View();
        }

        [Authorize]
        public ActionResult MoneyTransfer()
        {
            bool AccountsFound = false;
            int UserID = GetUserID();
            List<Account> myAccounts = GetUserBankAccounts(UserID);

            if (myAccounts.Count > 0)
            {
                AccountsFound = true;
            }


            List<Account> payeeAccounts = GetPayeeBankAccounts(UserID);

            ViewBag.myAccounts = myAccounts;
            ViewBag.myPayees = payeeAccounts;

            ViewBag.AccountsFound = AccountsFound;
            return View();
        }

        //[HttpPost]
        //public ActionResult MoneyTransfer(Account FromAccount, string ToAccount, string AccountPassword, float TransferAmount, string Reference)
        //{
        //    bool Status = true;
        //    string message = "";

        //    int UserID = GetUserID();
        //    User userRecord;
        //    using (var dbc = new WebAppDataEntities())
        //    {
        //        userRecord = dbc.Users.Where(u => u.UserID == UserID).FirstOrDefault();
        //    }

        //    if (userRecord != null)
        //    {
        //        if (FromAccount.AccountNumber != ToAccount)
        //        {
        //            if (userRecord.Password == Crypto.Hash(AccountPassword))
        //            {
        //                string pattern = @"^[0-9]+(\.[0-9]{1,2})?$";
        //                if (Regex.Match(TransferAmount.ToString(), pattern).Success)
        //                {

        //                        Account ConfirmAccountTo;
        //                        Account ConfirmAccountFrom;
        //                        using (var dbc = new MiniBankEntities())
        //                        {
        //                            ConfirmAccountFrom = dbc.Accounts.Where(a => a.AccountNumber == FromAccount.AccountNumber).FirstOrDefault();
        //                            ConfirmAccountTo = dbc.Accounts.Where(a => a.AccountNumber == ToAccount).FirstOrDefault();

        //                            if (ConfirmAccountTo != null && ConfirmAccountFrom != null)
        //                            {
        //                                if ((float)ConfirmAccountFrom.Amount >= TransferAmount)
        //                                {
        //                                    Bank_Transaction transactionSend = new Bank_Transaction()
        //                                    {
        //                                        AccountID = ConfirmAccountFrom.AccountID,
        //                                        Amount = (decimal)TransferAmount,
        //                                        NewBalance = ConfirmAccountFrom.Amount - (decimal)TransferAmount,
        //                                        PreviousBalance = ConfirmAccountFrom.Amount,
        //                                        Reference = $"Transfer to: {ConfirmAccountTo.AccountNumber}",
        //                                        TransactionType = 4
        //                                    };

        //                                    Bank_Transaction transactionRecieve = new Bank_Transaction()
        //                                    {
        //                                        AccountID = ConfirmAccountTo.AccountID,
        //                                        Amount = (decimal)TransferAmount,
        //                                        NewBalance = ConfirmAccountTo.Amount + (decimal)TransferAmount,
        //                                        PreviousBalance = ConfirmAccountTo.Amount,
        //                                        Reference = $"{Reference}",
        //                                        TransactionType = 3
        //                                    };

        //                                    dbc.Bank_Transaction.Add(transactionSend);
        //                                    dbc.Bank_Transaction.Add(transactionRecieve);
        //                                    dbc.SaveChanges();

        //                                    //Transfer the money
        //                                    ConfirmAccountFrom.Amount -= (decimal)TransferAmount;
        //                                    ConfirmAccountTo.Amount += (decimal)TransferAmount;

        //                                    dbc.SaveChanges();

        //                                    Status = true;
        //                                    message = $"You successfully transfered {TransferAmount} to Account {ConfirmAccountTo.AccountNumber}";
        //                                }
        //                                else
        //                                {
        //                                    message = "Sorry, you do not have sufficient funds to carry out this transaction";
        //                                }
        //                            }
        //                            else
        //                            {
        //                                message = "Account could not be found, please try again.";
        //                            }
        //                        }
        //                }
        //                else
        //                {
        //                    message = "The amount you entered was not a valid monetary value";
        //                }
        //            }
        //            else
        //            {
        //                message = "Authorisation failed, the password you entered did not match the account";
        //            }
        //        }
        //        else
        //        {
        //            message = "Authorisation failed, you can not transfer money to the same account.";
        //        }
        //    }
        //    else
        //    {
        //        message = "Authorisation failed, your customer account was not found.";
        //    }

        //    ViewBag.Success = true;
        //    ViewBag.Message = message;
        //    ViewBag.Status = Status;
        //    return View();
        //}        

        [Authorize]
        [HttpPost]
        public ActionResult MoneyTransfer(TransactionData transaction)
        {
            bool Status = false;
            string message = "";

            User userRecord;
            int UserID = GetUserID();

            string FromAccount = transaction.FromAccountNumber;
            string ToAccount = transaction.ToAccountNumber;
            decimal TransactionValue = transaction.Amount;
            string Reference = transaction.Reference;
            string Password = transaction.Password;

            if (FromAccount != null && ToAccount != null && TransactionValue > 0 && Password != null)
            {

                using (var dbc = new MiniBankEntities())
                {
                    userRecord = dbc.Users.Where(u => u.UserID == UserID).FirstOrDefault();
                }

                if (userRecord != null)
                {
                    if (transaction.FromAccountNumber != transaction.ToAccountNumber)
                    {
                        if (userRecord.Password == Crypto.Hash(transaction.Password))
                        {
                            string pattern = @"^[0-9]+(\.[0-9]{1,2})?$";
                            if (Regex.Match(transaction.Amount.ToString(), pattern).Success && transaction.Amount > 0)
                            {

                                Account ConfirmAccountTo;
                                Account ConfirmAccountFrom;
                                using (var dbc = new MiniBankEntities())
                                {
                                    ConfirmAccountFrom = dbc.Accounts.Where(a => a.AccountNumber == transaction.FromAccountNumber.ToString()).FirstOrDefault();
                                    ConfirmAccountTo = dbc.Accounts.Where(a => a.AccountNumber == transaction.ToAccountNumber.ToString()).FirstOrDefault();

                                    if (ConfirmAccountTo != null && ConfirmAccountFrom != null)
                                    {
                                        if (ConfirmAccountFrom.Amount >= transaction.Amount)
                                        {
                                            Bank_Transaction transactionSend = new Bank_Transaction()
                                            {
                                                AccountID = ConfirmAccountFrom.AccountID,
                                                Amount = transaction.Amount,
                                                NewBalance = ConfirmAccountFrom.Amount - transaction.Amount,
                                                PreviousBalance = ConfirmAccountFrom.Amount,
                                                Reference = $"Ref Acc: {ConfirmAccountTo.AccountNumber}",
                                                TransactionType = 4,
                                                TransactionDate = DateTime.Now
                                            };

                                            Bank_Transaction transactionRecieve = new Bank_Transaction()
                                            {
                                                AccountID = ConfirmAccountTo.AccountID,
                                                Amount = transaction.Amount,
                                                NewBalance = ConfirmAccountTo.Amount + transaction.Amount,
                                                PreviousBalance = ConfirmAccountTo.Amount,
                                                Reference = $"{transaction.Reference}",
                                                TransactionType = 3,
                                                TransactionDate = DateTime.Now
                                            };

                                            dbc.Bank_Transaction.Add(transactionSend);
                                            dbc.Bank_Transaction.Add(transactionRecieve);
                                            dbc.SaveChanges();

                                            //Transfer the money
                                            ConfirmAccountFrom.Amount -= transaction.Amount;
                                            ConfirmAccountTo.Amount += transaction.Amount;

                                            dbc.SaveChanges();

                                            if (ModelState.IsValid)
                                            {
                                                ModelState.Clear();
                                            }

                                            Status = true;
                                            ViewBag.Success = true;
                                            message = $"You successfully transfered {transaction.Amount} to Account {ConfirmAccountTo.AccountNumber}";
                                        }
                                        else
                                        {
                                            message = "Sorry, you do not have sufficient funds to carry out this transaction";
                                        }
                                    }
                                    else
                                    {
                                        message = "Account could not be found, please try again.";
                                    }
                                }
                            }
                            else
                            {
                                message = "The amount you entered was not a valid monetary value";
                            }
                        }
                        else
                        {
                            message = "Authorisation failed, the password you entered did not match the account";
                        }
                    }
                    else
                    {
                        message = "Authorisation failed, you can not transfer money to the same account.";
                    }
                }
                else
                {
                    message = "Authorisation failed, your customer account was not found.";
                }
            }

            List<Account> myAccounts = GetUserBankAccounts(UserID);

            if (myAccounts.Count <= 0)
            {
                Status = false;
            }

            ViewBag.myAccounts = myAccounts;

            ViewBag.Message = message;
            ViewBag.Status = Status;
            return View();
        }

        [Authorize]
        // GET: Account
        public ActionResult OpenAccount()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult OpenAccount(User user, string AccountName)
        {
            bool Status = false;
            string message = "";
            int UserID = GetUserID();
            if (AccountName != "")
            {
                using (var dbc = new MiniBankEntities())
                {
                    var userRecord = dbc.Users.Where(u => u.UserID == UserID).FirstOrDefault();

                    if (userRecord != null)
                    {
                        if (userRecord.Password == Crypto.Hash(user.Password))
                        {
                            CreateBankAccount(UserID, AccountName);
                            Status = true;
                            message = "Congratulations your new account was successfully set up, and has been credited with £50 just for you!";
                        }
                        else
                        {
                            message = "Authorisation failed, the password you entered did not match the account";
                        }
                    }
                    else
                    {
                        message = "Authorisation failed, your customer account was not found.";
                    }
                }
            }
            else
            {
                message = "A valid account name was not supplied";
            }

            ViewBag.Message = message;
            ViewBag.Status = Status;
            return View();
        }

        [Authorize]
        public ActionResult NewPayee()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult NewPayee(NewPayee payee)
        {
            bool status = false;
            string message = "";
            Payee payeeToAdd = null;
            int UserID = GetUserID();
            if (payee.Password != null)
            {
                using (var dbc = new MiniBankEntities())
                {
                    string HashedPass = Crypto.Hash(payee.Password);
                    var verifiedUser = dbc.Users.Where(u => u.UserID == UserID && u.Password == HashedPass).FirstOrDefault();

                    if (verifiedUser == null)
                    {
                        message = "The password you entered was incorrect";
                        ViewBag.Status = status;
                        ViewBag.Message = message;
                        return View();
                    }
                }
            }
            else
            {
                message = "The password you entered was incorrect";
                ViewBag.Status = status;
                ViewBag.Message = message;
                return View();
            }

            //Validate Payee Info
            using (var dbc = new MiniBankEntities())
            {
                var PayeeAccountDetails = dbc.Accounts.Where(a => a.AccountNumber == payee.PayeeAccount).FirstOrDefault();

                if (PayeeAccountDetails != null)
                {
                    var payeeExists = dbc.Payees.Where(p => p.PayeeAccountID == PayeeAccountDetails.AccountID && p.PayeeUserID == UserID).FirstOrDefault();

                    var myAccounts = GetUserBankAccounts(UserID);

                    var IsMyAccount = myAccounts.Where(a => a.AccountID == PayeeAccountDetails.AccountID).FirstOrDefault();

                    if (IsMyAccount == null)
                    {
                        if (payeeExists == null)
                        {
                            payeeToAdd = new Payee();
                            payeeToAdd.PayeeAccountID = PayeeAccountDetails.AccountID;
                            payeeToAdd.PayeeReference = payee.PayeeReference;
                            payeeToAdd.DateCreated = DateTime.Now;
                            payeeToAdd.PayeeUserID = UserID;

                            dbc.Payees.Add(payeeToAdd);
                            dbc.SaveChanges();

                            if (ModelState.IsValid)
                            {
                                ModelState.Clear();
                            }

                            status = true;
                            message = "Payee has successfully been added to your account";
                        }
                        else
                        {
                            message = "This payee already exists";
                        }
                    }
                    else
                    {
                        message = "You cannot add one of your accounts as a payee, please head directly to <a href=\".. / Account / MoneyTransfer\">Money Transfer</a> to transfer money between your accounts";
                    }
                }
                else
                {
                    message = "It was not possible to add your payee, please check the details you entered and try again.";
                }
            }

            ViewBag.Status = status;
            ViewBag.Message = message;
            //Add Payee
            return View();
        }

        [NonAction]
        public static void CreateBankAccount(int UserID, string AccountName)
        {
            using (var dbc = new MiniBankEntities())
            {
                var newCustomer = dbc.Users.OrderByDescending(c => c.UserID).FirstOrDefault();

                if (newCustomer != null)
                {

                    var AccountToAdd = new Account
                    {
                        CustomerID = UserID,
                        AccountNumber = new Random().Next(111111, 999999).ToString(),
                        Amount = 50,
                        AccountName = AccountName
                    };

                    dbc.Accounts.Add(AccountToAdd);
                }

                dbc.SaveChanges();
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

        [NonAction]
        public List<Account> GetUserBankAccounts(int UserID)
        {
            List<Account> myAccounts = new List<Account>();
            using (var dbc = new MiniBankEntities())
            {
                myAccounts = dbc.Accounts.Where(a => a.CustomerID == UserID).ToList();
            }
            return myAccounts;
        }

        [NonAction]
        public List<Account> GetPayeeBankAccounts(int UserID)
        {
            List<Account> myPayees = new List<Account>();
            using (var dbc = new MiniBankEntities())
            {
                var accounts = dbc.Payees.Where(p => p.PayeeUserID == UserID).Join(dbc.Accounts, p => p.PayeeAccountID, a => a.AccountID, (payee, account) => new { payee, account }).ToList();

                foreach (var acc in accounts)
                {
                    var payee = new Account()
                    {
                        AccountID = acc.account.AccountID,
                        AccountName = acc.account.AccountName,
                        AccountNumber = acc.account.AccountNumber
                    };

                    myPayees.Add(payee);
                }

            }
            return myPayees;
        }

    }
}