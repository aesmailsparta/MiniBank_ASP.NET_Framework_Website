using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using MiniBank.Models;

namespace MiniBank.Controllers
{
    public class UserController : Controller
    {

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Registration()
        {
            if (RedirectLoggedIn()) { return RedirectToAction("Index", "Home"); }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registration([Bind(Exclude = "IsVerified, ActivationCode")] User user)
        {

            bool Status = false;
            string message = "";

            if (ModelState.IsValid)
            {
                #region Check If Email Exists
                var EmailExists = DoesEmailExist(user.Email);
                if (EmailExists)
                {
                    ModelState.Remove("Email");
                    TempData["EmailExists"] = "This email address has already been registered";
                    ModelState.AddModelError("Email", "This email address has already been registered");

                    return View();
                }
                #endregion

                #region Generate Activation Code
                user.IsVerified = false;
                user.ActivationCode = Guid.NewGuid();
                #endregion

                #region Password Hashing
                user.Password = Crypto.Hash(user.Password);
                user.ConfirmPassword = Crypto.Hash(user.ConfirmPassword);
                #endregion

                #region Save to Database - Send Verification Email
                using (var dbc = new MiniBankEntities())
                {
                    //user.UserID = null;

                    //Commit user to database
                    dbc.Users.Add(user);
                    dbc.SaveChanges();

                    SendVerificationLinkEmail(user.Email, user.ActivationCode.ToString());

                    //Set success message for web output
                    message = $"You have successfully registered to MiniBank!" +
                        $" Welcome {user.FirstName}. An account verfication email" +
                        $" has been sent to : {user.Email}";
                    Status = true;
                }

                var AccName = $"{user.FirstName.Substring(0, 1)}{user.LastName}";

                AccountController.CreateBankAccount(user.UserID, AccName);
                #endregion
            }
            else
            {
                //Model error handling
                message = "Invalid Request";
            }

            ViewBag.Message = message;
            ViewBag.Status = Status;
            return View(user);
        }

        [NonAction]
        public bool DoesEmailExist(string email)
        {
            using (var dbc = new MiniBankEntities())
            {
                var ExistingEmail = dbc.Users.Where(u => u.Email == email).FirstOrDefault();
                return ExistingEmail != null;
            }
        }

        [NonAction]
        public void SendVerificationLinkEmail(string email, string activationCode, bool resend = false)
        {
            #region Build verification link
            var verifyURL = "/User/VerifyAccount/" + activationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyURL);
            #endregion

            //Set as Env Variable
            #region SMTP Credentials
            var fromEmail = new MailAddress("sprt.711@gmail.com", "MiniBank");
            var fromEmailPassword = "password5!";
            #endregion

            #region Recipient Details
            var toEmail = new MailAddress(email);
            #endregion

            #region Set up email details
            string subject;
            string body;
            if (resend)
            {
                subject = "MiniBank Email Varification.";

                body = "<br/><br/>We woul like to verify your email address for security purposes." +
                    " Please click on the below link to verify your email address." +
                    " <br/><br/><a href='" + link + "'>" + link + "</a> ";

            }
            else
            {
                subject = "Welcome to MiniBank, Email Varification.";

                body = "<br/><br/>We are excited to tell you that your MiniBank account was" +
                    " successfully created. Please click on the below link to verify your email address and get started." +
                    " <br/><br/><a href='" + link + "'>" + link + "</a> ";
            }
            #endregion

            #region Set up SMTP Client
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,//TLS PORT
                           // Port = 465,//SSL PORT
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };
            #endregion

            #region Send email
            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
            {
                smtp.Send(message);
            }
            #endregion
        }

