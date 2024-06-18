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
    public class OrdersController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public OrdersController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("createUserOrder/{userId}")]
        public async Task<IActionResult> CreateUserOrder(int userId)
        {
            string insertQuery = @"
        INSERT INTO tblOrder (userId, total) 
        VALUES (@userId, 0)";
            DataTable table = new DataTable();

            string sqlDataSource = _configuration.GetConnectionString("project_intern");

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                await myCon.OpenAsync();
                using (SqlCommand myCommand = new SqlCommand(insertQuery, myCon))
                {
                    myCommand.Parameters.AddWithValue("@userId", userId);

                    using (SqlDataReader myReader = myCommand.ExecuteReader())
                    {
                        table.Load(myReader);
                    }

                }
                return Ok(new
                    {
                        status = 200,
                        message = "Order created successfully",
                        data = table
                });
            }
        }


        [HttpGet("getUserOrder/{userId}")]
        public async Task<IActionResult> GetUserOrder(int userId)
        {
            string selectQuery = @"
    SELECT i.id, i.name, od.size, od.quantity, od.price
    FROM tblOrderDetail od
    JOIN tblOrder o ON od.OrderId = o.id
    JOIN tblItem i ON od.ItemId = i.id
    WHERE o.UserId = @userId;";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("project_intern");

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                await myCon.OpenAsync();
                using (SqlCommand myCommand = new SqlCommand(selectQuery, myCon))
                {
                    myCommand.Parameters.AddWithValue("@userId", userId);
                    using (SqlDataReader myReader = await myCommand.ExecuteReaderAsync())
                    {
                        table.Load(myReader);
                    }
                }
            }

            return Ok(new
            {
                status = 200,
                message = "Order fetched successfully",
                data = table
            });
        }


        // API để kiểm tra xem item với kích thước đã tồn tại trong order chưa
        [HttpGet("user/{userId}/orderDetails")]
            public async Task<IActionResult> CheckItemInOrder(int userId, [FromQuery] int itemId, [FromQuery] string size)
            {
                string query = @"
                SELECT COUNT(*) FROM tblOrderDetail od
                INNER JOIN tblOrder o ON od.OrderId = o.id
                WHERE o.UserId = @UserId AND od.ItemId = @ItemId AND od.Size = @Size";

                DataTable table = new DataTable();
                string sqlDataSource = _configuration.GetConnectionString("project_intern");
                SqlDataReader myReader;

                using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                {
                    await myCon.OpenAsync();
                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        myCommand.Parameters.AddWithValue("@UserId", userId);
                        myCommand.Parameters.AddWithValue("@ItemId", itemId);
                        myCommand.Parameters.AddWithValue("@Size", size);
                        myReader = await myCommand.ExecuteReaderAsync();
                        table.Load(myReader);
                        myReader.Close();
                    }
                    await myCon.CloseAsync();
                }

                bool exists = table.Rows.Count > 0 && (int)table.Rows[0][0] > 0;

                return Ok(new
                {
                    exists
                });
            }

        [HttpPut("user/{userId}/orderDetails")]
        public async Task<IActionResult> UpdateItemQuantity(int userId, [FromBody] UpdateOrderDetailRequest request)
        {
            string updateQuery = @"
UPDATE od
SET od.Quantity = od.Quantity + @Quantity, od.Price = od.Price + @Price
FROM tblOrderDetail od
INNER JOIN tblOrder o ON od.OrderId = o.id
WHERE o.UserId = @UserId AND od.ItemId = @ItemId AND od.Size = @Size";

            if (request.Action == "decrease")
            {
                updateQuery = @"
UPDATE od
SET od.Quantity = od.Quantity - @Quantity, od.Price = od.Price - @Price
FROM tblOrderDetail od
INNER JOIN tblOrder o ON od.OrderId = o.id
WHERE o.UserId = @UserId AND od.ItemId = @ItemId AND od.Size = @Size";
            }

            string selectQuery = @"
SELECT od.ItemId, od.Quantity, od.Size, od.Price
FROM tblOrderDetail od
INNER JOIN tblOrder o ON od.OrderId = o.id
WHERE o.UserId = @UserId AND od.ItemId = @ItemId AND od.Size = @Size";

            DataTable updatedTable = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("project_intern");
            SqlDataReader myReader;

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                await myCon.OpenAsync();
                using (SqlCommand myCommand = new SqlCommand(updateQuery, myCon))
                {
                    myCommand.Parameters.AddWithValue("@UserId", userId);
                    myCommand.Parameters.AddWithValue("@ItemId", request.ItemId);
                    myCommand.Parameters.AddWithValue("@Size", request.Size);
                    myCommand.Parameters.AddWithValue("@Quantity", request.Quantity);
                    myCommand.Parameters.AddWithValue("@Price", request.Price);

                    myReader = await myCommand.ExecuteReaderAsync();
                    myReader.Close();
                }

                using (SqlCommand myCommand = new SqlCommand(selectQuery, myCon))
                {
                    myCommand.Parameters.AddWithValue("@UserId", userId);
                    myCommand.Parameters.AddWithValue("@ItemId", request.ItemId);
                    myCommand.Parameters.AddWithValue("@Size", request.Size);

                    myReader = await myCommand.ExecuteReaderAsync();
                    updatedTable.Load(myReader);
                    myReader.Close();
                }

                await myCon.CloseAsync();
            }

            return Ok(new
            {
                status = 200,
                message = "Item quantity updated successfully",
                data = updatedTable
            });
        }



        [HttpPost("user/{userId}/addItem")]
            public async Task<IActionResult> AddItemToOrder(int userId, [FromBody] OrderDetail orderDetail)
            {
                string insertQuery = @"
                INSERT INTO tblOrderDetail (OrderId, ItemId, Quantity, Size, Price)
                VALUES (@OrderId, @ItemId, @Quantity, @Size, @Price)";

                string getOrderQuery = @"
                SELECT TOP 1 id
                FROM tblOrder
                WHERE UserId = @UserId
                ORDER BY id DESC";

                DataTable table = new DataTable();
                string sqlDataSource = _configuration.GetConnectionString("project_intern");
                SqlDataReader myReader;

                int orderId = 0;

                using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                {
                    await myCon.OpenAsync();
                    using (SqlCommand myCommand = new SqlCommand(getOrderQuery, myCon))
                    {
                        myCommand.Parameters.AddWithValue("@UserId", userId);
                        myReader = await myCommand.ExecuteReaderAsync();
                        if (myReader.Read())
                        {
                            orderId = myReader.GetInt32(0);
                        }
                        myReader.Close();
                    }
                    await myCon.CloseAsync();
                }

                if (orderId == 0)
                {
                    return NotFound(new
                    {
                        status = 404,
                        message = "Order not found for the user"
                    });
                }

                using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                {
                    await myCon.OpenAsync();
                    using (SqlCommand myCommand = new SqlCommand(insertQuery, myCon))
                    {
                        myCommand.Parameters.AddWithValue("@OrderId", orderId);
                        myCommand.Parameters.AddWithValue("@ItemId", orderDetail.ItemId);
                        myCommand.Parameters.AddWithValue("@Quantity", orderDetail.Quantity);
                        myCommand.Parameters.AddWithValue("@Size", orderDetail.Size);
                        myCommand.Parameters.AddWithValue("@Price", orderDetail.Price);
                        myReader = await myCommand.ExecuteReaderAsync();
                        table.Load(myReader);
                        myReader.Close();
                    }
                    await myCon.CloseAsync();
                }

                return Ok(new
                {
                    status = 200,
                    message = "Item added to cart successfully",
                    data = orderDetail
                });
            }

        [HttpDelete("user/{userId}/orderDetails/{itemId}/{size}")]
        public async Task<IActionResult> DeleteItemFromOrder(int userId, int itemId, string size)
        {
            string deleteQuery = @"
    DELETE od
    FROM tblOrderDetail od
    INNER JOIN tblOrder o ON od.OrderId = o.id
    WHERE o.UserId = @UserId AND od.ItemId = @ItemId AND od.Size = @Size";

            string sqlDataSource = _configuration.GetConnectionString("project_intern");

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                await myCon.OpenAsync();
                using (SqlCommand myCommand = new SqlCommand(deleteQuery, myCon))
                {
                    myCommand.Parameters.AddWithValue("@UserId", userId);
                    myCommand.Parameters.AddWithValue("@ItemId", itemId);
                    myCommand.Parameters.AddWithValue("@Size", size);

                    int rowsAffected = await myCommand.ExecuteNonQueryAsync();

                    if (rowsAffected > 0)
                    {
                        return Ok(new
                        {
                            status = 200,
                            message = "Item deleted successfully"
                        });
                    }
                    else
                    {
                        return NotFound(new
                        {
                            status = 404,
                            message = "Item not found"
                        });
                    }
                }
            }

        }
        [HttpDelete("user/{userId}/orderDetails")]
        public async Task<IActionResult> DeleteAllItemsFromOrder(int userId)
        {
            string deleteQuery = @"
    DELETE od
    FROM tblOrderDetail od
    INNER JOIN tblOrder o ON od.OrderId = o.id
    WHERE o.UserId = @UserId";

            string sqlDataSource = _configuration.GetConnectionString("project_intern");

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                await myCon.OpenAsync();
                using (SqlCommand myCommand = new SqlCommand(deleteQuery, myCon))
                {
                    myCommand.Parameters.AddWithValue("@UserId", userId);

                    int rowsAffected = await myCommand.ExecuteNonQueryAsync();

                    if (rowsAffected > 0)
                    {
                        return Ok(new
                        {
                            status = 200,
                            message = "All items deleted successfully"
                        });
                    }
                    else
                    {
                        return NotFound(new
                        {
                            status = 404,
                            message = "No items found for this user"
                        });
                    }
                }
            }
        }

    }

}
