namespace MvcStarter.Models
{
    public class Category
    {
        public int Id { get; private set; }
        public string Name { get; private set; }

        public Category(int id, string name) : this(name)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
            Id = id;
        }

        public Category(string name)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
            Name = name.Trim();
        }
    }
}
