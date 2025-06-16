using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Application.Dtos
{
    public class ClientUpdateDto
    {
        [Required]
        public int ClientId { get; set; }

        [Required(ErrorMessage = "Identification type is required.")]
        public string IdentificationType { get; set; }

        [Required(ErrorMessage = "Identification number is required.")]
        [StringLength(20, ErrorMessage = "Identification number cannot exceed 20 characters.")]
        [RegularExpression("^[0-9]+$", ErrorMessage = "Identification number must be numeric.")]
        public string IdentificationNumber { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [RegularExpression("^[a-zA-ZáéíóúÁÉÍÓÚñÑ ]+$", ErrorMessage = "First name must contain only letters and spaces.")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [RegularExpression("^[a-zA-ZáéíóúÁÉÍÓÚñÑ ]+$", ErrorMessage = "Last name must contain only letters and spaces.")]
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Phone is required.")]
        [RegularExpression("^[0-9]+$", ErrorMessage = "Phone must be numeric.")]
        [StringLength(15, ErrorMessage = "Phone cannot exceed 15 characters.")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
        public string Address { get; set; }
    }
}
