using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskListYangBotWeb.Models
{
    public class FavoriteEnvironment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? EnvironmentName { get; set; }
        public long PoolId { get; set; }
        public User User { get; set; }
    }
}
