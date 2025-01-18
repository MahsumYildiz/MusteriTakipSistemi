using Microsoft.AspNetCore.Mvc;
using MusteriTakipSistemi.Context;
using MusteriTakipSistemi.Models;
using MusteriTakipSistemi.Models.dtos;
using System.Linq;

namespace MusteriTakipSistemi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly VeritabaniContext _context;

        public CustomerController(VeritabaniContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetCustomers()
        {
            var customers = _context.Customers
            .Select(c => new
            {
                c.Id,
                c.Name,
                c.PhoneNumber,
                c.CallCount,
                c.TotalStatus,
                PackageStatus = c.TotalStatus >= 10 ? "Paketi almıştır" : "Paketi almamıştır"
            })
            .ToList();

            return Ok(customers);
        }

        [HttpPost]
        public IActionResult AddCustomer([FromBody] CustomerInsertDto customerDto)
        {
            var customer = new Customer
            {
                Name = customerDto.Name,
                PhoneNumber = customerDto.PhoneNumber,
            };

            _context.Customers.Add(customer);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetCustomers), new { id = customer.Id }, customer);
        }

        [HttpPut]
        public IActionResult UpdateCustomer([FromBody] UpdateCustomerDto updatedCustomer)
        {
            var customer = _context.Customers.Where(x => x.Id == updatedCustomer.Id && x.TotalStatus < 10).FirstOrDefault();
            
            if (customer == null)
                return NotFound();

            customer.Name = updatedCustomer.Name;
            customer.PhoneNumber = updatedCustomer.PhoneNumber;
            customer.TotalStatus = updatedCustomer.TotalStatus;

            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCustomer(int id)
        {
            var customer = _context.Customers.Find(id);
            if (customer == null)
                return NotFound();

            _context.Customers.Remove(customer);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpPost("make-call")]
        public IActionResult MakeCall([FromQuery] int customerId, [FromQuery] int adminId, [FromBody] string notes, [FromQuery] float status)
        {
            var customer = _context.Customers.Where(x => x.Id == customerId && x.TotalStatus < 10).FirstOrDefault();

            if (customer == null)
                return NotFound("Müşteri yok veya paketi satın aldı");

            var call = new Call
            {
                CustomerId = customerId,
                AdminId = adminId,
                CallDate = DateTime.Now,
                Notes = notes,
                Status = status
            };

            _context.Calls.Add(call);
            customer.CallCount++;

            if (status >= 10)
            {
                customer.TotalStatus = 10;
                _context.SaveChanges();
                
                return Ok(new { message = "Çağrı başarıyla yapıldı.", customer });
            }

            customer.TotalStatus = CalculateTotalStatusAverage(customerId, call);

            _context.SaveChanges();

            return Ok(new { message = "Çağrı başarıyla yapıldı.", customer });
        }

        private float CalculateTotalStatusAverage(int customerId, Call newCall)
        {
            var calls = _context.Calls.Where(c => c.CustomerId == customerId).ToList();
            calls.Add(newCall);
            
            if (!calls.Any())
                return 0;

            var averageStatus = (float)calls.Average(c => c.Status); // double -> float dönüşümü
            return averageStatus;
        }

        [HttpGet("{customerId}/status-total")]
        public IActionResult GetCustomerStatusTotal(int customerId)
        {
            var customer = _context.Customers.Where(x => x.Id == customerId).FirstOrDefault();
            if (customer == null)
                return NotFound();

            return Ok(new { CustomerId = customerId, StatusTotal = customer.TotalStatus, Renk = CalculateColor(customer.TotalStatus) });
        }

        private string CalculateColor(float averageStatus)
        {
            if (averageStatus >= 0 & averageStatus < 4)
            {
                return "Kırmızı";
            }
            else if (averageStatus >= 4 & averageStatus < 7)
            {
                return "Sarı";
            }
            else
            {
                return "Yeşil";
            }
        }
    }
}
