using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Application.Dtos
{
    public class InvoiceUpdateDto
    {
        public int InvoiceId { get; set; }
        public string InvoiceNumber { get; set; }
        public int ClientId { get; set; }
        public string UserId { get; set; }
        public DateTime IssueDate { get; set; }
        public string Observations { get; set; }
        public List<InvoiceDetailUpdateDto> InvoiceDetails { get; set; }
    }
}
