using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace htddict.Models
{
    public class Entry
    {
        public int Id {  get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(25, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 25 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Content is required")]
        [StringLength(5000, MinimumLength = 8, ErrorMessage = "Content must be at least 8 characters")]
        public string Content { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

        public int AuthorId { get; set; }

        [ForeignKey("AuthorId")]
        public User? Author { get; set; }
    }
}
