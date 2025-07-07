using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Application.Dtos
{
    public class UserDto
    {
        public string Id { get; set; }
        public string IdentificationNumber { get; set; } // For "cédula"
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string UserName { get; set; }
        public IList<string> Roles { get; set; }
        public bool IsLocked { get; set; }
        public class UserCreateDto
        {
            public string IdentificationNumber { get; set; }
            public string Email { get; set; }
            public string UserName { get; set; }
            public string Password { get; set; }
            public IList<string> Roles { get; set; }
        }
        public class UserUpdateDto
        {
            public string Id { get; set; }
            public string IdentificationNumber { get; set; }
            public string Email { get; set; }
            public string UserName { get; set; }
            public string Password { get; set; }
            public bool EmailConfirmed { get; set; }
            public IList<string> Roles { get; set; }
            public bool IsLocked { get; set; }
        }
    }
}
