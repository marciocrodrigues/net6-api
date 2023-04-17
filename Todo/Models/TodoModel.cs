namespace Todo.Models
{
    public class TodoModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool Done { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class TodoUpdateModel
    {
        public int Id { get; set; }
        public bool Done { get; set; }
    }
}