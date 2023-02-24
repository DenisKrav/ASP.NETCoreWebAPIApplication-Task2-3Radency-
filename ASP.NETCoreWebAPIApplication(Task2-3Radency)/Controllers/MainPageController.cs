using ASP.NETCoreWebAPIApplication_Task2_3Radency_.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ASP.NETCoreWebAPIApplication_Task2_3Radency_.Controllers
{
    public class MainPageController : Controller
    {
        // Змінна для роботи з контектом даних БД
        BookLibraryContext db;

        // Конструктор, який ініціалізує БД
        public MainPageController(BookLibraryContext context)
        {
            db = context;
        }

        // Обробка запиту Get за адресою /api/books, який повертає відсортировані книжки за автором або назвою,
        // якщо сортування не вказано, то повертається просто список книг.
        [HttpGet]
        [Route("/api/books")]
        public IResult GetAllBooks(string? order)
        {
            if (order == "author")
            {
                return Results.Json(db.Books.OrderByDescending(p => p.Author).Select(c => new
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
                return Results.Json(db.Books.OrderByDescending(p => p.Title).Select(c => new
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
        }

        // Обробка запиту Get за адресою /api/recommended, який повертає перших 10 відсортованих за рейтингом книг з певним жанром,
        // якщо жанр не вказано, то просто перших 10 книг з найбільшим рейтингом.
        [HttpGet]
        [Route("/api/recommended")]
        public IResult GetRecomendedBooks(string? genre)
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
        }

        // Обробка запиту Get за адресою /api/books/{id:int}, який повертає книгу за вказним id з повним списком усіх параметрів
        // та навіть з додатковою колекцією відгуків саме для заданої книги.
        [HttpGet]
        [Route("/api/books/{id:int}")]
        public IResult GetBook(int id)
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
        }

        // Обробка запиту Delete, який видяляє книжку за вказаним id, а також перевіряє чи було задано секретне слово, та чи
        // правильне воно.
        [HttpDelete]
        [Route("/api/books/{id:int}")]
        public async Task<IResult> DeleteBook(int id, string secret, IConfiguration conf) 
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
        }

        // Обробка запиту Post, який оброблює маршрут /api/books/save та сзберігає нову книгу, якщо не було вказано id,
        // інакше дані оновлюються.
        [HttpPost]
        [Route("/api/books/save")]
        public async Task<IResult> AddBook(Book book)
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
        }

        // Обробка запиту Put, який оброблює маршрут /api/books/{id:int}/review, та створює новий відгук.
        [HttpPut]
        [Route("/api/books/{id:int}/review")]
        public async Task<IResult> AddReview(int id, Review review)
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
        }

        // Обробка запиту Put, який оброблює маршрут /api/books/{id}/rate, та створює новий рейтинг.
        [HttpPut]
        [Route("/api/books/{id}/rate")]
        public async Task<IResult> AddRating(int id, Rating rating)
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
        }
    }
}
