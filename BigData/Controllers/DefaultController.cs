
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

        public IActionResult GetAllCars()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string sqlQuery = "SELECT TOP 10 * FROM Plates"; 
                IEnumerable<Plates> cars = connection.Query<Plates>(sqlQuery);
                return View(cars);
            }
        }

    }
}
