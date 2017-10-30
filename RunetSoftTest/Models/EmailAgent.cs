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
using System.Web.Configuration;

namespace RunetSoftTest.Models
{
    public static class MailAgent
    {
        public static void SendVerificationLinkEmail(string email, string activationCode, bool isPasswordRestoring = false)
        {

            //initializing smtp client and the content for the letter
            var fromEmail = new MailAddress(WebConfigurationManager.AppSettings["smtpRobotName"],
                WebConfigurationManager.AppSettings["smtpRobotDisplayingName"]);
            var fromEmailPassword = WebConfigurationManager.AppSettings["smtpRobotPass"];
            var host = WebConfigurationManager.AppSettings["smtpClientHost"];
            var port = Convert.ToInt32(WebConfigurationManager.AppSettings["smtpClientPort"]);
            var isSslEnabled = Convert.ToBoolean(WebConfigurationManager.AppSettings["isSmtpClientUsingSSL"]);
            var isCredentialsDefault = Convert.ToBoolean(WebConfigurationManager.AppSettings["isCredentialsDefaultSmtp"]);
            string subject = "";
            string body = "";
            //if it's password restoring we are sending the following subject and the body
            if (isPasswordRestoring)
            {
                subject = WebConfigurationManager.AppSettings["letterSubjectRestorePass"];
                body = WebConfigurationManager.AppSettings["letterBodyRestorePass"]; 
            }
            //the subject and the body for verifying email
            else
            {
                subject = WebConfigurationManager.AppSettings["letterSubjectVerify"];
                body = WebConfigurationManager.AppSettings["letterBodyVerify"];
            }
            //the method is using in two situations so far. The first one is when the user is verifying 
            //his account and the second one is when he/she is restoring the password ower the email
            string verifyUrl =
                (isPasswordRestoring ? "/User/OnPasswordRestoring/" : "/User/VerifyAccount/") + activationCode;
            var link = HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.PathAndQuery, verifyUrl);
            body += " <br/><br/><a href='" + link + "'>" + link + "</a> ";
            var toEmail = new MailAddress(email);
            //setting up the mail client
            var smtp = new SmtpClient
            {
                Host = host,
                Port = port,
                EnableSsl = isSslEnabled,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = isCredentialsDefault,
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