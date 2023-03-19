namespace Shop.Data.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int GraphicPhotoId { get; set; }
        public int Amount { get; set; }
        public decimal Price { get; set; }
        public virtual GraphicPhoto GraphicPhoto{ get; set; }
        public virtual Order Order { get; set; }
    }
}