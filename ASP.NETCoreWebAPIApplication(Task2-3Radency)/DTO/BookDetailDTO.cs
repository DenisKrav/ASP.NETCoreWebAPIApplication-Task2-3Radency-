using ASP.NETCoreWebAPIApplication_Task2_3Radency_.Models;

namespace ASP.NETCoreWebAPIApplication_Task2_3Radency_.DTO
{
    public class BookDetailDTO
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Author { get; set; }
        public string? Cover { get; set; }
        public string? Content { get; set; }
        public double Rating { get; set; }
        public List<Review>? Reviews { get; set; }
    }
}
