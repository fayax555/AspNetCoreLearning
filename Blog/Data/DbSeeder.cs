using Blog.Models;

namespace Blog.Data
{
    public static class DbSeeder
    {
        public static void Seed(BlogDbContext db)
        {
            if (db.Users.Any())
            {
                return;
            }

            var alice = new User
            {
                Name = "Alice",
                Email = "alice@example.com"
            };

            var bob = new User
            {
                Name = "Bob",
                Email = "bob@example.com"
            };

            db.Users.AddRange(alice, bob);
            db.SaveChanges();

            var post1 = new Post
            {
                Title = "My First Post",
                Content =
                    "This is my first blog post. I created this blog to share what I learn " +
                    "while working on different programming projects. I hope to use it to " +
                    "document useful ideas, challenges I run into, and solutions that might " +
                    "help other developers as well.",
                UserId = alice.Id
            };

            var post2 = new Post
            {
                Title = "Learning EF Core",
                Content =
                    "Entity Framework Core makes working with databases in .NET much easier " +
                    "by allowing developers to use C# classes and LINQ instead of writing SQL " +
                    "for every operation. I have been learning how relationships, migrations, " +
                    "constraints, and database seeding work together in a typical web application.",
                UserId = bob.Id
            };

            db.Posts.AddRange(post1, post2);
            db.SaveChanges();

            var comments = new[]
            {
                new Comment
                {
                    Content = "Nice post! Looking forward to reading more.",
                    UserId = bob.Id,
                    PostId = post1.Id
                },

                new Comment
                {
                    Content = "Thanks! I plan to keep adding more as I learn.",
                    UserId = alice.Id,
                    PostId = post1.Id
                },

                new Comment
                {
                    Content = "This was helpful. EF Core relationships confused me at first too.",
                    UserId = alice.Id,
                    PostId = post2.Id
                }
            };

            db.Comments.AddRange(comments);
            db.SaveChanges();
        }
    }
}