        [HttpGet]
        public ActionResult VerifyAccount(string id)
        {
            bool Status = false;
            if (id != null)
            {
                using (var dbc = new MiniBankEntities())
                {
                    dbc.Configuration.ValidateOnSaveEnabled = false;

                    //Find user with the matching activation code
                    var userToValidate = dbc.Users.Where(u => u.ActivationCode == new Guid(id)).FirstOrDefault();

                    //if a user is found verify their account otherwise send error message to the user as invalid activation attempt
                    if (userToValidate != null)
                    {
                        userToValidate.IsVerified = true;
                        dbc.SaveChanges();
                        Status = true;
                    }
                    else
                    {
                        ViewBag.message = "Verification failed, the activation key does not exist";
                    }
                }
            }
            else
            {
                //If user somehow gets to this page directly, redirect them to the home page
                ViewBag.message = "Invalid Request";
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Status = Status;
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            if (RedirectLoggedIn()) { return RedirectToAction("Index", "Home"); }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UserLogin login, string ReturnURL = "")
        {
            string message = "";

            using (var dbc = new MiniBankEntities())
            {
                var ExisitingUser = dbc.Users.Where(u => u.Email == login.Email).FirstOrDefault();
                if (ExisitingUser != null)
                {
                    if (string.Compare(ExisitingUser.Password, Crypto.Hash(login.Password)) == 0)
                    {
                        if (ExisitingUser.IsVerified == true)
                        {
                            int timeout = login.RememberMe ? 525600 : 30;
                            var ticket = new FormsAuthenticationTicket(login.Email, login.RememberMe, timeout);
                            string encrypted = FormsAuthentication.Encrypt(ticket);
                            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
                            cookie.Expires = DateTime.Now.AddMinutes(timeout);
                            cookie.HttpOnly = true;
                            Response.Cookies.Add(cookie);


                            if (Url.IsLocalUrl(ReturnURL))
                            {
                                return Redirect(ReturnURL);
                            }
                            else
                            {
                                return RedirectToAction("Index", "Home");
                            }
                        }
                        else
                        {
                            ViewBag.Warning = true;
                            message = "This email address has not yet been verified, please check your inbox and junk folder.";
                        }
                    }
                    else
                    {
                        message = "Invalid password and/or email provided";
                    }
                }
                else
                {
                    message = "Invalid password and/or email provided";
                }
            }


            ViewBag.Message = message;
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult Logout()
        {
            //Logout user from the web application and redirect to homepage
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "User");
        }

        //public ActionResult ForgotPassword()
        //{
        //    ViewBag.VerifyCode = false;
        //    return View();
        //}

        [HttpPost]
        public ActionResult ForgotPassword(AccountHelper helper)
        {
            bool status = false;
            string message = "";
            User ExistingUser = null;
            //verify email exists
            using (var dbc = new MiniBankEntities())
            {
                ExistingUser = dbc.Users.Where(u => u.Email == helper.Email).FirstOrDefault();
            }

            if (ExistingUser != null)
            {
                //create new quick code
                var quickCode = new Random().Next(11111111, 99999999);

                var Token = Crypto.URLSafeHash(quickCode.ToString());

                //create reset token in database
                using (var dbc = new MiniBankEntities())
                {
                    var newToken = new UserToken()
                    {
                        Token = Token,
                        DateRequested = DateTime.Now,
                        UserID = ExistingUser.UserID,
                        Used = false
                    };

                    dbc.UserTokens.Add(newToken);
                    dbc.SaveChanges();
                }

                //send email quick code email
                SendQuickCodeEmail(ExistingUser.Email, quickCode.ToString());

                return RedirectToAction("ForgotPassword", "User", new { id = Token });

            }
            else
            {
                message = "We could not find this email address, please check your information and try again.";
            }

            ViewBag.Status = status;
            ViewBag.Message = message;
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPasswordReset(AccountHelper helper, string ValidateToken)
        {
            //Verify entered token
            string message = "";
            bool status = false;
            var userToken = Crypto.URLSafeHash(helper.QuickCode);
            if (userToken == ValidateToken)
            {
                if (helper.Password == helper.ConfirmPassword)
                {
                    int UserToUpdate;
                    using (var dbc = new MiniBankEntities())
                    {
                        UserToUpdate = (int)dbc.UserTokens.Where(ut => ut.Token == userToken).Select(ut => ut.UserID).FirstOrDefault();
                    }

                    if (UserToUpdate > 0)
                    {
                        using (var dbc = new MiniBankEntities())
                        {
                            var user = dbc.Users.Find(UserToUpdate);

                            if (user != null)
                            {
                                var newPassword = Crypto.Hash(helper.Password);

                                user.Password = newPassword;
                                user.ConfirmPassword = newPassword;

                                dbc.SaveChanges();
                                status = true;
                                message = "Your password has been sucessfully updated.";
                            }
                            else
                            {
                                message = "Password update failed, please try again";
                            }
                        }
                    }
                    else
                    {
                        message = "Invalid Token";
                    }
                }
                else
                {
                    message = "Passwords did not match";
                }
                //redirect to change password page
            }
            else
            {
                message = "Invalid verification code";
            }

            if (status)
            {
                using (var dbc = new MiniBankEntities())
                {
                    var tokentodiscard = dbc.UserTokens.Where(ut => ut.Token == userToken && ut.Used == false).FirstOrDefault();
                    if (tokentodiscard != null)
                    {
                        tokentodiscard.Used = true;
                        dbc.SaveChanges();
                    }
                }
            }

            ViewBag.ChangePassword = true;
            ViewBag.Token = ValidateToken;
            ViewBag.Status = status;
            ViewBag.Message = message;
            return View("ForgotPassword");
        }

        [HttpGet]
        public ActionResult ForgotPassword(string id)
        {
            bool changePassword = false;
            if (id != null)
            {
                bool status = false;
                string message = "";

                //verify email exists
                using (var dbc = new MiniBankEntities())
                {
                    //Validate token
                    var tokenRecord = dbc.UserTokens.Where(ut => ut.Token == id).FirstOrDefault();
                    if (tokenRecord != null)
                    {
                        TimeSpan TimeSinceRequest = DateTime.Now.Subtract((DateTime)tokenRecord.DateRequested);
                        if (TimeSinceRequest.TotalMinutes < 30)
                        {
                            ViewBag.Token = tokenRecord.Token;
                            //Show form for validation code
                            changePassword = true;
                            ViewBag.ChangePassword = changePassword;
                            ViewBag.Status = status;
                            ViewBag.Message = message;
                            return View();
                        }
                        else
                        {
                            return RedirectToAction("ForgotPassword", "User");
                        }
                    }
                    else
                    {
                        return RedirectToAction("ForgotPassword", "User");
                    }
                }
            }
            return View();
        }

        public ActionResult ResendActivation()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ResendActivation(AccountHelper helper)
        {
            bool status = false;
            string message = "";
            //verify email exists
            using (var dbc = new MiniBankEntities())
            {
                var EmailExists = dbc.Users.Where(u => u.Email == helper.Email).FirstOrDefault();

                if (EmailExists != null)
                {
                    //validate email has not already been activated
                    if (!(bool)EmailExists.IsVerified)
                    {
                        //resend activation email
                        SendVerificationLinkEmail(EmailExists.Email, EmailExists.ActivationCode.ToString(), true);
                        status = true;
                        message = $"Your activation email has been sent to {helper.Email}. Please remember to check your" +
                            " junk mail folder, as it may be recieved there.";
                    }
                    else
                    {
                        message = "This email address has already been verified, you can <a href=\"/User/Login\" >login here</a>";
                    }

                }
                else
                {
                    message = "We could not find this email address, please check your information and try again.";
                }
            }

            ViewBag.Status = status;
            ViewBag.Message = message;
            return View();
        }

        public bool RedirectLoggedIn()
        {
            //Check if a user is logged in
            if (User.Identity.IsAuthenticated)
            {
                return true;
            }

            return false;
        }

        [NonAction]
        public void SendQuickCodeEmail(string email, string quickCode)
        {
            //Set as Env Variable
            #region SMTP Credentials
            var fromEmail = new MailAddress("sprt.711@gmail.com", "MiniBank");
            var fromEmailPassword = "password5!";
            #endregion

            #region Recipient Details
            var toEmail = new MailAddress(email);
            #endregion

            #region Set up email details
            string subject;
            string body;

            subject = "Reset password verification";

            body = "<br/><br/>A password reset has been requested for your MiniBank account" +
                " Please use the 8 digit verification code below on the prompted page to create a new password" +
                $" <br/><br/><div style=\"width:100%;\" ><h3>Verification Code:</h3><h1>{quickCode}</h1></div> ";
            #endregion

            #region Set up SMTP Client
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,//TLS PORT
                           // Port = 465,//SSL PORT
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };
            #endregion

            #region Send email
            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
            {
                smtp.Send(message);
            }
            #endregion
        }

    }
}