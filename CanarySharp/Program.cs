using CanarySharp.Endpoints;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddHttpClient();


var app = builder.Build();
app.MapOpenApi();
app.MapCanaryEndpoint();
app.UseSwaggerUI(opt =>
{
    opt.SwaggerEndpoint("/openapi/v1.json", "v1");
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
