using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Application.Dtos
{
    public class InvoiceCreateDto
    {
        
        public string InvoiceNumber { get; set; }
        public int ClientId { get; set; }
        public string UserId { get; set; }
        public DateTime? IssueDate { get; set; } // Opcional, el backend puede asignar la fecha actual si es null
        public string Observations { get; set; }
        public List<InvoiceDetailCreateDto> InvoiceDetails { get; set; }
    }
}
