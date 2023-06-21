using NSE.Catalogo.API.Configuration;
using NSE.Services.Catalog.Configuration;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

//Extensions
builder.Services.AddSwaggerConfiguration();
builder.Services.AddRepositories();
builder.Services.AddApiConfiguration(builder.Configuration,builder.Environment);

var app = builder.Build();

app.UseSwaggerConfiguration();
app.UseApiConfiguration();
app.Run();
