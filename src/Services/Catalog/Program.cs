using NSE.Catalogo.API.Configuration;
using NSE.Services.Catalog.Configuration;
using NSE.WebAPI.Core.Identidade;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

//Extensions
builder.Services.AddSwaggerConfiguration();
builder.Services.AddRepositories();
builder.Services.AddJwtConfiguration(builder.Configuration);
builder.Services.AddApiConfiguration(builder.Configuration,builder.Environment);

var app = builder.Build();

app.UseSwaggerConfiguration();
app.UseApiConfiguration();
app.Run();
