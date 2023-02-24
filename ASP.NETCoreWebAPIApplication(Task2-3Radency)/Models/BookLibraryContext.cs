using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ASP.NETCoreWebAPIApplication_Task2_3Radency_.Models
{
    public class BookLibraryContext : DbContext
    {
        public DbSet<Book> Books { get; set; } = null!;
        public DbSet<Rating> Ratings { get; set; } = null!;
        public DbSet<Review> Reviews { get; set; } = null!;

        public BookLibraryContext(DbContextOptions<BookLibraryContext> options) : base(options)
        {
            // Викликаємо метод Database.EnsureCreated(), для генерації бд та наповнення її початковими значеннями 
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Додаємо початкові дані
            Book book1 = new Book() 
            {
                Id = 1, 
                Title = "The Great Gatsby", 
                Cover = "https://images.gr-assets.com/books/1490528560l/4671.jpg",
                Content = "The story primarily concerns the young and mysterious millionaire Jay Gatsby and his quixotic passion and obsession with the beautiful former debutante Daisy Buchanan.", 
                Author = "F. Scott Fitzgerald", 
                Genre = "Fiction" 
            };
            Book book2 = new Book() 
            { 
                Id = 2, 
                Title = "To Kill a Mockingbird", 
                Cover = "https://images.gr-assets.com/books/1361975680l/2657.jpg",
                Content = "The unforgettable novel of a childhood in a sleepy Southern town and the crisis of conscience that rocked it.", 
                Author = "Harper Lee", 
                Genre = "Fiction" 
            };
            Book book3 = new Book() 
            {
                Id = 3, 
                Title = "The Lord of the Rings", 
                Cover = "https://images.gr-assets.com/books/1346072396l/33.jpg", 
                Content = "The Lord of the Rings tells of the great quest undertaken by Frodo and the Fellowship of the Ring: Gandalf the wizard; the hobbits Merry, Pippin, and Sam; Gimli the dwarf; Legolas the elf; Boromir of Gondor; and a tall, mysterious stranger called Strider.",
                Author = "J.R.R. Tolkien", 
                Genre = "Fantasy" 
            };
            Book book4 = new Book() 
            {
                Id = 4, 
                Title = "1984", 
                Cover = "https://images.gr-assets.com/books/1532714506l/5470.jpg", 
                Content = "The story takes place in an imagined future, the year 1984, when much of the world has fallen victim to perpetual war, omnipresent government surveillance, historical negationism, and propaganda.", 
                Author = "George Orwell", 
                Genre = "Science Fiction" 
            };
            Book book5 = new Book() 
            { 
                Id = 5, 
                Title = "Pride and Prejudice",
                Cover = "https://images.gr-assets.com/books/1320399351l/1885.jpg", 
                Content = "It follows the character development of Elizabeth Bennet, who learns the error of making hasty judgments and comes to appreciate the difference between the superficial and the essential.", 
                Author = "Jane Austen", 
                Genre = "Romance" 
            };

            Review review1 = new Review
            {
                Id = 1,
                Message = "Great book, highly recommend!",
                Reviewer = "John Doe",
                BookId = book1.Id
            };
            Review review2 = new Review
            {
                Id = 2,
                Message = "Disappointing read, wouldn't recommend.",
                Reviewer = "Jane Smith",
                BookId = book2.Id
            };
            Review review3 = new Review
            {
                Id = 3,
                Message = "A well-written book with a compelling story.",
                Reviewer = "Alex Johnson",
                BookId = book3.Id
            };
            Review review4 = new Review
            {
                Id = 4,
                Message = "I couldn't put this book down, it was so engaging!",
                Reviewer = "Samantha Lee",
                BookId = book4.Id
            };
            Review review5 = new Review
            {
                Id = 5,
                Message = "This book was just okay, nothing special.",
                Reviewer = "Robert Brown",
                BookId = book5.Id
            };
            Review review6 = new Review
            {
                Id = 6,
                Message = "Good book.",
                Reviewer = "Jane Smith",
                BookId = book1.Id
            };

            Rating rating1 = new Rating
            {
                Id = 1,
                Scope = 4,
                BookId = book1.Id
            };
            Rating rating2 = new Rating
            {
                Id = 2,
                Scope = 3,
                BookId = book2.Id
            };
            Rating rating3 = new Rating
            {
                Id = 3,
                Scope = 4,
                BookId = book3.Id
            };
            Rating rating4 = new Rating
            {
                Id = 4,
                Scope = 4,
                BookId = book4.Id
            };
            Rating rating5 = new Rating
            {
                Id = 5,
                Scope = 4,
                BookId = book5.Id
            };

            modelBuilder.Entity<Book>().HasData(book1, book2, book3, book4, book5);
            modelBuilder.Entity<Review>().HasData(review1, review2, review3, review4, review5, review6);
            modelBuilder.Entity<Rating>().HasData(rating1, rating2, rating3, rating4, rating5);
        }
    }
}
