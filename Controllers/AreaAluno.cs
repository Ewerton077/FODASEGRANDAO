using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace backendconfigconecta.Controllers;

    [Route("[controller]")]
    public class AreaAluno : Controller
    {
        private readonly ILogger<AreaAluno> _logger;

        public AreaAluno(ILogger<AreaAluno> logger)
        {
            _logger = logger;
        }

        public IActionResult Areaaluno_index()
        {
            return View();
        }


    }
