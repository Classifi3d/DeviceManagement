using Application.Abstraction;
using Infrastructure.Persistence.Repository;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions;
public static class DatabaseExtension
{
    public static void AddUnitOfWork(this IServiceCollection services)
    {
        services.AddScoped<SqlConnection>(sp =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            var connString = config.GetConnectionString("SQLServer_Connection_String");

            var connection = new SqlConnection(connString);
            connection.Open(); 

            return connection;
        });

        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }
}
