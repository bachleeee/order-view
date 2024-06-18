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
    public class ItemsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public ItemsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public JsonResult Get(int page = 1, int itemsPerPage = 10)
        {
            string query = @"
    SELECT * 
    FROM tblItem
    ORDER BY Id
    OFFSET @Offset ROWS 
    FETCH NEXT @ItemsPerPage ROWS ONLY";

            string countQuery = "SELECT COUNT(*) FROM tblItem";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("project_intern");
            int totalRecords = 0;

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    int offset = (page - 1) * itemsPerPage;
                    myCommand.Parameters.AddWithValue("@Offset", offset);
                    myCommand.Parameters.AddWithValue("@ItemsPerPage", itemsPerPage);

                    using (SqlDataReader myReader = myCommand.ExecuteReader())
                    {
                        table.Load(myReader);
                    }
                }

                using (SqlCommand countCommand = new SqlCommand(countQuery, myCon))
                {
                    totalRecords = (int)countCommand.ExecuteScalar();
                }
            }

            int totalPages = (int)Math.Ceiling((double)totalRecords / itemsPerPage);

            var response = new
            {
                status = 200,
                message = "Items retrieved successfully",
                data = new
                {
                    currentPage = page,
                    itemsPerPage = itemsPerPage,
                    totalPages = totalPages,
                    totalRecords = totalRecords,
                    items = table
                }
            };

            return new JsonResult(response);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            string query = @"
                SELECT * FROM tblItem 
                WHERE id = @id";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("project_intern");
            SqlDataReader myReader;

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@id", id);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            if (table.Rows.Count == 1)
            {
                var item = new
                {
                    id = table.Rows[0]["id"].ToString(),
                    name = table.Rows[0]["name"].ToString(),
                    description = table.Rows[0]["description"].ToString(),
                    image_url = table.Rows[0]["image_url"].ToString(),
                    price_s = table.Rows[0]["price_s"].ToString(),
                    price_m = table.Rows[0]["price_m"].ToString(),
                    price_l = table.Rows[0]["price_l"].ToString()
                };
                return Ok(new
                {
                    status = 200,
                    message = "Item retrieved successfully",
                    data = item
                });
            }
            else
            {
                return NotFound(new { Message = "Item not found" });
            }
        }

        [HttpPost("search")]
        public JsonResult Search([FromBody] SearchRequest request, int page = 1, int itemsPerPage = 9)
        {
            string query = @"
    SELECT tblItem.*
    FROM tblItem
    INNER JOIN tblCategory ON tblItem.category_id = tblCategory.id
    WHERE 
        (@Name = '' OR tblItem.Name LIKE '%' + @Name + '%') AND
        (@Category = '' OR tblCategory.Name = @Category) AND
        (@PriceMin IS NULL OR tblItem.price_m >= @PriceMin) AND
        (@PriceMax IS NULL OR tblItem.price_m <= @PriceMax)
    ORDER BY tblItem.Id
    OFFSET @Offset ROWS 
    FETCH NEXT @ItemsPerPage ROWS ONLY";

            string countQuery = @"
    SELECT COUNT(*)
    FROM tblItem
    INNER JOIN tblCategory ON tblItem.category_id = tblCategory.id
    WHERE 
        (@Name = '' OR tblItem.Name LIKE '%' + @Name + '%') AND
        (@Category = '' OR tblCategory.Name = @Category) AND
        (@PriceMin IS NULL OR tblItem.price_m >= @PriceMin) AND
        (@PriceMax IS NULL OR tblItem.price_m <= @PriceMax)";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("project_intern");
            int totalRecords = 0;

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@Name", request.Name ?? string.Empty);
                    myCommand.Parameters.AddWithValue("@Category", request.Category ?? string.Empty);
                    myCommand.Parameters.AddWithValue("@PriceMin", request.PriceMin.HasValue ? request.PriceMin.Value : (object)DBNull.Value);
                    myCommand.Parameters.AddWithValue("@PriceMax", request.PriceMax.HasValue ? request.PriceMax.Value : (object)DBNull.Value);

                    int offset = (page - 1) * itemsPerPage;
                    myCommand.Parameters.AddWithValue("@Offset", offset);
                    myCommand.Parameters.AddWithValue("@ItemsPerPage", itemsPerPage);

                    using (SqlDataReader myReader = myCommand.ExecuteReader())
                    {
                        table.Load(myReader);
                    }
                }

                using (SqlCommand countCommand = new SqlCommand(countQuery, myCon))
                {
                    countCommand.Parameters.AddWithValue("@Name", request.Name ?? string.Empty);
                    countCommand.Parameters.AddWithValue("@Category", request.Category ?? string.Empty);
                    countCommand.Parameters.AddWithValue("@PriceMin", request.PriceMin.HasValue ? request.PriceMin.Value : (object)DBNull.Value);
                    countCommand.Parameters.AddWithValue("@PriceMax", request.PriceMax.HasValue ? request.PriceMax.Value : (object)DBNull.Value);

                    totalRecords = (int)countCommand.ExecuteScalar();
                }

                int totalPages = (int)Math.Ceiling((double)totalRecords / itemsPerPage);

                var response = new
                {
                    status = 200,
                    message = "Items retrieved successfully",
                    data = new
                    {
                        currentPage = page,
                        itemsPerPage = itemsPerPage,
                        totalPages = totalPages,
                        totalRecords = totalRecords,
                        items = table
                    }
                };

                return new JsonResult(response);
            }
        }


    }
}
