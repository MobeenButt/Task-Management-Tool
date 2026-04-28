namespace Domain.Entites
{
    public class TaskItem
    {
        public int Id { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }

        public string Status { get; set; } // Pending, InProgress, Completed
        public string? Category { get; set; }
        public int Priority { get; set; }

        public DateTime DueDate { get; set; }

        public int UserId { get; set; }

    }
}
