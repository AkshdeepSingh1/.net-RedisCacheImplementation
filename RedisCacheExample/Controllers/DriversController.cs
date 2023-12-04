using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RedisCacheExample.DbContext;
using RedisCacheExample.Interface;
using RedisCacheExample.Model;

namespace RedisCacheExample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriversController : ControllerBase
    {
        private readonly DbApplication _context;
        private readonly IRedisCacheProvider redisCacheProvider;

        public DriversController(DbApplication context, IRedisCacheProvider redisCacheProvider)
        {
            _context = context;
            this.redisCacheProvider = redisCacheProvider;
        }

       
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Driver>>> GetDrivers()
        {
          var cacheKey = "AllDrivers";
          var redisData = await redisCacheProvider.TryGet<List<Driver>>(cacheKey);
          if (redisData != null )
            {
                //Returning data from redis cache if found
                return redisData;
            }
          else
            {
                //Db Request if nothing is found from redis cache
                if (_context.Drivers == null)
                {
                    return NotFound();
                }
                // Simulate a delay of 5 seconds for larger data to be fetched from db to know the real importance of chaching
                await Task.Delay(TimeSpan.FromSeconds(5));
                var alldrivers = await _context.Drivers.ToListAsync();


                //Storing to redis cache

                int expiresIn = 30; //minutes (30 minutes)
                redisCacheProvider.Insert(cacheKey, alldrivers, TimeSpan.FromMinutes(expiresIn));
                return alldrivers;
            }
          
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Driver>> GetDriver(int id)
        {
            var cacheKey = $"Driver:{id}";
            var redisData = await redisCacheProvider.TryGet<Driver>(cacheKey);
            if (redisData != null)
            {
                //Returning data from redis cache if found
                return redisData;
            }
            else
            {
                //Db Request if nothing is found from redis cache

                if (_context.Drivers == null)
                {
                    return NotFound();
                }
                // Simulate a delay of 5 seconds for larger data to be fetched from db to know the real importance of chaching
                await Task.Delay(TimeSpan.FromSeconds(5));
                var driver = await _context.Drivers.FindAsync(id);

                if (driver == null)
                {
                    return NotFound();
                }

                //Storing to redis cache

                int expiresIn = 30; //minutes (30 minutes)
                redisCacheProvider.Insert(cacheKey, driver, TimeSpan.FromMinutes(expiresIn));
                return driver;
            }
           
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutDriver(int id, Driver driver)
        {
            if (id != driver.Id)
            {
                return BadRequest();
            }
            redisCacheProvider.Remove($"Driver:{id}");
            _context.Entry(driver).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DriverExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

     
        [HttpPost]
        public async Task<ActionResult<Driver>> PostDriver(Driver driver)
        {
          if (_context.Drivers == null)
          {
              return Problem("Entity set 'DbApplication.Drivers'  is null.");
          }
            _context.Drivers.Add(driver);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDriver", new { id = driver.Id }, driver);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDriver(int id)
        {
            if (_context.Drivers == null)
            {
                return NotFound();
            }
            var driver = await _context.Drivers.FindAsync(id);
            if (driver == null)
            {
                return NotFound();
            }
            redisCacheProvider.Remove($"Driver:{id}");
            _context.Drivers.Remove(driver);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DriverExists(int id)
        {
            return (_context.Drivers?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
