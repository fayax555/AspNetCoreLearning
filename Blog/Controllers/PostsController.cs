using Blog.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers
{
    public class PostsController : Controller
    {
        private readonly BlogDbContext _context;

        public PostsController(BlogDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("/posts/{id:int}")]
        public IActionResult Details(int id)
        
        {
            var post = _context.Posts.Include(post => post.User).SingleOrDefault(post => post.Id == id);

            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }
    }
}
