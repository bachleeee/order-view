namespace internship.Models
{
    public class ItemModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }

        public float PriceS { get; set; }
        public float PriceM { get; set; }
        public float PriceL { get; set; }
    }

    public class SearchRequest
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public decimal? PriceMin { get; set; }
        public decimal? PriceMax { get; set; }
    }

}
