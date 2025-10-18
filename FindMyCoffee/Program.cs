using FindMyCoffee.Data;
using FindMyCoffee.Data.API;
using FindMyCoffee.Domain;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;


var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json"); // Read your API key

Console.WriteLine(new String('-', 40));

Console.WriteLine("Here");
Console.WriteLine(new String('-', 40));

// Add services to the container.



builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
//builder.Services.AddScoped<IFindMyCoffeeRepo, MockShopRepo>();
builder.Services.AddScoped<IFindMyCoffeeRepo, SqlFindMyCoffeeRepo>();
builder.Services.AddTransient<GooglePlacesService>();
builder.Services.AddDbContext<FindMyCoffeeContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("CoffeeDbConnection")));
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<PullData>();
// Add Swagger service
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers().AddNewtonsoftJson(s =>
{
    s.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
});
builder.Services.AddScoped<IUserUniquenessChecker, UserUniquenessChecker>();
/* Add HttpClient */
builder.Services.AddHttpClient();

var app = builder.Build();      

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    // Enable Swagger UI
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
// Remove or fix the following lines to resolve CS0201 and CS0165 errors:

// IFindMyCoffeeRepo obj;
// obj.GetWebShopInfo;

// Example fix: Properly instantiate the object and call the method
// (This is just for demonstration; in a real application, you would get the implementation from DI)



Console.WriteLine("---------------Delete later---------------");
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var repo = services.GetRequiredService<IFindMyCoffeeRepo>();
        var webShopInfo = repo.GetWebShopInfo();
        Console.WriteLine(webShopInfo);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}
Console.WriteLine("---------------Delete later---------------");

app.MapControllers();
app.Run();





