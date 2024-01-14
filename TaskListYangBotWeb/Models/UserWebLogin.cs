using System.ComponentModel.DataAnnotations;

namespace TaskListYangBotWeb.Models
{
    public class UserWebLogin
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
