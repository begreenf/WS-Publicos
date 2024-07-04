using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System;

namespace MyWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExchangeRateController : ControllerBase
    {
        private readonly string _connectionString;

        public ExchangeRateController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpGet("{currencyCode}")]
        public IActionResult GetExchangeRate(string currencyCode)
        {
            decimal exchangeRate = 0;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT ExchangeRate FROM ExchangeRates WHERE CurrencyCode = @currencyCode";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@currencyCode", currencyCode);

                conn.Open();
                var result = cmd.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    exchangeRate = Convert.ToDecimal(result);
                }
                else
                {
                    return NotFound("Currency code not found");
                }
            }

            return Ok(exchangeRate);
        }
    }
}
