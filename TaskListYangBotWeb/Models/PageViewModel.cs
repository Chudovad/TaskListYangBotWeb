namespace TaskListYangBotWeb.Models
{
    public class PageViewModel
    {
        public IEnumerable<Message> Messages { get; set; }
        public PageInfoModel PageInfoModel { get; set; }
        public Message Message { get; set; }
        public User User { get; set; }
    }
}
