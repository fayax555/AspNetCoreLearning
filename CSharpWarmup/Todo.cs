namespace CSharpWarmup
{
    internal class Todo
    {
        public string Title { get; }
        public TodoPriority Priority { get; }
        public bool IsCompleted { get; private set; }

        public Todo(string title, TodoPriority priority)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException("Title must not be empty", nameof(title));
            }

            if (!Enum.IsDefined(priority))
            {
                throw new ArgumentOutOfRangeException(nameof(priority));
            }

            Title = title;
            Priority = priority;
        }

        public void MarkCompleted()
        {
            IsCompleted = true;
        }
    }
}
