using Domain.Entites;
using Application.DTOs;
namespace Application.Interfaces
{
    public interface ITaskService
    {
        List<TaskResponseDto> GetAllTasks(); // for admin
        List<TaskResponseDto> GetAllByUserId(int userId); // for user
        TaskResponseDto GetById(int id,int userId);
        void Create(CreateTaskDto task,int userId);
        void Update(UpdateTaskDto task, int userId);
        void UpdateAsAdmin(UpdateTaskDto task);
        void Delete(int id, int userId);
        void DeleteAsAdmin(int id);
        Dictionary<string, int> GetTaskCountByStatus(int? userId= null);
        void ReassignTask(int taskId, int newUserId);
    }
}
