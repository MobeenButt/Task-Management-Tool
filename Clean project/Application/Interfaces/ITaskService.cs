using Domain.Entites;
namespace Application.Interfaces
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
