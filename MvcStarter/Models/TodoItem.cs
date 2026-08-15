namespace MvcStarter.Models
{
    public class TodoItem
    {
        public int Id { get; private set; }
        public string Title { get; private set; }
        public bool IsCompleted { get; private set; }
        
        public TodoItem(int id, string title, bool isCompleted): this(title)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

            Id = id;
            IsCompleted = isCompleted;
        }

        public TodoItem(string title)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(title);

            Title = title.Trim();
        }

        public void MarkCompleted()
        {
            IsCompleted = true;
        }

        public void Rename(string title)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(title);
            Title = title.Trim();
        }
    }
}
