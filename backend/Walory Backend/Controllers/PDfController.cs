using Application.CQRS.PDF;
using Cars.API.Controllers;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Walory_Backend.Controllers
{
    [ApiController]
    [Route("api/pdf")]
    public class PdfController : BaseApiController
    {


        [HttpGet("export-pdf")]
        [Authorize]
        public async Task<IActionResult> ExportPdf([FromQuery] string? category)
        {

            var pdfBytes = await Mediator.Send(new ExportCollectionsToPdf.Query
            {
                Category = category
            });

            return File(pdfBytes, "application/pdf", "collections_export.pdf");
        }

        [HttpGet("categories")]
        [Authorize]
        public async Task<IActionResult> Categories()
        {

            var query = await Mediator.Send(new GetCategoryList.Query());
            return Ok(query);

        }
    }

}
