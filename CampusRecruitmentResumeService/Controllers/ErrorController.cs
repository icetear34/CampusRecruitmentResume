using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System;

namespace CampusRecruitmentResumeService.Controllers
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorController : ControllerBase
    {
        [Route("/error")]
        public IActionResult Error()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var exception = context?.Error;

            // 記錄錯誤到控制台
            Console.WriteLine($"Error: {exception?.Message}");
            Console.WriteLine($"StackTrace: {exception?.StackTrace}");

            return StatusCode(500, new
            {
                success = false,
                message = "系統錯誤",
                errors = new { general = new[] { "處理您的申請時發生錯誤，請稍後再試" } }
            });
        }
    }
}
