using Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entites;

namespace Infrastructure.TaskServices
{
    public class TaskService : ITaskService
    {
        private readonly ApplicationDbContext _context;
        public TaskService(ApplicationDbContext context)
        {
            _context = context;
        }

        void ITaskService.Create(TaskItem task)
        {
           _context.Tasks.Add(task);
            _context.SaveChanges();
        }
        void ITaskService.Delete(int id)
        {
            var task = _context.Tasks.FirstOrDefault(x => x.Id == id);
            if (task != null)
            {
                _context.Tasks.Remove(task);
                _context.SaveChanges();
            }
        }

        List<TaskItem> ITaskService.GetAll()
        {
            return _context.Tasks.ToList();
        }

        TaskItem ITaskService.GetById(int id)
        {
            return _context.Tasks.FirstOrDefault(x => x.Id == id);
        }

        void ITaskService.Update(TaskItem task)
        {
            _context.Tasks.Update(task);
            _context.SaveChanges();
        }
    }
}
