using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RunetSoftTest.Models
{
    public class UserLogin
    {
        //model for the login form
        [Required(ErrorMessage = "Введите пожалуйста логин")]
        [Display(Name = "Логин пользователя")]
        [RegularExpression(@"[a-zA-ZА-ЯЁа-яё0-9_]{0,150}", ErrorMessage = "Введите логин корректно")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Введите пожалуйста пароль")]
        [Display(Name = "Пароль")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Минимальное кол-во символов для пароля - 6")]
        public string Password { get; set; }

        [Display(Name = "Запомнить меня")]
        public bool IsRememberMeRequested { get; set; } = false;
    }
}