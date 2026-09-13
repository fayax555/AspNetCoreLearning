namespace Blog.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int PostId { get; set; }
        public int UserId { get; set; }
        public Post? Post { get; set; }
        public User? User { get; set; }
    }
}
