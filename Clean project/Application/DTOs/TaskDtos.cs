using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class CreateTaskDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public int Priority { get; set; }
        public DateTime DueDate { get; set; }

    }
    public class UpdateTaskDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public int Priority { get; set; }
        public DateTime DueDate { get; set; }
    }
    public class TaskResponseDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public int Priority { get; set; }
        public DateTime DueDate { get; set; }
        public int UserId { get; set; }
        public string? Username { get; set; }
    }
}
