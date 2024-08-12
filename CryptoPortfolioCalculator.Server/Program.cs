
using CryptoPortfolioCalculator.Server.Filters;
using CryptoPortfolioCalculator.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace CryptoPortfolioCalculator.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            Log.Logger = new LoggerConfiguration()
               .WriteTo.Console()
               .WriteTo.File("Logs/portfolio-operations.log", rollingInterval: RollingInterval.Day)
               .CreateLogger();

            try
            {
                builder.Host.UseSerilog();

                Log.Information("Starting web application");
                builder.Services.AddControllers(options =>
                {
                    options.Filters.Add<ExceptionHandlingMiddleware>();
                    options.Filters.Add(new ProducesAttribute("application/json"));
                });
                // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();
                builder.Services.AddHttpClient();

                builder.Services.AddScoped<IPortfolioFileService, PortfolioFileService>();

                builder.Services.AddCors(options =>
                {
                    options.AddPolicy("AllowAngularApp", policy =>
                    {
                        policy.WithOrigins("https://localhost:4200")
                              .AllowAnyMethod()
                              .AllowAnyHeader();
                    });
                });

                var app = builder.Build();

                app.UseCors("AllowAngularApp");
                app.UseDefaultFiles();
                app.UseStaticFiles();

                // Configure the HTTP request pipeline.
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                app.UseHttpsRedirection();

                app.UseAuthorization();


                app.MapControllers();

                app.MapFallbackToFile("/index.html");

                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}