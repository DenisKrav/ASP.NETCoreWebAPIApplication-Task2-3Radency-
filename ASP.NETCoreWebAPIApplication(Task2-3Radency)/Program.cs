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
            builder.Services.AddDbContext<BookLibraryContext>(options =>
            {
                options.UseInMemoryDatabase("MyDatabase");
            });
            builder.Services.AddMvc();


            var app = builder.Build();


            // Маршрут для співставлення контролера та його методів із запитом користувача
            app.MapControllerRoute(   
                name: "default",
                pattern: "{controller}/{action}/{id?}");

            app.Run();
        }
    }
}