using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Properties.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService taskService;
        public TaskController(ITaskService taskService)
        {
            this.taskService = taskService;
        }
        private int GetUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new Exception("User ID not found in token."));
        }
        private string GetUserRole()
        {
            return User.FindFirst(ClaimTypes.Role)?.Value ?? throw new Exception("User role not found in token.");
        }
        [HttpGet]
        [Route("all")]
        public IActionResult GetTasks()
        {
            try {
                var userId = GetUserId();
                var role = GetUserRole();
                if(role == "Admin")
                {
                    var allTasks = taskService.GetAllTasks();
                    return Ok(allTasks);
                }
                else
                {
                    var tasks = taskService.GetAllByUserId(userId);
                    return Ok(tasks);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpGet]
        [Route("{id}")]
        public IActionResult GetTask(int id)
        {
            try
            {
                var userId = GetUserId();
                var task = taskService.GetById(id, userId);
                return Ok(task);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpPost]
        [Route("create")]
        public IActionResult CreateTask([FromBody] CreateTaskDto dto) {
            try
            {
                var userId = GetUserId();
                taskService.Create(dto, userId);
                return Ok(new { message = "Task created successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpPut]
        [Route("update/{id}")]
        public IActionResult UpdateTask(int id,[FromBody] UpdateTaskDto dto) {
            try
            {
                var userId = GetUserId();
                dto.Id = id;
                taskService.Update(dto, userId);
                return Ok(new { message = "Task updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpDelete]
        [Route("delete/{id}")]
        public IActionResult DeleteTask(int id)
        {
            try
            {
                var userId = GetUserId();
                taskService.Delete(id, userId);
                return Ok(new { message = "Task deleted successfully" });

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpGet]
        [Route("dashboard")]
        public IActionResult GetDashboard() {
            try
            {
                var userId = GetUserId();
                var role = GetUserRole();
                if(role == "Admin")
                {
                    var dashboardData = taskService.GetTaskCountByStatus();
                    return Ok(dashboardData);
                }
                else
                {
                    var dashboardData = taskService.GetTaskCountByStatus(userId);
                    return Ok(dashboardData);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

    }
}
