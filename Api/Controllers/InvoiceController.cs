using Microsoft.AspNetCore.Mvc;
using Project.Application.Dtos;

using Project.Application.Services;
using System.Threading.Tasks;

namespace Project.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceServices _invoiceService;

        public InvoiceController(IInvoiceServices invoiceService)
        {
            _invoiceService = invoiceService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var invoice = await _invoiceService.GetByIdAsync(id);
            if (invoice == null) return NotFound();
            return Ok(invoice);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int pageNumber = 1, int pageSize = 10, string? searchTerm = null)
        {
            var invoices = await _invoiceService.GetAllAsync(pageNumber, pageSize, searchTerm);
            return Ok(invoices);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] InvoiceCreateDto invoiceDto)
        {
            await _invoiceService.AddAsync(invoiceDto);
            // Idealmente, deberías retornar el ID generado o el objeto completo
            return StatusCode(201);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] InvoiceUpdateDto invoiceDto)
        {
            if (id != invoiceDto.InvoiceId) return BadRequest();
            await _invoiceService.UpdateAsync(invoiceDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _invoiceService.DeleteAsync(id);
            return NoContent();
        }

        // Métodos para detalles de factura
        [HttpGet("{invoiceId}/details")]
        public async Task<IActionResult> GetDetails(int invoiceId)
        {
            var details = await _invoiceService.GetInvoiceDetailsAsync(invoiceId);
            return Ok(details);
        }

        [HttpPost("{invoiceId}/add-product")]
        public async Task<IActionResult> AddProduct(int invoiceId, [FromBody] InvoiceDetailCreateDto detailDto)
        {
            await _invoiceService.AddProductToInvoiceAsync(invoiceId, detailDto.ProductId, detailDto.Quantity);
            return NoContent();
        }

        [HttpDelete("{invoiceId}/remove-product/{productId}")]
        public async Task<IActionResult> RemoveProduct(int invoiceId, int productId)
        {
            await _invoiceService.RemoveProductFromInvoiceAsync(invoiceId, productId);
            return NoContent();
        }
    }
}