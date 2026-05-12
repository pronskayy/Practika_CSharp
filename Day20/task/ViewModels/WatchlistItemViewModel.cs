using System.ComponentModel.DataAnnotations;

namespace WatchList.ViewModels
{
    public class WatchlistItemViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Название фильма/сериала обязательно")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Название должно быть от 2 до 100 символов")]
        [Display(Name = "Название")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Укажите жанр")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Жанр должен быть от 2 до 50 символов")]
        [Display(Name = "Жанр")]
        public string Genre { get; set; }

        [Required(ErrorMessage = "Выберите тип контента")]
        [Display(Name = "Тип")]
        public string Type { get; set; } // "Film" или "Series"

        [Required(ErrorMessage = "Выберите статус просмотра")]
        [Display(Name = "Статус")]
        public string Status { get; set; } // "PlanToWatch", "Watching", "Watched"

        [Range(1, 10, ErrorMessage = "Рейтинг должен быть от 1 до 10")]
        [Display(Name = "Рейтинг")]
        public int? Rating { get; set; }

        [Display(Name = "Дата добавления")]
        public DateTime DateAdded { get; set; } = DateTime.Now;
    }
}