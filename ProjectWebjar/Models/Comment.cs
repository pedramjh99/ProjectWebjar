namespace ProjectWebjar.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Message { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }

        public Comment(string name, string message, int productId)
        {
            Name = name;
            Message = message;
            ProductId = productId;
        }
    }
}