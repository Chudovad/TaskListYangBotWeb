using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskListYangBotWeb.Models
{
    public class FavoriteTask
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? TaskName { get; set; }
        public long PoolId { get; set; }
        public User User { get; set; }
    }
}
