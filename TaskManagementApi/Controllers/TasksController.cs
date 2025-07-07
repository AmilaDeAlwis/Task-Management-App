using Microsoft.AspNetCore.Mvc;
using TaskManagementApi.Filter;
using TaskManagementApi.Interfaces;
using ModelTask = TaskManagementApi.Models.Task;

namespace TaskManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [BasicAuth]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly ILogger<TasksController> _logger;

        public TasksController(ITaskService taskService, ILogger<TasksController> logger)
        {
            _taskService = taskService;
            _logger = logger;
        }

        // GET: api/Tasks
        [HttpGet]
        public async System.Threading.Tasks.Task<ActionResult<IEnumerable<ModelTask>>> GetTasks()
        {
            _logger.LogInformation("Attempting to retrieve all tasks.");
            var tasks = await _taskService.GetAllTasksAsync();
            return Ok(tasks);
        }

        // GET: api/Tasks/5
        [HttpGet("{id}")]
        public async System.Threading.Tasks.Task<ActionResult<ModelTask>> GetTask(int id)
        {
            _logger.LogInformation($"Attempting to retrieve task with ID: {id}");
            var task = await _taskService.GetTaskByIdAsync(id);

            if (task == null)
            {
                _logger.LogWarning($"Task with ID {id} not found.");
                return NotFound();
            }

            return Ok(task);
        }

        // POST: api/Tasks
        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult<ModelTask>> PostTask(ModelTask task)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for new task creation.");
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Attempting to create a new task.");
            var createdTask = await _taskService.AddTaskAsync(task);
            return CreatedAtAction(nameof(GetTask), new { id = createdTask.Id }, createdTask);
        }

        // PUT: api/Tasks/5
        [HttpPut("{id}")]
        public async System.Threading.Tasks.Task<IActionResult> PutTask(int id, ModelTask task)
        {
            if (id != task.Id)
            {
                _logger.LogWarning($"Mismatch between route ID ({id}) and task ID ({task.Id}) during update.");
                return BadRequest("Task ID in URL does not match task ID in body.");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for task update.");
                return BadRequest(ModelState);
            }

            try
            {
                _logger.LogInformation($"Attempting to update task with ID: {id}");
                await _taskService.UpdateTaskAsync(id, task);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, $"Task with ID {id} not found during update.");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating task with ID {id}.");
                throw;
            }

            return NoContent();
        }

        // DELETE: api/Tasks/5
        [HttpDelete("{id}")]
        public async System.Threading.Tasks.Task<IActionResult> DeleteTask(int id)
        {
            try
            {
                _logger.LogInformation($"Attempting to delete task with ID: {id}");
                await _taskService.DeleteTaskAsync(id);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, $"Task with ID {id} not found for deletion.");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting task with ID {id}.");
                throw;
            }

            return NoContent();
        }
    }
}
