using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DocManagementAPI.Models;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace DocManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentsController : ControllerBase
    {
        private readonly DocumentContext _context;

        public DocumentsController(DocumentContext context)
        {
            _context = context;
        }

        [HttpGet]
        [EnableQuery()]
        public IEnumerable<dynamic> Get()
        {
            IEnumerable<Document> documents = _context.Documents.ToList<Document>();
            var baseurl = Request?.Host;

            //For display, removing  not required properties
            var documentsMod = documents.Select(x =>
               new 
               {
                   Name = x.Name,
                   Location = baseurl + x.Location,
                   Size = x.Size                  
               }).ToList<dynamic>();

           

            return documentsMod;
        }

        [HttpPost]
        public async Task<IActionResult> OnPostUploadAsync(IFormFile formFile)
        {
            if (formFile != null && formFile.Length > 0)
            {
                if (formFile.ContentType != "application/pdf")
                {
                    return BadRequest("Not a valid file type");
                }
                if (formFile.Length > 5 * 1024 * 1024)
                {
                    return BadRequest("File size is greater than 5 MB");
                }
               
                Document document = new Document();
                document.ID = Guid.NewGuid();
                document.Name = formFile.FileName;
                document.Location = "/api/documents/" + formFile.FileName;
                document.Size = (formFile.Length / (1024.00 * 1024)); //storing in mb
                using (var memoryStream = new MemoryStream())
                {
                    await formFile.CopyToAsync(memoryStream);
                    document.Content = memoryStream.ToArray();

                }
                _context.Documents.Add(document);
                _context.SaveChanges();
            }

            return Ok("Uploaded");
        }

        //GET api/documents/12345abc.pdf
        [HttpGet("{filename}")]
        public IActionResult DownloadFile(string filename)
        {
            try
            {
                Document document = _context.Documents.Where(x => x.Name == filename).FirstOrDefault<Document>();
                if (document == null)
                    return NotFound("File not found"); // returns a NotFoundResult with Status404NotFound response.
                Stream stream = new MemoryStream(document.Content);

              

                return new FileStreamResult(stream, "application/pdf")
                {
                    FileDownloadName = filename
                };
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);//logging
                return NotFound("File not found"); // returns a NotFoundResult with Status404NotFound response.
            }
        }

        //GET api/documents/12345abc.pdf
        [HttpDelete("{filename}")]
        public async Task<IActionResult> DeleteFileAsync(string filename)
        {
            try
            {
                Document document = _context.Documents.Where(x => x.Name == filename).FirstOrDefault<Document>();
                if (document != null)
                {
                    _context.Documents.Attach(document);
                }
                _context.Documents.Remove(document);
                await _context.SaveChangesAsync();

           
                return Ok("File deleted");
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);//logging
                return NotFound("File not found"); // returns a NotFoundResult with Status404NotFound response.
            }
        }
    }

}