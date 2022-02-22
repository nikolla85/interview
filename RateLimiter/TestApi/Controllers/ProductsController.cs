using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace TestApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {

        [HttpGet("books")]
        public List<string> GetBooks()
        {
            return new List<string> { "B1", "B2", "B3" };
        }

        [HttpGet("pencils")]
        public List<string> GetPencils()
        {
            return new List<string> { "PE1", "PE2", "PE3" };
        }

        [HttpGet("photos")]
        public List<string> GetPhotos()
        {
            return new List<string> { "PH1", "PH2", "PH3" };
        }
    }
}
