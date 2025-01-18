using Microsoft.EntityFrameworkCore;
using MusteriTakipSistemi.Context;
using MusteriTakipSistemi.Models;

namespace MusteriTakipSistemi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();

            // Add DbContext for Entity Framework Core
            builder.Services.AddDbContext<VeritabaniContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Configure Swagger for API Documentation
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddCors(options =>
     options.AddDefaultPolicy(builder =>
     builder.AllowAnyHeader().AllowAnyMethod()
                .SetIsOriginAllowed(host => true).WithExposedHeaders("result-count")));


            var app = builder.Build();

            // Seed initial data for Admins, Customers, and Calls
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<VeritabaniContext>();

                // Admins data
                if (!context.Admins.Any())
                {
                    var admins = new List<Admin>
                    {
                        new Admin { Name = "Admin1", Email = "admin1@example.com", Password = "1234" },
                        new Admin { Name = "Admin2", Email = "admin2@example.com", Password = "1234" },
                        new Admin { Name = "Admin3", Email = "admin3@example.com", Password = "1234" },
                        new Admin { Name = "Admin4", Email = "admin4@example.com", Password = "1234" },
                        new Admin { Name = "Admin5", Email = "admin5@example.com", Password = "1234" }
                    };

                    context.Admins.AddRange(admins);
                    context.SaveChanges();
                }

                // Customers data
                if (!context.Customers.Any())
                {
                    var customers = new List<Customer>
                    {
                        new Customer { Name = "Customer1", PhoneNumber = "1234567890",  CallCount = 2, TotalStatus = 7.5f / 2 },
                        new Customer { Name = "Customer2", PhoneNumber = "2345678901",  CallCount = 3, TotalStatus = 9 / 3 },
                        new Customer { Name = "Customer3", PhoneNumber = "3456789012",  CallCount = 1, TotalStatus = 10 / 1 },
                        new Customer { Name = "Customer4", PhoneNumber = "4567890123",  CallCount = 4, TotalStatus = 15.6f / 4 },
                        new Customer { Name = "Customer5", PhoneNumber = "5678901234",  CallCount = 2, TotalStatus = 7.2f / 2 },
                    };

                    context.Customers.AddRange(customers);
                    context.SaveChanges();
                }

                // Calls data
                if (!context.Calls.Any())
                {
                    var calls = new List<Call>
                    {
                        new Call { CustomerId = 1, AdminId = 1, CallDate = DateTime.Now.AddDays(-10), Notes = "Inquiry about product", Status = 3.5f },
                        new Call { CustomerId = 1, AdminId = 2, CallDate = DateTime.Now.AddDays(-5), Notes = "Complaint resolution", Status = 4.0f },
                        new Call { CustomerId = 2, AdminId = 3, CallDate = DateTime.Now.AddDays(-7), Notes = "Service update request", Status = 2.5f },
                        new Call { CustomerId = 2, AdminId = 4, CallDate = DateTime.Now.AddDays(-3), Notes = "Feedback on service", Status = 5.0f },
                        new Call { CustomerId = 2, AdminId = 5, CallDate = DateTime.Now.AddDays(-15), Notes = "Order issue", Status = 1.5f },
                        new Call { CustomerId = 3, AdminId = 1, CallDate = DateTime.Now.AddDays(-2), Notes = "New product inquiry", Status = 10 },
                        new Call { CustomerId = 4, AdminId = 2, CallDate = DateTime.Now.AddDays(-8), Notes = "Upgrade request", Status = 4.8f },
                        new Call { CustomerId = 4, AdminId = 3, CallDate = DateTime.Now.AddDays(-1), Notes = "Contract renewal", Status = 3.6f },
                        new Call { CustomerId = 4, AdminId = 4, CallDate = DateTime.Now.AddDays(-12), Notes = "Warranty question", Status = 2.9f },
                        new Call { CustomerId = 4, AdminId = 5, CallDate = DateTime.Now.AddDays(-9), Notes = "Payment inquiry", Status = 4.3f },
                        new Call { CustomerId = 5, AdminId = 4, CallDate = DateTime.Now.AddDays(-12), Notes = "Warranty question", Status = 2.9f },
                        new Call { CustomerId = 5, AdminId = 5, CallDate = DateTime.Now.AddDays(-9), Notes = "Payment inquiry", Status = 4.3f }
                    };

                    context.Calls.AddRange(calls);
                    context.SaveChanges();
                }
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.UseCors(builder =>
            {
                builder.AllowAnyHeader()
                       .AllowAnyMethod()
                       .AllowCredentials()
                       .SetIsOriginAllowed((host) => true); // T?m origin'lere izin ver
            });


            app.Run();
        }
    }
}
