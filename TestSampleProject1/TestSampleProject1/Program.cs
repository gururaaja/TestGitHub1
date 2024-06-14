using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json.Serialization;
using TestSampleProject1.DAL;
using TestSampleProject1.Services;

var builder = WebApplication.CreateBuilder(args);


#region Register Database connection

builder.Services.AddDbContext<DBContext>(opts => opts.UseSqlServer(builder.Configuration["ConnectionStrings:DatabaseConnection"], opt => opt.CommandTimeout((int)TimeSpan.FromMinutes(10).TotalSeconds)));

#endregion

#region Add Services to the Container

builder.Services.AddTransient<ITest1, Test1>();
builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

#endregion

// Add services to the container.

builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
    options.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
