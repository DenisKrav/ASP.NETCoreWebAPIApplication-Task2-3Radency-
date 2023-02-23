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

            // �������� �������� ����� �� �����, ��� ���� ������������� ���������� ��� SQLite, ��� ����� ����� ����
            // ������������� ����� ����� ����������, � �� ������ ���������� ���������� ���������� �� ��.
            // � ������ ������� �� ������, �� ��� ����������� � ���'��
            builder.Services.AddDbContext<BookStoreContext>(options =>
            {
                options.UseInMemoryDatabase("MyDatabase");
            });

            var app = builder.Build();



            // ������ Get �� ������� /api/books, ���� ������� ������������ ������ �� ������� ��� ������,
            // ���� ���������� �� �������, �� ����������� ������ ������ ����.
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

            // ����� Get �� ������� /api/recommended, ���� ������� ������ 10 ������������ �� ��������� ���� � ������ ������,
            // ���� ���� �� �������, �� ������ ������ 10 ���� � ��������� ���������
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

            // ����� Get �� ������� /api/books/{id:int}, ���� ������� ����� �� ������� id � ������ ������� ��� ���������
            // �� ����� � ���������� ��������� ������ ���� ��� ������ �����
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

            // ����� Delete, ���� ������� ������ �� �������� id, � ����� �������� �� ���� ������ �������� �����, �� ��
            // ��������� ����
            app.MapDelete("/api/books/{id:int}", (int id, string secret, BookStoreContext db, IConfiguration conf) =>
            {
                // ���� ������ ����� ����, �� ���������� ���������, ���� �, �� ��������� ��������� ��� �
                // ������������ ��� �������
                if (secret == conf["MySettings:SecretWord"])
                {
                    // �������� ����� �� id
                    Book? book = db.Books.FirstOrDefault(u => u.Id == id);

                    // ���� ����� �� ��������, �� ��������� ��������� ��� ������� ������ ��������� ����� � ��������� ���������
                    if (book == null)
                    {
                        return Results.NotFound(new { message = "����� �� ��������." });
                    }
                    else
                    {
                        db.Books.Remove(book);
                        return Results.Json(book);
                    }
                }
                else
                {
                    return Results.NotFound(new { message = "��� ���������� ���� ������� ������� ������� ������ �����" });
                }
            });

            app.Run();
        }
    }
}