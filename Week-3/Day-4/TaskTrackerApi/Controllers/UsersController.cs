using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskTrackerApi.Data;
using TaskTrackerApi.DTOs;
using TaskTrackerApi.Entities;


namespace TaskTrackerApi.Controllers;


[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{

    private readonly AppDbContext _context;


    public UsersController(AppDbContext context)
    {
        _context = context;
    }




    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
    {
        var users = await _context.Users
            .AsNoTracking()
            .Select(u => new UserDto
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email
            })
            .ToListAsync();


        return Ok(users);
    }





    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUser(int id)
    {

        var user = await _context.Users
            .AsNoTracking()
            .Where(u => u.Id == id)
            .Select(u => new UserDto
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email
            })
            .FirstOrDefaultAsync();



        if(user == null)
            return NotFound();



        return Ok(user);
    }







    [HttpPost]
    public async Task<ActionResult<UserDto>> CreateUser(
        UserCreateDto dto)
    {

        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email
        };


        _context.Users.Add(user);


        await _context.SaveChangesAsync();



        return CreatedAtAction(
            nameof(GetUser),
            new { id = user.Id },
            new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email
            });
    }









    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(
        int id,
        UserUpdateDto dto)
    {

        var user =
            await _context.Users.FindAsync(id);



        if(user == null)
            return NotFound();



        user.Name = dto.Name;

        user.Email = dto.Email;



        await _context.SaveChangesAsync();



        return NoContent();
    }








    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(
        int id)
    {

        var user =
            await _context.Users.FindAsync(id);



        if(user == null)
            return NotFound();



        _context.Users.Remove(user);



        await _context.SaveChangesAsync();



        return NoContent();
    }
}