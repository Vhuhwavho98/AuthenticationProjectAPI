using AuthenticationProjectAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//Services for Identity

builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AuthenticationConnectionString")));


builder.Services.Configure<IdentityOptions>(options =>
{
   //Password requirements
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
    options.Password.RequiredUniqueChars = 1;
}

); 
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


app.
    MapGroup("/api").
    MapIdentityApi<IdentityUser>();

app.MapPost("/api/signup", async (UserManager<ApplicationUser> userManager, [FromBody] UserRegistrationModel userRegistrationModel) =>
{
    ApplicationUser applicationUser = new ApplicationUser
    {
        Email = userRegistrationModel.Email,
        UserName = userRegistrationModel.Email,
        FullName = userRegistrationModel.FullName
    };

    var result = await userManager.CreateAsync(applicationUser, userRegistrationModel.Password);

    if (result.Succeeded)
        return Results.Ok(result);
    else
        return Results.BadRequest(result.Errors);
});

app.Run();



public class UserRegistrationModel
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string FullName { get; set; }
}