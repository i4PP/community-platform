﻿
using System.ComponentModel.DataAnnotations;
namespace BMW.ASP.Models

{
    public class LoginViewModel
    {

        [Required]
        public string? User { get; set; }

        [Required]
        public string? Password { get; set; }


    }
}
