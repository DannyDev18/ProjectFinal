using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Application.Dtos
{
    public class RolDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class RoleCreateDto
    {
        [Required(ErrorMessage = "Role name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Role name must be between 2 and 50 characters")]
        public string Name { get; set; } = string.Empty;
   
        [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters")]
        public string? Description { get; set; }
        
        public bool IsActive { get; set; } = true;
    }

    public class RoleUpdateDto
    {
        [Required(ErrorMessage = "Role ID is required")]
        public string Id { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Role name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Role name must be between 2 and 50 characters")]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters")]
        public string? Description { get; set; }
        
        public bool IsActive { get; set; } = true;
    }

    public class RoleListDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
