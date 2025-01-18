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
                //Paket durumu için bir değişken tanımladık
                PackageStatus = c.TotalStatus >= 10 ? "Paketi almıştır" : "Paketi almamıştır"
            })
            .ToList();

            return Ok(customers);
        }

        [HttpGet("{id}")]
        public IActionResult GetCustomerById(int id)
        {
            // İlgili müşteriyi veritabanından bul
            var customer = _context.Customers
                .Where(c => c.Id == id)
                .Select(c => new
                {
                    c.Id,
                    c.Name,
                    c.PhoneNumber,
                    c.CallCount,
                    c.TotalStatus,
                    PackageStatus = c.TotalStatus >= 10 ? "Paketi almıştır" : "Paketi almamıştır"
                })
                .FirstOrDefault();

            // Eğer müşteri bulunamazsa, NotFound döndür
            if (customer == null)
            {
                return NotFound(new { message = "Müşteri bulunamadı." });
            }

            // Müşteri bilgilerini döndür
            return Ok(customer);
        }


        [HttpPost]
        public IActionResult AddCustomer([FromBody] CustomerInsertDto customerDto)
        {
            //İlk müşteri oluşturmada totalStatus ve CallCount 0'dan başlar
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
            
            //Müşterinin statusu 10'dan büyükse üzerinde güncelleme yapamayız
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
        public IActionResult MakeCall([FromBody] MakeCallDto makeCall)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Hatalı parametreleri döndürür.
            }

            var customer = _context.Customers.Where(x => x.Id == makeCall.CustomerId && x.TotalStatus < 10).FirstOrDefault();

            if (customer == null)
                return NotFound("Müşteri yok veya paketi satın aldı");

            var call = new Call
            {
                CustomerId = makeCall.CustomerId,
                AdminId = makeCall.AdminId,
                CallDate = DateTime.Now,
                Notes = makeCall.Notes,
                Status = makeCall.Status
            };

            _context.Calls.Add(call);
            customer.CallCount++;

            if (makeCall.Status >= 10)
            {
                customer.TotalStatus = 10;
                _context.SaveChanges();
                
                return Ok(new { message = "Çağrı başarıyla yapıldı, Müşteri paketi aldı.", customer });
            }

            customer.TotalStatus = CalculateTotalStatusAverage(makeCall.CustomerId, call);

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
