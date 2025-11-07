using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PlanStack.Backend.Database;
using PlanStack.Backend.Database.Core;
using PlanStack.Backend.Database.DataModels;
using PlanStack.Backend.Database.Repositories;
using PlanStack.Backend.WebAPI.Handlers;
using PlanStack.Backend.WebAPI.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Adding CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowDevelopment", builder => builder.WithOrigins("http://localhost:5173").AllowAnyMethod().AllowAnyHeader().AllowCredentials());
    options.AddPolicy("AllowProduction", builder => builder.WithOrigins("https://planstack.dk").AllowAnyMethod().AllowAnyHeader().AllowCredentials());
});

//Adding Connection
builder.Services.AddDbContext<DatabaseContext>(
       o => o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//Adding JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["validIssuer"],
        ValidAudience = jwtSettings["validAudience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
            .GetBytes(jwtSettings.GetSection("securityKey").Value))
    };
});

// Adding UnitOfWork
builder.Services.AddScoped<UnitOfWork>();

// Adding AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// Adding Handlers
builder.Services.AddScoped<JwtHandler>();

// Adding Services
builder.Services.AddScoped<ImageService>();
builder.Services.AddScoped<BlueprintService>();

// Adding Repositories
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<BlueprintRepository>();
builder.Services.AddScoped<BlueprintBuildingStructureRepository>();
builder.Services.AddScoped<BlueprintComponentRepository>();
builder.Services.AddScoped<BuildingStructureRepository>();
builder.Services.AddScoped<ComponentRepository>();
builder.Services.AddScoped<ProjectRepository>();
builder.Services.AddScoped<RuleSetRepository>();
builder.Services.AddScoped<StandardRepository>();
builder.Services.AddScoped<StandardRuleSetRepository>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Adding Identity and Password Policy
// TODO: put these values in a configuration file
builder.Services.AddIdentity<User, IdentityRole>(opt =>
{
    opt.Password.RequiredLength = 7;
    opt.Password.RequireDigit = false;
    opt.Password.RequireUppercase = false;
    opt.Password.RequireNonAlphanumeric = false;

    opt.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<DatabaseContext>();

var app = builder.Build();

// Apply migrations in Live
if (app.Environment.IsProduction())
{
    var databaseContext = app.Services.GetRequiredService<DatabaseContext>();
    databaseContext.Database.SetCommandTimeout(0);
    databaseContext.Database.Migrate();
}

// Seed the database with admin user if it doesnt exist
await DatabaseInitializer.SeedAdminUserAsync(app.Services);
await DatabaseInitializer.SeedComponentsAsync(app.Services);

// Configure the HTTP request pipeline.
// Enable Swagger
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

// Enables Cors
app.UseCors("AllowDevelopment");
app.UseCors("AllowProduction");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
