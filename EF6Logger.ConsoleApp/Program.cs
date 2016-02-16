using EF6Logger;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Interception;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace EF6LoggerConsoleTestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            GetMsdnBlogs();
            GetBlogs();
        }

        public static Blog[] GetMsdnBlogs()
        {
            using (var db = new BloggingContext())
            {
                return db.Blogs
                    .Where(b => b.Url.Contains("msdn"))
                    .OrderBy(b => b.Url)
                    .ToArray();
            }
        }

        public static Blog[] GetBlogs()
        {
            using (var db = new BloggingContext())
            {
                return db.Blogs
                    .SqlQuery("SELECT * FROM dbo.wrong_table")
                    .ToArray();
            }
        }
    }

    public class BloggingContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }
    }

    public class Blog
    {
        public int BlogId { get; set; }
        public string Url { get; set; }

        public List<Post> Posts { get; set; }
    }

    public class Post
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public int BlogId { get; set; }
        public Blog Blog { get; set; }
    }

    public class MyConfig : DbConfiguration
    {
        public MyConfig()
        {
            this.AddInterceptor(new ExpensiveSqlLoggerInterceptor(new OutputWindowLogger(), 1));
        }
    }
}