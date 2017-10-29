using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace RunetSoftTest.Models
{
    public static class ExtentionMethodsForUser
    {
        //Verifying if a user with the same credentials already exists
        //true - email of user name already exists, false - they don't
        public static bool IsEmailOrNameExist(this User incomingUser, ref string message)
        {
            bool isExist = false;
            StringBuilder msg = new StringBuilder("Пользователь");
            using (RunetSoftDbEntities dataContext = new RunetSoftDbEntities())
            {
                var user = dataContext.tblUsers.Where(a => a.Email == incomingUser.Email).FirstOrDefault();
                //if the email already exists in the db, we are appending the following info
                if (user != null)
                {
                    msg.Append(", c почтовым адресом " + user.Email);
                    isExist = true;
                }
                user = dataContext.tblUsers.Where(a => a.UserName == incomingUser.UserName).FirstOrDefault();
                //if the userName already exists in the db, we are appending the following info
                if (user != null)
                {
                    msg.Append(", c именем " + user.UserName);
                    isExist = true;
                }
                msg.Append(" - уже зарегистрирован в системе.");
            }
            message = msg.ToString();
            return isExist;
        }
    }
}