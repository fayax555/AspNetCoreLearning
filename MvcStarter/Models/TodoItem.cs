namespace MvcStarter.Models
{
    public class TodoItem
    {
        public int Id { get; private set; }
        public string Title { get; private set; }
        public bool IsCompleted { get; private set; }
        public TodoPriority Priority { get; private set; }

        public DateOnly? DueDate { get; private set; }

        public int? CategoryId { get; private set; }
        public Category? Category { get; private set; }

        
        public TodoItem(int id, string title, TodoPriority priority, bool isCompleted): this(title, priority)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

            Id = id;
            IsCompleted = isCompleted;
        }

        public TodoItem(string title, TodoPriority priority)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(title);

            if (!Enum.IsDefined(priority))
            {
                throw new ArgumentOutOfRangeException(nameof(priority));
            }

            Title = title.Trim();
            Priority = priority;
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

        public void ChangePriority(TodoPriority priority)
        {
            if (!Enum.IsDefined(priority))
            {
                throw new ArgumentOutOfRangeException(nameof(priority));
            }

            Priority = priority;
        }

        public void ChangeCategory(int? categoryId)
        {
            if (categoryId.HasValue)
            {
                ArgumentOutOfRangeException.ThrowIfNegativeOrZero(
                    categoryId.Value,
                    nameof(categoryId)
                );
            }

            CategoryId = categoryId;
        }

        public void ChangeDueDate(DateOnly? dueDate)
        {
            DueDate = dueDate;
        }
    }
}
