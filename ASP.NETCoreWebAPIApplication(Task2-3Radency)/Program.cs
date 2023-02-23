using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using ASP.NETCoreWebAPIApplication_Task2_3Radency_.Models;
using Microsoft.EntityFrameworkCore;

namespace ASP.NETCoreWebAPIApplication_Task2_3Radency_
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            // Реєструємо контекст даних як сервіс, при чому використовуємо розширення для SQLite, для інших треба буде
            // встановлювати інший пакет розширення, і по іншому показувати показувати підключення до БД.
            // В даному випадку ми кажемо, що дані зберігаються у пам'яті
            builder.Services.AddDbContext<BookStoreContext>(options =>
            {
                options.UseInMemoryDatabase("MyDatabase");
            });

            var app = builder.Build();



            app.MapGet("/api/books", (string? order, BookStoreContext db) =>
            {
                if (order == "author")
                {
                    return Results.Json(db.Books.Join(db.Ratings,    
                        u => u.Id,    
                        c => c.BookId,    
                        (u, c) => new    
                        {       
                            id = u.Id,      
                            title = u.Title,       
                            author = u.Author,        
                            rating = c.Scope,       
                            reviewsNumber = db.Reviews.Where(d => d.BookId == u.Id).Count()  
                        }).OrderBy(p => p.author));
                }
                else if (order == "title")
                {
                    return Results.Json(db.Books.Join(db.Ratings,
                        u => u.Id,
                        c => c.BookId,
                        (u, c) => new
                        {
                            id = u.Id,
                            title = u.Title,
                            author = u.Author,
                            rating = c.Scope,
                            reviewsNumber = db.Reviews.Where(d => d.BookId == u.Id).Count()
                        }).OrderBy(p => p.title));
                }
                else
                {
                    return Results.Json(db.Books.Join(db.Ratings,
                        u => u.Id,
                        c => c.BookId,
                        (u, c) => new
                        { id = u.Id,
                            title = u.Title,
                            author = u.Author,
                            rating = c.Scope,
                            reviewsNumber = db.Reviews.Where(d => d.BookId == u.Id).Count()
                        }));
                }
            });

            //app.MapGet("/api/recommended", (string? genre, BookStoreContext db) =>
            //{
            //    if (order == "author")
            //    {
            //        return Results.Json(db.Books.Where(p => p.Genre == genre).OrderBy(p => p.));
            //    }
            //    else if (order == "title")
            //    {
            //        return Results.Json(db.Books.OrderBy(p => p.Title));
            //    }
            //    else
            //    {
            //        return Results.Json(db.Books.Where(p => p.Genre == genre));
            //    }
            //});



            app.Run();
        }
    }
}