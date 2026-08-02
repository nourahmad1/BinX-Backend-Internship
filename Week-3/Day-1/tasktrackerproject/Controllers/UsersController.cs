using Microsoft.AspNetCore.Mvc;
using TaskTrackerApi.Data;
using TaskTrackerApi.Models;

namespace TaskTrackerApi.Controllers;


[ApiController]
[Route("api/v1/users")]
public class UsersController : ControllerBase
{

    // GET: api/v1/users
    [HttpGet]
    public IActionResult GetUsers()
    {
        return Ok(AppData.Users);
    }



    // GET: api/v1/users/1
    [HttpGet("{id}")]
    public IActionResult GetUserById(int id)
    {
        var user = AppData.Users
            .FirstOrDefault(u => u.Id == id);


        if (user == null)
        {
            return NotFound();
        }


        return Ok(user);
    }




    // POST: api/v1/users
    [HttpPost]
    public IActionResult CreateUser(User user)
    {

        user.Id = AppData.Users.Count + 1;


        AppData.Users.Add(user);


        return CreatedAtAction(
            nameof(GetUserById),
            new { id = user.Id },
            user
        );
    }




    // PUT: api/v1/users/1
    [HttpPut("{id}")]
    public IActionResult UpdateUser(int id, User updatedUser)
    {

        var user = AppData.Users
            .FirstOrDefault(u => u.Id == id);



        if (user == null)
        {
            return NotFound();
        }



        user.Name = updatedUser.Name;

        user.Email = updatedUser.Email;



        return Ok(user);
    }





    // DELETE: api/v1/users/1
    [HttpDelete("{id}")]
    public IActionResult DeleteUser(int id)
    {

        var user = AppData.Users
            .FirstOrDefault(u => u.Id == id);



        if (user == null)
        {
            return NotFound();
        }



        AppData.Users.Remove(user);



        return NoContent();
    }
    // GET: api/v1/users/1/tasks
[HttpGet("{id}/tasks")]
public IActionResult GetUserTasks(int id)
{
    var userExists = AppData.Users
        .Any(u => u.Id == id);


    if (!userExists)
    {
        return NotFound("User not found");
    }


    var tasks = AppData.Tasks
        .Where(t => t.UserId == id)
        .ToList();


    return Ok(tasks);
}

}