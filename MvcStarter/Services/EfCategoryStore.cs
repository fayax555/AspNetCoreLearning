using Microsoft.EntityFrameworkCore;
using MvcStarter.Data;
using MvcStarter.Models;

namespace MvcStarter.Services
{
    public class EfCategoryStore
    {
        private readonly TodoDbContext _context;

        public EfCategoryStore(TodoDbContext context)
        {
            _context = context;
        }

        public IReadOnlyList<Category> GetCategories()
        {
            return _context.Categories.AsNoTracking().OrderBy(category => category.Name).ToList();
        }

        public bool CategoryExists(int id)
        {
            return _context.Categories.Any(category => category.Id == id);
        }

        public bool CategoryNameExists(string name, int? excludedCategoryId = null)
        {
            var query = _context.Categories.AsQueryable();

            if (excludedCategoryId != null)
            {
                query = query
                .Where(category => category.Id != excludedCategoryId);
            }

            return query
                .Any(category => category.Name.ToUpper() == name.Trim()
                .ToUpper());
        }

        public Category AddCategory(string name)
        {
            var category = new Category(name);
            _context.Categories.Add(category);
            _context.SaveChanges();
            return category;
        }

        public Category? GetCategoryById(int id)
        {
            return _context.Categories.AsNoTracking().SingleOrDefault(category => category.Id == id);
        }

        public bool TryRenameCategory(int id, string name)
        {
            var category = _context.Categories.SingleOrDefault(category => category.Id == id);

            if (category == null)
            {
                return false;
            }

            category.Rename(name);
            _context.SaveChanges();
            return true;
        }

        public bool TryDeleteCategory(int id)
        {
            var category = _context.Categories.SingleOrDefault(category => category.Id == id);

            if (category == null)
            {
                return false;
            }

            _context.Categories.Remove(category);
            _context.SaveChanges();

            return true;
        }
    }
}
