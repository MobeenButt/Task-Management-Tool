using System;
using System.Collections.Generic;
using System.Text;
using TaskManagement.Domain;

namespace TaskManagement.Application.Interfaces.Persistence
{
    public interface ITaskService
    {
        List<TaskItem> GetAll();
        TaskItem GetById(int id);
        void Create(TaskItem task);
        void Update(TaskItem task);
        void Delete(int id);


    }
}
