using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using internship.Models;
using Microsoft.Extensions.Configuration;

namespace internship.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public CategoryController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public JsonResult Get()
        {
            string query = @"
    SELECT * 
    FROM tblCategory
    ORDER BY Id;";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("project_intern");
            int totalRecords = 0;

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    using (SqlDataReader myReader = myCommand.ExecuteReader())
                    {
                        table.Load(myReader);
                    }
                }
            }


            var response = new
            {
                status = 200,
                message = "Category retrieved successfully",
                data = new
                {
                    category = table
                }
            };

            return new JsonResult(response);
        }



    }
}
