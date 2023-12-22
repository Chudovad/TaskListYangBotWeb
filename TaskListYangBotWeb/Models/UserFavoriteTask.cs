namespace TaskListYangBotWeb.Models
{
    public class UserFavoriteTask
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public int FavoriteTaskId { get; set; }
        public FavoriteTask FavoriteTask { get; set; }
    }
}
