
using BigData.DAL.Entities;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Numerics;

namespace BigData.Controllers
{
    public class DefaultController : Controller
    {
        private readonly string _connectionString;

        public DefaultController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public IActionResult GetAllCars(int pageNumber = 1, int pageSize = 50)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                int offset = (pageNumber - 1) * pageSize;
                string sqlQuery = $"SELECT * FROM Plates ORDER BY ID OFFSET {offset} ROWS FETCH NEXT {pageSize} ROWS ONLY";
                IEnumerable<Plates> cars = connection.Query<Plates>(sqlQuery);

                ViewBag.PageNumber = pageNumber;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalCount = GetTotalCarCount(connection); // Toplam veri sayısını alır.

                return View(cars);
            }
        }

        private int GetTotalCarCount(SqlConnection connection)
        {
            string countQuery = "SELECT COUNT(*) FROM Plates";
            return connection.ExecuteScalar<int>(countQuery);
        }
    }
}
