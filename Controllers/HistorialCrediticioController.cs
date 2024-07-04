using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class HistorialCrediticioController : ControllerBase
{
    private readonly string _connectionString;

    public HistorialCrediticioController(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    [HttpGet("{companyRNC}")]
    public async Task<ActionResult<IEnumerable<object>>> GetHistorialCrediticio(string companyRNC)
    {
        var historialList = new List<object>();

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = "SELECT * FROM HistorialCrediticio WHERE CompanyRNC = @companyRNC";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@companyRNC", companyRNC);

            connection.Open();
            SqlDataReader reader = await command.ExecuteReaderAsync();

            while (reader.Read())
            {
                var historial = new
                {
                    Id = reader.GetInt32(0),
                    CompanyRNC = reader.GetString(1),
                    ConceptoDeuda = reader.GetString(2),
                    FechaDeuda = reader.GetString(3),
                    MontoTotalAdeudado = reader.GetDouble(4) // Cambiado de GetFloat a GetDouble
                };
                historialList.Add(historial);
            }
        }

        if (historialList.Count == 0)
        {
            return NotFound();
        }

        return Ok(historialList);
    }
}
