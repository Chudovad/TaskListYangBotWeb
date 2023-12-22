using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskListYangBotWeb.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public long UserId { get; set; }
        public string? UserName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime DateReg { get; set; }
        public int TypeSorting { get; set; }
        public byte[]? Token { get; set; }
        public ICollection<UserFavoriteTask>? UserFavoriteTasks { get; set; }
        public ICollection<Message>? Messages { get; set; }
    }
}
