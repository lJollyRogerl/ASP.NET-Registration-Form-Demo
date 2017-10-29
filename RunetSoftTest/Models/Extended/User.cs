using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RunetSoftTest.Models
{
    [MetadataType(typeof(UserMetadata))]
    public partial class User
    {
        //this prop is using only to verify passwords in the register form and in the user edditing one
        //it verifies if in two text boxes, passwords are the same
        [NotMapped]
        public string ConfirmPassword { get; set; }

        //List of countries goes from web.config, under the CountriesSection. 
        //A user can choose from those in the register form
        [NotMapped]
        public static List<SelectListItem> Countries
        {
            get
            {
                NameValueCollection section =
                (NameValueCollection)ConfigurationManager.GetSection("CountriesSection");
                List<SelectListItem> countries = new List<SelectListItem>();
                for (int i = 0; i < section.Count; i++)
                {
                    countries.Add(new SelectListItem() { Text = section.GetKey(i), Value = section[i] });
                }
                return countries;
            }
            
        }
    }
    //mapping attributes to the entity data model
    public class UserMetadata
    {
        [ScaffoldColumn(false)]
        public int UserID { get; set; }

        [Required(ErrorMessage = "Введите пожалуйста логин")]
        [Display(Name = "Логин пользователя")]
        [RegularExpression(@"[a-zA-ZА-ЯЁа-яё0-9_]{0,150}", ErrorMessage = "Введите логин корректно")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Введите пожалуйста e-mail")]
        [Display(Name = "E-mail")]
        [RegularExpression(@"^[\w-]+(\.[\w-]+)*@([a-z0-9-]+(\.[a-z0-9-]+)*?\.[a-z]{2,6}|(\d{1,3}\.){3}\d{1,3})(:\d{4})?$", ErrorMessage = "Введите корректный e-mail")]
        public string Email { get; set; }

        [Display(Name = "День рождения")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "0:dd.MM.yyyy")]
        public Nullable<System.DateTime> DateOfBirth { get; set; }

        [Required(ErrorMessage = "Введите пожалуйста пароль")]
        [Display(Name = "Пароль")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Минимальное кол-во символов для пароля - 6")]
        public string Password { get; set; }

        [Display(Name = "Подтвердите пароль")]
        [DataType(DataType.Password)]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }


        [Display(Name = "Страна")]
        public string Country { get; set; }
    }
}