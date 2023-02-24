using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using ASP.NETCoreWebAPIApplication_Task2_3Radency_.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using System;

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



            // Запиту Get за адресою /api/books, який повертає відсортировані книжки за автором або назвою,
            // якщо сортування не вказано, то повертається просто список книг.
            app.MapGet("/api/books", (string? order, BookStoreContext db) =>
            {
                if (order == "author")
                {
                    return Results.Json(db.Books.OrderBy(p => p.Author).Select(c => new
                    {
                        id = c.Id,
                        title = c.Title,
                        author = c.Author,
                        rating = db.Ratings.Where(i => i.BookId == c.Id).Average(p => p.Scope),
                        reviewsNumber = db.Reviews.Where(d => d.BookId == c.Id).Count()
                    }));
                }
                else if (order == "title")
                {
                    return Results.Json(db.Books.OrderBy(p => p.Title).Select(c => new
                    {
                        id = c.Id,
                        title = c.Title,
                        author = c.Author,
                        rating = db.Ratings.Where(i => i.BookId == c.Id).Average(p => p.Scope),
                        reviewsNumber = db.Reviews.Where(d => d.BookId == c.Id).Count()
                    }));
                }
                else
                {
                    return Results.Json(db.Books.Select(c => new
                    {
                        id = c.Id,
                        title = c.Title,
                        author = c.Author,
                        rating = db.Ratings.Where(i => i.BookId == c.Id).Average(p => p.Scope),
                        reviewsNumber = db.Reviews.Where(d => d.BookId == c.Id).Count()
                    }));
                }
            });

            // Запит Get за адресою /api/recommended, який повертає перших 10 відсортованих за рейтингом книг з певним жанром,
            // якщо жанр не вказано, то просто перших 10 книг з найбільшим рейтингом
            app.MapGet("/api/recommended", (string? genre, BookStoreContext db) =>
            {
                if (genre != null)
                {
                    return Results.Json(db.Books.Where(p => p.Genre == genre).Select(c => new
                    {
                        id = c.Id,
                        title = c.Title,
                        author = c.Author,
                        rating = db.Ratings.Where(i => i.BookId == c.Id).Average(p => p.Scope),
                        reviewsNumber = db.Reviews.Where(d => d.BookId == c.Id).Count()
                    }).OrderByDescending(l => l.rating).Take(10));
                }
                else
                {
                    return Results.Json(db.Books.Select(c => new
                    {
                        id = c.Id,
                        title = c.Title,
                        author = c.Author,
                        rating = db.Ratings.Where(i => i.BookId == c.Id).Average(p => p.Scope),
                        reviewsNumber = db.Reviews.Where(d => d.BookId == c.Id).Count()
                    }).OrderByDescending(l => l.rating).Take(10));
                }
            });

            // Запит Get за адресою /api/books/{id:int}, який повертає книгу за вказним id з повним списком усіх параметрів
            // та навіть з додатковою колекцією відгуків саме для заданої книги
            app.MapGet("/api/books/{id:int}", (int id, BookStoreContext db) =>
            {
                return Results.Json(db.Books.Where(p => p.Id == id).Select(c => new
                {
                    id = c.Id,
                    title = c.Title,
                    author = c.Author,
                    cover = c.Cover,
                    content = c.Content,
                    rating = db.Ratings.Where(i => i.BookId == c.Id).Average(p => p.Scope),
                    reviews = db.Reviews.Where(d => d.BookId == c.Id).Select(p => new
                    {
                        id = p.Id,
                        message = p.Message,
                        reviewer = p.Reviewer
                    }).ToList()
                }));
            });

            // Запит Delete, який видяляє книжку за вказаним id, а також перевіряє чи було задано секретне слово, та чи
            // правильне воно
            app.MapDelete("/api/books/{id:int}", async (int id, string secret, BookStoreContext db, IConfiguration conf) =>
            {
                // Якщо кодове слово вірне, то продовжуємо видалення, якщо ні, то надсилаємо статусний код з
                // повідомленням про помилку
                if (secret == conf["MySettings:SecretWord"])
                {
                    // Отримуємо книгу по id
                    Book? book = await db.Books.FirstOrDefaultAsync(u => u.Id == id);

                    // Якщо книга не знайдена, то надсилаємо статусний код помилки інакше видаляємо книгу і надсилаємо результат
                    if (book == null)
                    {
                        return Results.NotFound(new { message = "Книга не знайдена." });
                    }
                    else
                    {
                        // Видаляємо усі пов'язані з книгою відгуки та рейтинги
                        foreach (Review review in db.Reviews)
                        {
                            if (review.BookId == book.Id)
                            {
                                db.Reviews.Remove(review);
                                await db.SaveChangesAsync();
                            }
                        }
                        foreach (Rating rating in db.Ratings)
                        {
                            if (rating.BookId == book.Id)
                            {
                                db.Ratings.Remove(rating);
                                await db.SaveChangesAsync();
                            }
                        }
                        // Видаляємо книгу
                        db.Books.Remove(book);
                        await db.SaveChangesAsync();

                        return Results.Json(book);
                    }
                }
                else
                {
                    return Results.NotFound(new { message = "Для проведення такої операцї потрібно вказати кодове слово" });
                }
            });

            // Запит Post, який оброблює маршрут /api/books/save та сзберігає нову книгу, якщо не було вказано id,
            // інакше дані оновлюються 
            app.MapPost("/api/books/save", async (Book book, BookStoreContext db) =>
            {
                Book? b = await db.Books.FirstOrDefaultAsync(c => c.Id == book.Id);

                if (b == null)
                {
                    await db.Books.AddAsync(book);
                    await db.SaveChangesAsync();
                    return Results.Json(book.Id);
                }
                else
                {
                    b.Title = book.Title;
                    b.Cover = book.Cover;
                    b.Content = book.Content;
                    b.Author = book.Author;
                    b.Genre = book.Genre;
                    await db.SaveChangesAsync();
                    return Results.Json(book.Id);
                }
            });

            // Запит Put, який оброблює маршрут /api/books/{id:int}/review, та створює новий відгук
            app.MapPut("/api/books/{id:int}/review", async (int id, Review review, BookStoreContext db) =>
            {
                Book? book = await db.Books.FindAsync(id);
                if (book == null)
                {
                    return Results.NotFound();
                }
                
                review.BookId = book.Id;
                db.Reviews.Add(review);
                await db.SaveChangesAsync();

                return Results.Json(review.Id);
            });

            // Запит Put, який оброблює маршрут /api/books/{id}/rate, та створює новий рейтинг
            app.MapPut("/api/books/{id}/rate", async (int id, Rating rating, BookStoreContext db) =>
            {
                Book? book = await db.Books.FindAsync(id);
                if (book == null)
                {
                    return Results.NotFound();
                }

                rating.BookId = book.Id;
                db.Ratings.Add(rating);
                await db.SaveChangesAsync();

                return Results.Json(rating.Id);
            });



            app.Run();
        }
    }
}