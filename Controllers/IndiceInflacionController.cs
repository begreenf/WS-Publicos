using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace MyWebApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IndiceInflacionController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public IndiceInflacionController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // GET api/IndiceInflacion/2023
        [HttpGet("{periodo}")]
        public IActionResult GetIndiceInflacion(string periodo)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            string query = "SELECT Inflacion FROM dbo.IndiceInflacion WHERE Periodo = @Periodo";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Periodo", periodo);
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    double inflacion = reader.GetDouble(0);
                    return Ok(new { Periodo = periodo, Inflacion = inflacion });
                }
                else
                {
                    return NotFound($"No se encontró índice de inflación para el período {periodo}");
                }
            }
        }
    }
}
