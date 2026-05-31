using Application.DTOs;
using Application.Interfaces;
using Domain.Entites;
using Microsoft.EntityFrameworkCore;

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
            var task = new TaskItem
            {
                Title = dto.Title,
                Description = dto.Description,
                Status = dto.Status,
                Priority = dto.Priority,
                DueDate = dto.DueDate,
                Category = dto.Category,
                UserId = userId
            };
            _context.Tasks.Add(task);
            _context.SaveChanges();
        }




        void ITaskService.Delete(int id, int userId)
        {
            var task = _context.Tasks.FirstOrDefault(t => t.Id == id && t.UserId == userId);
            if (task == null)
                throw new Exception("Task not found or access denied.");
            //_context.Tasks.Remove(task);
            // now we will do soft delete 
            task.IsDeleted = true;
            task.DeletedAt = DateTime.UtcNow;
            task.DeletedBy= userId;
            _context.SaveChanges();
        }

        void ITaskService.DeleteAsAdmin(int id)
        {
            var task = _context.Tasks.FirstOrDefault(t => t.Id == id);
            if (task == null)
                throw new Exception("Task not found.");
            // Soft delete 
            task.IsDeleted = true;
            task.DeletedAt = DateTime.UtcNow;
            task.DeletedBy = 0; // System/Admin delete
            _context.SaveChanges();
        }

        // for optional admin to permanently delete a task
        public void PermanentlyDelete(int id)
        {
            var task=_context.Tasks.IgnoreQueryFilters().FirstOrDefault(t => t.Id == id);
            if(task!=null)
            {
                _context.Tasks.Remove(task);
                _context.SaveChanges();
            }

        }

        List<TaskResponseDto> ITaskService.GetAllByUserId(int userId)
        {
            var tasks = _context.Tasks
                .Where(t => t.UserId == userId)
                .Join(_context.Users,
                    task => task.UserId,
                    user => user.Id,
                    (task, user) => new TaskResponseDto
                    {
                        Id = task.Id,
                        Title = task.Title,
                        Description = task.Description,
                        Status = task.Status,
                        Priority = task.Priority,
                        DueDate = task.DueDate,
                        UserId = task.UserId,
                        Username = user.Username,
                        Category = task.Category,
                    })
                .ToList();
            return tasks;
        }




        List<TaskResponseDto> ITaskService.GetAllTasks()
        {
            var tasks = _context.Tasks
                .Join(_context.Users,
                    task => task.UserId,
                    user => user.Id,
                    (task, user) => new TaskResponseDto
                    {
                        Id = task.Id,
                        Title = task.Title,
                        Description = task.Description,
                        Status = task.Status,
                        Priority = task.Priority,
                        DueDate = task.DueDate,
                        UserId = task.UserId,
                        Username = user.Username,
                        Category = task.Category,
                    })
                .ToList();
            return tasks;
        }


        TaskResponseDto ITaskService.GetById(int id, int userId)
        {
            var task = _context.Tasks
                .Where(t => t.Id == id && t.UserId == userId)
                .Join(_context.Users,
                    task => task.UserId,
                    user => user.Id,
                    (task, user) => new TaskResponseDto
                    {
                        Id = task.Id,
                        Title = task.Title,
                        Description = task.Description,
                        Status = task.Status,
                        Priority = task.Priority,
                        DueDate = task.DueDate,
                        UserId = task.UserId,
                        Username = user.Username,
                        Category = task.Category,
                    })
                .FirstOrDefault();

            if (task == null)
                throw new Exception("Task not found or access denied.");

            return task;
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

        void ITaskService.Update(UpdateTaskDto dto, int userId)
        {
            var task = _context.Tasks.FirstOrDefault(_t => _t.Id == dto.Id && _t.UserId == userId);
            if (task == null) throw new Exception("Task not found or access denied");
            task.Title = dto.Title;
            task.Description = dto.Description;
            task.Status = dto.Status;
            task.Priority = dto.Priority;
            task.DueDate = dto.DueDate;
            task.Category = dto.Category;

            _context.SaveChanges();
        }

        void ITaskService.UpdateAsAdmin(UpdateTaskDto dto)
        {
            var task = _context.Tasks.FirstOrDefault(_t => _t.Id == dto.Id);
            if (task == null) throw new Exception("Task not found");
            task.Title = dto.Title;
            task.Description = dto.Description;
            task.Status = dto.Status;
            task.Priority = dto.Priority;
            task.DueDate = dto.DueDate;
            task.Category = dto.Category;

            _context.SaveChanges();
        }

        void ITaskService.ReassignTask(int taskId, int newUserId)
        {
            var task = _context.Tasks.FirstOrDefault(t => t.Id == taskId);
            if (task == null)
                throw new Exception("Task not found");

            var user = _context.Users.FirstOrDefault(u => u.Id == newUserId);
            if (user == null)
                throw new Exception("User not found");

            task.UserId = newUserId;
            _context.SaveChanges();
        }
    }
}
