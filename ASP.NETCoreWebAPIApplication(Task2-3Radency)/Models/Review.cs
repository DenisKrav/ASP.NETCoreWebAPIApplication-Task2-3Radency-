using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASP.NETCoreWebAPIApplication_Task2_3Radency_.Models
{
    public class Review
    {
        [Key]
        public int Id { get; set; }
        public string? Message { get; set; }
        public string? Reviewer { get; set; }

        public int BookId { get; set; }
        public Book? Book { get; set; }
    }
}
