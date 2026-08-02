using Microsoft.AspNetCore.Mvc;
using TaskTrackerApi.Data;
using TaskTrackerApi.Models;

namespace TaskTrackerApi.Controllers;


[ApiController]
[Route("api/v1/tasks")]
public class TasksController : ControllerBase
{

    // GET: api/v1/tasks
    [HttpGet]
    public IActionResult GetTasks()
    {
        return Ok(AppData.Tasks);
    }



    // GET: api/v1/tasks/1
    [HttpGet("{id}")]
    public IActionResult GetTaskById(int id)
    {
        var task = AppData.Tasks
            .FirstOrDefault(t => t.Id == id);


        if (task == null)
        {
            return NotFound();
        }


        return Ok(task);
    }





    // POST: api/v1/tasks
    [HttpPost]
    public IActionResult CreateTask(TaskItem task)
    {

        var userExists = AppData.Users
            .Any(u => u.Id == task.UserId);


        if (!userExists)
        {
            return BadRequest("User does not exist");
        }



        task.Id = AppData.Tasks.Count + 1;


        AppData.Tasks.Add(task);



        return CreatedAtAction(
            nameof(GetTaskById),
            new { id = task.Id },
            task
        );
    }





    // PUT: api/v1/tasks/1
    [HttpPut("{id}")]
    public IActionResult UpdateTask(int id, TaskItem updatedTask)
    {

        var task = AppData.Tasks
            .FirstOrDefault(t => t.Id == id);



        if (task == null)
        {
            return NotFound();
        }



        task.Title = updatedTask.Title;

        task.Description = updatedTask.Description;

        task.IsCompleted = updatedTask.IsCompleted;

        task.UserId = updatedTask.UserId;



        return Ok(task);
    }





    // DELETE: api/v1/tasks/1
    [HttpDelete("{id}")]
    public IActionResult DeleteTask(int id)
    {

        var task = AppData.Tasks
            .FirstOrDefault(t => t.Id == id);



        if (task == null)
        {
            return NotFound();
        }



        AppData.Tasks.Remove(task);



        return NoContent();
    }

}