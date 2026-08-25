using Microsoft.AspNetCore.Mvc;

namespace CardiacPatientMonitoring.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    [HttpGet("error")]
    public IActionResult TriggerError()
    {
        throw new Exception("This is a test exception.");
    }
}