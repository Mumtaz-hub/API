using System;
using System.Reflection;
using Common;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    public class UtilityController : ControllerBase
    {
        private readonly AppSettings settings;

        public UtilityController(AppSettings settings)
        {
            this.settings = settings;
        }

        [HttpGet]
        [Route("api/version")]
        public string GetVersion()
        {
            return $"Assembly - {Assembly.GetExecutingAssembly().GetName().Name} " +
                   $"{Environment.NewLine}Version - {Assembly.GetExecutingAssembly().GetName().Version}" +
                   $"{Environment.NewLine}Environment - {settings.Environment}";

        }
    }
}
