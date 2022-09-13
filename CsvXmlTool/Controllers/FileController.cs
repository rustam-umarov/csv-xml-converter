using System;
using CsvXmlTool.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CsvXmlTool.Controllers
{
    [ApiController]
    [Route("file")]
    public class FileController : ControllerBase
    {
        private readonly IDataService _dataService;

        public FileController(IDataService dataService)
        {
            _dataService = dataService;
        }

        [HttpPost("import")]
        public IActionResult Import (IFormFile formFile)
        {
            Tuple<bool, string> res;

            if (formFile != null && formFile.ContentType.Contains("csv"))
            {
                res = _dataService.RecordCsvToDatabase(formFile);
            }
            else if(formFile != null && formFile.ContentType.Contains("officedocument.spreadsheetml.sheet"))
            {
                res = _dataService.RecordXlsxToDatabase(formFile);
            }
            else
            {
                return BadRequest("Invalid file format!");
            }


            if(res.Item1)
            {
                return Ok(res.Item2);
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, res.Item2);
            }

        }



        [HttpGet("export")]
        public IActionResult Export()
        {
            return Ok(_dataService.GetXmlFromDatabase());
        }
    }
}
