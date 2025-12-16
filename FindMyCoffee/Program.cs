using FindMyCoffee.Data;
using FindMyCoffee.Data.API;
using FindMyCoffee.Data.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;
using static System.Net.WebRequestMethods;


var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json"); // Read your API key

Console.WriteLine(new String('-', 40));

Console.WriteLine("Here");
Console.WriteLine(new String('-', 40));

// Add services to the container.



// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
//builder.Services.AddScoped<IFindMyCoffeeRepo, MockShopRepo>();
builder.Services.AddScoped<ICoffeeShopRepository, CoffeeShopRepository>();
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
builder.Services.AddScoped<IUserManager, UserManager>();
/* Add HttpClient */
builder.Services.AddHttpClient();



//Cookie authentication:
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "auth-cookie"; 
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.None;  //During development, the app runs over HTTP and not HTTPS so chrome blocks it. By placing "None" instead of "Always", chrome lets the http call to pass to the frontend.
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromHours(24);
        //options.Cookie.IsEssential = true;
    });





var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi().AllowAnonymous(); // allow anonymous access to /openapi endpoints

    // Enable Swagger UI
    app.UseSwagger();
    app.UseSwaggerUI();
}




app.UseHttpsRedirection();  // If anybody tries to access the app from a non-secured port, it will force him to go to the SSL port



app.UseRouting();           // Right now not in use

//Cookie authentication
app.UseAuthentication();    // Turns on the ability to have users log-in and log-out. -> The act it self of login-in\out
app.UseAuthorization();     // When log-in and log-out, we secure the resources based on who they are. -> What you can do
app.MapControllers();

app.Run();