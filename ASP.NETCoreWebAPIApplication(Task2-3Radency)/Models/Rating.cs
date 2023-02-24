using System.ComponentModel.DataAnnotations;

namespace ASP.NETCoreWebAPIApplication_Task2_3Radency_.Models
{
    public class Rating
    {
        [Key]
        public int Id { get; set; }
        public int Scope { get; set; }

        public int BookId { get; set; }
        public Book? Book { get; set; }
    }
}
