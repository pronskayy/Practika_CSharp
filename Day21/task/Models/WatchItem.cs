using System.ComponentModel.DataAnnotations;

namespace WatchList.Models
{
    public class WatchItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Type { get; set; } = string.Empty;

        [Required]
        public string Status { get; set; } = string.Empty;
    }
}