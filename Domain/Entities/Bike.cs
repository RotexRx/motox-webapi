namespace MotoX.Domain.Entities
{
    public class Bike
    {
        public int Id { get; set; }
        public string Category { get; set; }  
        public string Title { get; set; }
        public string? Subtitle { get; set; }
        public string Price { get; set; } = "0";
        public string Image { get; set; }
        public string? Badge { get; set; }
    }
}
