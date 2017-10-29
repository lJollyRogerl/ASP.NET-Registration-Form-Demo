using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RunetSoftTest.Models
{
    public class UserPasswordRestore
    {
        //model for the OnPasswordRestoring form
        [Required(ErrorMessage = "Введите пожалуйста пароль")]
        [Display(Name = "Пароль")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Минимальное кол-во символов для пароля - 6")]
        public string Password { get; set; }

        [Display(Name = "Подтвердите пароль")]
        [DataType(DataType.Password)]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }
        [UIHint("HiddenInput")]
        public string ID { get; set; }
    }
}