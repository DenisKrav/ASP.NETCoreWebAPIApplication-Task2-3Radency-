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
            builder.Services.AddDbContext<BookLibraryContext>(options =>
            {
                options.UseInMemoryDatabase("MyDatabase");
            });
            builder.Services.AddMvc();


            var app = builder.Build();


            // ������� ��� ������������ ���������� �� ���� ������ �� ������� �����������
            app.MapControllerRoute(   
                name: "default",
                pattern: "{controller}/{action}/{id?}");

            app.Run();
        }
    }
}