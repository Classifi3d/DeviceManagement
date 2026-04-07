using Application.Extensions;
using Infrastructure.Extensions;
using Microsoft.OpenApi;
using Presentation.Configurations;
using Presentation.Extensions;
using Serilog;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog(LoggerConfigurator.ConfigureLogger(builder.Configuration));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder
    .Services
    .AddSwaggerGen(options =>
    {
        options.AddSecurityDefinition(
            "oauth2",
            new OpenApiSecurityScheme
            {
                Description =
                    "Standard Authorization Header Using The Bearer Scheme (\"bearer {token}\")",
                In = ParameterLocation.Header,
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey
            }
        );
        ;
        options.OperationFilter<SecurityRequirementsOperationFilter>();
    });


builder.Services.AddUnitOfWork();
builder.Services.AddApplicationServices();

builder.Services.AddEndpointRateLimiters();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseRateLimiter();
app.UseExceptionHandler();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();


app.Run();
