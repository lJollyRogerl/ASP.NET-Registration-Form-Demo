using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RunetSoftTest.Models;
using System.Net.Mail;
using System.Net;
using System.Data.Entity.Validation;
using System.Data.Entity.Migrations;
using System.Web.Security;
using System.Text;

namespace RunetSoftTest.Controllers
{
    public class UserController : Controller
    {
        [HttpGet]
        public ActionResult Register()
        {
            if (Request.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register([Bind(Exclude = "IsEmailVerified,ActivationCode")] User registatingUser)
        {
            bool status = false;
            string message = "";
            if (ModelState.IsValid)
            {
                //register the user if the name or the email doesn't exist
                if (!registatingUser.IsEmailOrNameExist(ref message))
                {
                    #region Generate Activation Code 
                    registatingUser.ActivationCode = Guid.NewGuid();
                    #endregion

                    #region  Password Hashing 
                    registatingUser.Password = Crypto.GetHash(registatingUser.Password);
                    registatingUser.ConfirmPassword = registatingUser.Password;
                    #endregion
                    registatingUser.IsEmailVerified = false;

                    #region Save to the Database

                    using (RunetSoftDbEntities dbContext = new RunetSoftDbEntities())
                    {
                        try
                        {
                            dbContext.tblUsers.Add(registatingUser);
                            dbContext.SaveChanges();
                        }
                        catch (DbEntityValidationException ex)
                        {
                            status = true;
                            ViewBag.Message = ex.Message;
                            return View(registatingUser);
                        }


                        //send email to the user
                        MailAgent.SendVerificationLinkEmail(registatingUser.Email, registatingUser.ActivationCode.ToString());
                        message = "Регистрация прошла успешно. Для дальнейшего использования аккаунта необходимо воспользоваться ссылкой, " +
                            " отправленной на ваш e-mail:" + registatingUser.Email;
                        status = true;
                    }
                    #endregion
                }
            }
            else
            {
                message = "Неверный запрос";
            }

            ViewBag.Message = message;
            ViewBag.Status = status;
            return View(registatingUser);
        }
        [HttpGet]
        public ActionResult VerifyAccount(string id)
        {
            //verifying the accound after the registration over the user's e-mail
            //status will be checked within the form. 
            //If it's false - the user'll see the according message
            bool status = false;
            string message = "";
            using (RunetSoftDbEntities dbContext = new RunetSoftDbEntities())
            {
                dbContext.Configuration.ValidateOnSaveEnabled = false;
                var user = dbContext.tblUsers.Where(u => u.ActivationCode == new Guid(id)).FirstOrDefault();
                if (user != null)
                {
                    user.IsEmailVerified = true;
                    status = true;
                    dbContext.SaveChanges();
                    message = "Ваш аккаунт успешно активирован! Спасибо за то что вы с нами!";
                }
                else
                {
                    status = false;
                    message = "Ваша ссылка некорректна. Активация не успешна.";
                }
                ViewBag.Message = message;
                ViewBag.Status = status;
                return View();
            }
        }
        [HttpGet]
        public ActionResult Login()
        {
            if (Request.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            return View();
        }
        [HttpPost]
        public ActionResult Login(UserLogin userToLogin, string ReturnUrl = "")
        {
            //status will be checked within the form. 
            //If it's false - the user'll see the according message
            string message = null;
            bool status = false;
            if (ModelState.IsValid)
            {
                using (RunetSoftDbEntities dataContext = new RunetSoftDbEntities())
                {
                    //verify if the user exists in the database
                    var usr = dataContext.tblUsers.Where(u => u.UserName == userToLogin.UserName).FirstOrDefault();
                    if (usr != null)
                    {
                        //verify if the password matches
                        if (Crypto.Verify(usr.Password, userToLogin.Password))
                        {
                            //verify if the user account has been activated
                            if ((usr.IsEmailVerified != null) && (Convert.ToBoolean(usr.IsEmailVerified)))
                            {
                                #region adding cookies to user's browser
                                int timeout = userToLogin.IsRememberMeRequested ? 525600 : 20; // 525600 min = 1 year
                                var ticket = new FormsAuthenticationTicket(userToLogin.UserName, userToLogin.IsRememberMeRequested, timeout);
                                string encrypted = FormsAuthentication.Encrypt(ticket);
                                var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
                                cookie.Expires = DateTime.Now.AddMinutes(timeout);
                                cookie.HttpOnly = true;
                                Response.Cookies.Add(cookie);
                                #endregion
                                #region redirection after the user has logged in
                                if (Url.IsLocalUrl(ReturnUrl))
                                {
                                    return Redirect(ReturnUrl);
                                }
                                else
                                {
                                    return RedirectToAction("Index", "Home");
                                }
                                #endregion
                            }
                            else
                            {
                                message = "Сначала необходимо активировать аккаунт, перейдя по ссылке, отправленной вам на e-mail";
                            }
                        }
                        else
                        {
                            message = "Логин или пароль неверный!";
                        }
                    }
                    else
                    {
                        message = "Пользователя с таким именем не существует!";
                    }
                }
            }
            
            ViewBag.Message = message;
            ViewBag.Status = status;
            return View(userToLogin);
        }

        [HttpGet]
        public ActionResult RestorePassword()
        {
            //if the user is already authenticated - redirect to the home page
            if (Request.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        public ActionResult RestorePassword(string email)
        {
            //if the user is already authenticated - redirect to the home page
            if (Request.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            string message = null;
            bool status = false;
            //checking if the user has specified the email
            if (!string.IsNullOrEmpty(email))
            {
                using (RunetSoftDbEntities dataContext = new RunetSoftDbEntities())
                {
                    var user = dataContext.tblUsers.Where(usr => usr.Email == email).FirstOrDefault();
                    //checking if the user exists in the database
                    if (user != null)
                    {
                        //sending the link to user's email which will redirect to the OnPasswordRestoring view 
                        //and the user could change the pass there. If the Guid mathes of course
                        user.ActivationCode = Guid.NewGuid();
                        dataContext.Configuration.ValidateOnSaveEnabled = false;
                        dataContext.SaveChanges();
                        MailAgent.SendVerificationLinkEmail(user.Email, user.ActivationCode.ToString(), true);
                        message = "На вашу почту отправлена ссылка для восстановления пароля.";
                        status = true;
                    }
                    else
                        message = "Пользователь с таким почтовым адресом не зарегистрирован.";
                }
            }
            else
                message = "Укажите почтовый адрес, для восстановления пароля.";

            ViewBag.Status = status;
            ViewBag.Message = message;
            return View();
        }

        [HttpGet]
        public ActionResult OnPasswordRestoring(string id)
        {
            //status will be checked within the form. 
            //if it'll be false - the user can't change the pass
            //if id is wrong - the status will be false 
            bool status = false;
            bool hidePasswordDiv = true;
            string message = "";
            
            using (RunetSoftDbEntities dbContext = new RunetSoftDbEntities())
            {
                dbContext.Configuration.ValidateOnSaveEnabled = false;
                var user = dbContext.tblUsers.Where(u => u.ActivationCode == new Guid(id)).FirstOrDefault();
                if (user != null)
                {
                    status = true;
                    hidePasswordDiv = false;
                    message = "Введите новый пароль в поля ниже. И на этот раз - запомните его.";
                }
                else
                {
                    hidePasswordDiv = true;
                    message = "Ваша ссылка некорректна. Восстановление пароля не возможно.";
                }
                ViewBag.Status = status;
                ViewBag.Message = message;
                ViewBag.HidePasswordDiv = hidePasswordDiv;
                return View();
            }
        }

        [HttpPost]
        public ActionResult OnPasswordRestoring(UserPasswordRestore restore)
        {
            //status will be checked within the form. 
            //If it's false - the user'll see the according message and the new pass will be declined
            bool status = false;
            string message = null;
            if (ModelState.IsValid)
            {
                using (RunetSoftDbEntities dbContext = new RunetSoftDbEntities())
                {
                    dbContext.Configuration.ValidateOnSaveEnabled = false;
                    var user = dbContext.tblUsers.Where(u => u.ActivationCode == new Guid(restore.ID)).FirstOrDefault();
                    if (user != null)
                    {
                        //encrypting and saving the new pass here
                        user.Password = Crypto.GetHash(restore.Password);
                        dbContext.SaveChanges();
                        status = true;
                        message = "Пароль был успешно изменен.";
                    }
                    else
                    {
                        message = "Пароль не был изменен.";
                    }
                }
            }
            else
                message = "Пароль не был изменен.";
            ViewBag.Message = message;
            ViewBag.Status = status;
            return View(restore);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "User");
        }

        //provides the form when the user can change some his/her information 
        [Authorize]
        [HttpGet]
        public ActionResult EditAccount()
        {
            using (RunetSoftDbEntities dataContext = new RunetSoftDbEntities())
            {
                var userToEdit = dataContext.tblUsers.Where(u => u.UserName == HttpContext.User.Identity.Name).FirstOrDefault();
                userToEdit.ConfirmPassword = userToEdit.Password;
                return View(userToEdit);
            }
        }

        //submittion of user's information.
        [Authorize]
        [HttpPost]
        public ActionResult EditAccount([Bind(Exclude = "IsEmailVerified")] User userToEdit)
        {
            //status will be checked within the form. 
            //If it's false - the user'll see the according message
            bool status = false;
            String message = null;
            userToEdit.IsEmailVerified = true;
            userToEdit.Password = Crypto.GetHash(userToEdit.Password);
            userToEdit.ConfirmPassword = userToEdit.Password;
            if (ModelState.IsValid)
            {
                using (RunetSoftDbEntities dataContext = new RunetSoftDbEntities())
                {
                    //since the user can't modify his/her user name and email we will not check
                    //if those exist within the db  
                    dataContext.tblUsers.Attach(userToEdit);
                    dataContext.Entry(userToEdit).State = System.Data.Entity.EntityState.Modified;
                    dataContext.SaveChanges();
                    status = true;
                    message = "Ваши учетные данные были успешно изменены";
                }
            }
            else
                message = "Ваши учетные данные не были изменены";
            ViewBag.Status = status;
            ViewBag.Message = message;
            return View(userToEdit);
        }

        //A function which provides a connection between JQuery action in Scripts.js and the database
        //it gives an information if the user name or the email already exists in the db
        public JsonResult CheckUsernameAvailability(string userName = "", string email = "")
        {
            using (RunetSoftDbEntities dataContext = new RunetSoftDbEntities())
            {
                var user = dataContext.tblUsers
                    .Where(usr => (usr.UserName == userName) || (usr.Email == email))
                    .FirstOrDefault();
                if (user != null)
                {
                    return Json(1, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
        }
    }

}