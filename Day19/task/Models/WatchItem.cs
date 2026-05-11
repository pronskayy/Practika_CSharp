namespace WatchList.Models
{
    public class WatchItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }   // Фильм / Сериал
        public string Status { get; set; } // Просмотрено / Не просмотрено
    }
}