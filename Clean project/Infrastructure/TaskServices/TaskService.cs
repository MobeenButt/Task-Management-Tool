using Application.DTOs;
using Application.Interfaces;
using Domain.Entites;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Infrastructure.TaskServices
{
    public class TaskService : ITaskService
    {
        private readonly ApplicationDbContext _context;
        public TaskService(ApplicationDbContext context)
        {
            _context = context;
        }

        void ITaskService.Create(CreateTaskDto dto, int userId)
        {
           var task=new TaskItem
            {
                Title=dto.Title,
                Description=dto.Description,
                Status=dto.Status,
                Priority=dto.Priority,
                DueDate=dto.DueDate,
                UserId=userId
            };
            _context.Tasks.Add(task);
            _context.SaveChanges();
        }




        void ITaskService.Delete(int id, int userId)
        {
            var task=_context.Tasks.FirstOrDefault(t=>t.Id==id && t.UserId==userId);
            if(task==null)
                throw new Exception("Task not found or access denied.");
            _context.Tasks.Remove(task);
            _context.SaveChanges();
        }

        List<TaskResponseDto> ITaskService.GetAllByUserId(int userId)
        {
            var tasks=_context.Tasks.Where(t=>t.UserId==userId).ToList();
            return tasks.Select(t=>new TaskResponseDto
            {
                Id=t.Id,
                Title=t.Title,
                Description=t.Description,
                Status=t.Status,
                Priority=t.Priority,
                DueDate=t.DueDate,
                UserId=t.UserId
            }).ToList();
        }
           



        List<TaskResponseDto> ITaskService.GetAllTasks()
        {
            var tasks = _context.Tasks.ToList();
            return tasks.Select(t=>new TaskResponseDto
            {
                Id=t.Id,
                Title=t.Title,
                Description=t.Description,
                Status=t.Status,
                Priority=t.Priority,
                DueDate=t.DueDate,
                UserId=t.UserId
            }).ToList();
        }


        TaskResponseDto ITaskService.GetById(int id, int userId)
        {
            var task = _context.Tasks.FirstOrDefault(t => t.Id == id && t.UserId == userId);
            if (task == null)
                throw new Exception("Task not found or access denied.");
            return new TaskResponseDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status,
                Priority = task.Priority,
                DueDate = task.DueDate,
                UserId = task.UserId
            };
        }

        Dictionary<string, int> ITaskService.GetTaskCountByStatus(int? userId)
        {
            var tasks = userId.HasValue ? _context.Tasks.Where(t => t.UserId == userId.Value).ToList() : _context.Tasks.ToList();
            return new Dictionary<string, int>
            {
                { "Pending", tasks.Count(t => t.Status == "Pending") },
                { "InProgress", tasks.Count(t => t.Status == "InProgress") },
                { "Completed", tasks.Count(t => t.Status == "Completed") }
            };
        }

        void ITaskService.Update(UpdateTaskDto task, int userId)
        {
            throw new NotImplementedException();
        }
    }
}
