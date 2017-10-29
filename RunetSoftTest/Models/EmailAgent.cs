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

namespace RunetSoftTest.Models
{
    public static class MailAgent
    {
        public static void SendVerificationLinkEmail(string email, string activationCode, bool isPasswordRestoring = false)
        {
            //the method is using in two situations so far. The first one is when the user is verifying 
            //his account and the second one is when he/she is restoring the password ower the email
            string verifyUrl =
                (isPasswordRestoring ? "/User/OnPasswordRestoring/" : "/User/VerifyAccount/") + activationCode;
            var link = HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.PathAndQuery, verifyUrl);
            var fromEmail = new MailAddress(System.Web.Configuration.WebConfigurationManager.AppSettings["smtpRobotName"],
                "Test for RunetSoft");
            var toEmail = new MailAddress(email);
            var fromEmailPassword = System.Web.Configuration.WebConfigurationManager.AppSettings["smtpRobotPass"];
            string subject = "";
            string body = "";
            //if it's password restoring we are sending the following subject and the body
            if (isPasswordRestoring)
            {
                subject = "Восстановление пароля.";

                body = "<br/><br/>Вы умудрились забыть ваш пароль. Ай-ай-ай. Ничего страшного, мы о вас позаботились." +
                    " Пожалуйста, перейдите по ссылке ниже, для того что бы перейти на форму восстановления пароля" +
                    " <br/><br/><a href='" + link + "'>" + link + "</a> ";
            }
            //the subject and the body for verifying email
            else
            {
                subject = "Ваш аккаунт успешно создан!";

                body = "<br/><br/>Мы спешим вам сообщить, что ваш аккаунт был успешно создан." +
                    " Пожалуйста, перейдите по ссылке, для того что бы подтвердить ваш аккаунт" +
                    " <br/><br/><a href='" + link + "'>" + link + "</a> ";
            }
            //setting up the mail client
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                //credentials are stored within the web.config
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };
            //sending the letter
            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);
        }

    }
}