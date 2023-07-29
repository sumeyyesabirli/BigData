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
            int totalCount = connection.ExecuteScalar<int?>(countQuery) ?? 0; // Default değeri 0 olarak ayarladık.
            return totalCount;
        }

        public IActionResult GetAllCars(int pageNumber = 1, int pageSize = 25, string searchText = "")
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                int offset = (pageNumber - 1) * pageSize;

                // Arama metnini kullanarak parametreli bir sorgu oluşturun
                string whereClause = string.IsNullOrEmpty(searchText)
                    ? "" // Eğer arama metni boş ise tüm verileri getir
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
                    OR CASETYPE LIKE @SearchText"; // Diğer alanlar burada eklenmeli

                string sqlQuery = $@"
                    SELECT * FROM Plates
                    {whereClause}
                    ORDER BY ID OFFSET {offset} ROWS FETCH NEXT {pageSize} ROWS ONLY";

                // Parametreli sorguyu kullanmak için parametreleri ekleyin
                IEnumerable<Plates> cars = connection.Query<Plates>(sqlQuery, new { SearchText = $"%{searchText}%" });

                ViewBag.PageNumber = pageNumber;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalCount = GetTotalCarCount(connection);

                return View(cars);
            }
        }
    }
}
