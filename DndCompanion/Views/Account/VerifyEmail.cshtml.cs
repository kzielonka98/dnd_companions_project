using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace DndCompanion.Views.Account
{
    public class VerifyEmail : PageModel
    {
        private readonly ILogger<VerifyEmail> _logger;

        public VerifyEmail(ILogger<VerifyEmail> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}