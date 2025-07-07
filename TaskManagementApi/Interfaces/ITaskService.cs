using ModelTask = TaskManagementApi.Models.Task;

namespace TaskManagementApi.Interfaces
{
    public interface ITaskService
    {
        System.Threading.Tasks.Task<IEnumerable<ModelTask>> GetAllTasksAsync();
        System.Threading.Tasks.Task<ModelTask> GetTaskByIdAsync(int id);
        System.Threading.Tasks.Task<ModelTask> AddTaskAsync(ModelTask task);
        System.Threading.Tasks.Task UpdateTaskAsync(int id, ModelTask task);
        System.Threading.Tasks.Task DeleteTaskAsync(int id);
        System.Threading.Tasks.Task<bool> TaskExistsAsync(int id);
    }
}
