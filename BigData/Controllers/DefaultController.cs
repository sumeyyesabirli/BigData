using BigData.DAL.Entities;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace BigData.Controllers
{
    public class DefaultController : Controller
    {
        private readonly string _connectionString;
        private readonly IConfiguration _configuration;

        public DefaultController(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        public IActionResult Index()
        {
            return View();
        }

        private int GetTotalCarCount(SqlConnection connection)
        {
            string countQuery = "SELECT COUNT(*) FROM Plates";
            int totalCount = connection.ExecuteScalar<int?>(countQuery) ?? 0; 
            return totalCount;
        }

        public IActionResult GetAllCars(int pageNumber = 1, int pageSize = 25, string searchText = "")
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                int offset = (pageNumber - 1) * pageSize;

                
                string whereClause = string.IsNullOrEmpty(searchText)
                    ? "" 
                    : @"WHERE PLATE LIKE @SearchText
                    OR LICENCEDATE LIKE @SearchText
                    OR TITLE LIKE @SearchText
                    OR BRAND LIKE @SearchText
                    OR MODEL LIKE @SearchText
                    OR YEAR_ LIKE @SearchText
                    OR FUEL LIKE @SearchText
                    OR SHIFTTYPE LIKE @SearchText
                    OR MOTORVOLUME LIKE @SearchText
                    OR MOTORPOWER LIKE @SearchText
                    OR COLOR LIKE @SearchText
                    OR CASETYPE LIKE @SearchText";

                string sqlQuery = $@"
                    SELECT * FROM Plates
                    {whereClause}
                    ORDER BY ID OFFSET {offset} ROWS FETCH NEXT {pageSize} ROWS ONLY";

                
                IEnumerable<Plates> cars = connection.Query<Plates>(sqlQuery, new { SearchText = $"%{searchText}%" });

                ViewBag.PageNumber = pageNumber;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalCount = GetTotalCarCount(connection);

                return View(cars);
            }
        }
    }
}
