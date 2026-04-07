using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);

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


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();


app.Run();
