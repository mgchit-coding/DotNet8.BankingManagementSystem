﻿namespace DotNet8.BankingManagementSystem.Backend.Features;

[ApiController]
public class BaseController : ControllerBase
{
    [NonAction]
    protected IActionResult InternalServerError(Exception exception)
    {
            return Ok(new
            {
                Response = new MessageResponseModel(false, exception)
            });
        }
}