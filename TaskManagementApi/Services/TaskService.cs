using Microsoft.EntityFrameworkCore;
using TaskManagementApi.Data;
using TaskManagementApi.Interfaces;
using ModelTask = TaskManagementApi.Models.Task;

namespace TaskManagementApi.Services
{
    public class TaskService : ITaskService
    {
        private readonly ApplicationDbContext _context;

        public TaskService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async System.Threading.Tasks.Task<IEnumerable<ModelTask>> GetAllTasksAsync()
        {
            return await _context.Tasks.ToListAsync();
        }

        public async System.Threading.Tasks.Task<ModelTask> GetTaskByIdAsync(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                throw new KeyNotFoundException($"Task with ID {id} not found.");
            }
            return task;
        }

        public async System.Threading.Tasks.Task<ModelTask> AddTaskAsync(ModelTask task)
        {
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public async System.Threading.Tasks.Task UpdateTaskAsync(int id, ModelTask task)
        {
            _context.Entry(task).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await TaskExistsAsync(id))
                {
                    throw new KeyNotFoundException($"Task with ID {id} not found.");
                }
                else
                {
                    throw;
                }
            }
        }

        public async System.Threading.Tasks.Task DeleteTaskAsync(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                throw new KeyNotFoundException($"Task with ID {id} not found for deletion.");
            }
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
        }

        public async System.Threading.Tasks.Task<bool> TaskExistsAsync(int id)
        {
            return await _context.Tasks.AnyAsync(e => e.Id == id);
        }
    }
}
