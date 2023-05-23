namespace GameLibraryAPI.Models
{
    public class Game
    {
        public int Id { get; set; }
        public string Title { get; set; }=null!;
        public int GenreID { get; set; }
    }
}
