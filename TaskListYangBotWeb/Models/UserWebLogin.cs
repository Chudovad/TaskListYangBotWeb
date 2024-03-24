using System.ComponentModel.DataAnnotations;

namespace TaskListYangBotWeb.Models
{
    public class UserWebLogin
    {
        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }
        [Required, MinLength(6, ErrorMessage = "Please enter at least 6 characters!")]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }
}
