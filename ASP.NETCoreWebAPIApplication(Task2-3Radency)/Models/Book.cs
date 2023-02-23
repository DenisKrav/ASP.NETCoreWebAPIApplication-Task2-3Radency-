using System.ComponentModel.DataAnnotations;

namespace ASP.NETCoreWebAPIApplication_Task2_3Radency_.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Cover { get; set; }
        public string? Content { get; set; }
        public string? Author { get; set; }
        public string? Genre { get; set; }
    }
}
