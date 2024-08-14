using Grpc.Core;
using GrpcService1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Reflection;

namespace GrpcService1.Services
{
    [Authorize]
    public class BillingPeriodService : billingPeriod.billingPeriodBase
    {
        private readonly ILogger<BillingPeriodService> _logger;
        public BillingPeriodService(ILogger<BillingPeriodService> logger)
        {
            _logger = logger;
        }

        public override async Task<FetchResult> GetData(StartDate request, ServerCallContext context)
        {
            string? userName = context.GetHttpContext().User.Identity.Name;

            var builder = new ConfigurationBuilder()
          .SetBasePath(Directory.GetCurrentDirectory())
            .AddUserSecrets(Assembly.GetExecutingAssembly(), optional: true, reloadOnChange: false);
            IConfiguration config = builder.Build();

            string connectionStringCWMan = config.GetConnectionString("sqlConnStr");

            SqlConnection sqlConnectionCWMan = new SqlConnection(connectionStringCWMan);
            String strSQL = " SELECT * " +
                           " FROM AppUser ";

            SqlCommand myCommand = new SqlCommand(strSQL, sqlConnectionCWMan);
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = sqlConnectionCWMan)
            {
                await sqlConnection.OpenAsync();
                using (SqlDataReader reader = await myCommand.ExecuteReaderAsync())
                {
                    dt.Load(reader);
                }
            }
            string name = (string)dt.Rows[0]["name"];

            JObject obj = new JObject();
            if (request.Date == "2024-01-01")
            {
                obj.Add("2024-01-01", "100");
                obj.Add("id", name);
                return await Task.FromResult(new FetchResult
                {
                    Result = obj.ToString()
                });
            }
            if (request.Date == "2024-01-02")
            {
                obj.Add("2024-01-01", "200");
                obj.Add("id", "200");
                return await Task.FromResult(new FetchResult
                {
                    Result = obj.ToString()
                });
            }
            if (request.Date == "2024-01-03")
            {
                obj.Add("2024-01-01", "500");
                obj.Add("id", "500");
                return await Task.FromResult(new FetchResult
                {
                    Result = obj.ToString()
                });
            }
            else
            {
                obj.Add("id", "-1");
                return await Task.FromResult(new FetchResult
                {
                    Result = obj.ToString()
                });
            }
        }
    }
}