using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace DndCompanion.Models
{
    public class UserModel : IdentityUser
    {
        public string? PublicUserName { get; set; }
    }
}