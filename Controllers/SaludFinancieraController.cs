using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class SaludFinancieraController : ControllerBase
{
    private readonly string _connectionString;

    public SaludFinancieraController(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    [HttpGet("{cedulaOrRNC}")]
    public async Task<IActionResult> GetFinancialHealth(string cedulaOrRNC)
    {
        if (string.IsNullOrWhiteSpace(cedulaOrRNC))
        {
            return BadRequest("El valor de Cedula o RNC no puede estar vacío.");
        }

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            string query = "SELECT Indicador, Comentario, MontoTotalAdeudado FROM dbo.SaludFinanciera WHERE cedulaOrRNC = @cedulaOrRNC";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@cedulaOrRNC", cedulaOrRNC);

                using (SqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        var financialHealth = new
                        {
                            Indicador = reader["Indicador"].ToString(),
                            Comentario = reader["Comentario"].ToString(),
                            MontoTotalAdeudado = Convert.ToDouble(reader["MontoTotalAdeudado"])
                        };

                        return Ok(financialHealth);
                    }
                    else
                    {
                        return NotFound("No se encontró la salud financiera para el cliente proporcionado.");
                    }
                }
            }
        }
    }
}
