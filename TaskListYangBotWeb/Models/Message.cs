using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskListYangBotWeb.Models
{
    public class Message
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? TextMessage { get; set; }
        public DateTime DateTime { get; set; }
        public User? User { get; set; }
    }
}
