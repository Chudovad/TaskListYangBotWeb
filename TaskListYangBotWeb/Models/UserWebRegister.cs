using System.ComponentModel.DataAnnotations;

namespace TaskListYangBotWeb.Models
{
    public class UserWebRegister
    {
        [Required]
        public string Username { get; set; }
        [Required, MinLength(6, ErrorMessage = "Please enter at least 6 characters!")]
        public string Password { get; set; }
        [Required, Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}
