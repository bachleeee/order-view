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
    public class UsersController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public UsersController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public JsonResult Get(int page = 1, int itemsPerPage = 10)
        {
            string query = @"
    SELECT * 
    FROM tblUser
    ORDER BY Id
    OFFSET @Offset ROWS 
    FETCH NEXT @ItemsPerPage ROWS ONLY";

            string countQuery = "SELECT COUNT(*) FROM tblUser";

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
                message = "Users retrieved successfully",
                data = new
                {
                    currentPage = page,
                    itemsPerPage = itemsPerPage,
                    totalPages = totalPages,
                    totalRecords = totalRecords,
                    users = table
                }
            };

            return new JsonResult(response);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            string query = @"
                SELECT * FROM tblUser 
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
                var user = new
                {
                    name = table.Rows[0]["Name"].ToString(),
                    email = table.Rows[0]["Email"].ToString(),
                    address = table.Rows[0]["Address"].ToString(),
                    phone = table.Rows[0]["Phone"].ToString(),
                };
                return Ok(new
                {
                    status = 200,
                    message = "User retrieved successfully",
                    data = user
                });
            }
            else
            {
                return Unauthorized(new { Message = "User not found" });
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, [FromBody] UpdateUserModel userModel)
        {
            string query = @"
        UPDATE tblUser 
        SET Name = @Name, Phone = @Phone, Address = @Address, Email = @Email 
        WHERE Id = @Id";

            try
            {
                string sqlDataSource = _configuration.GetConnectionString("project_intern");
                using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                {
                    myCon.Open();
                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        myCommand.Parameters.AddWithValue("@Id", id); 
                        myCommand.Parameters.AddWithValue("@Name", userModel.Name);
                        myCommand.Parameters.AddWithValue("@Phone", userModel.Phone);
                        myCommand.Parameters.AddWithValue("@Address", userModel.Address);
                        myCommand.Parameters.AddWithValue("@Email", userModel.Email);

                        int rowsAffected = myCommand.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            return Ok(new { status = 200, message = "User updated successfully" });
                        }
                        else
                        {
                            return NotFound(new { status = 404, message = "User not found" });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = 500, message = ex.Message });
            }
        }
    }
}
