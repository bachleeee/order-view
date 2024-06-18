namespace internship.Models
{
    public class OrderDetail
    {
        public int ItemId { get; set; }
        public int Quantity { get; set; }
        public String Size { get; set; }
        public decimal Price { get; set; }
    }

    public class UpdateOrderDetailRequest
    {
        public int ItemId { get; set; }
        public string Size { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string Action { get; set; }  // "increase" or "decrease"
    }

}
