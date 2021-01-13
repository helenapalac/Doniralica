using Doniralica.Data;
using Doniralica.Data.Models;
using Doniralica.Models;
using Doniralica.Models.Read;
using Doniralica.Models.Write;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace Doniralica.Controllers
{
    [ApiController]
    [Route("proteges/{protegeId}/wishes")]
    public class WishesController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public WishesController(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAsync(int protegeId, int page = 1, int pageSize = 10)
        {
            var dbWishes = _context.Wishes.Where(x => x.ProtegeId == protegeId && x.CreatedUserId == MyUser.Id && x.Active == true);

            var count = dbWishes.Count();

            var result = new PageResult<WishRead>
            {
                Count = count,
                PageIndex = page,
                PageSize = 10,
                Items = dbWishes
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(x => new WishRead 
                    {
                        Id = x.Id,
                        Description = x.Description,
                        DonatorName = x.DonatorName,
                        DonatorPhone = x.DonatorPhone
                    }).ToList()
            };

            return Ok(result);
        }

        [HttpGet("{id}")]
        public IActionResult GetAsync(int id)
        {
            var dbWish = _context.Wishes.FirstOrDefault(x => x.Id == id && x.CreatedUserId == MyUser.Id && x.Active == true);

            if (dbWish == null)
            {
                return NotFound("Wish does not exist");
            }

            var wish = new WishRead 
            {
                Id = dbWish.Id,
                Description = dbWish.Description,
                DonatorName = dbWish.DonatorName,
                DonatorPhone = dbWish.DonatorPhone
            };

            return Ok(wish);
        }

        [HttpPost]
        public IActionResult Post([FromBody] WishWrite wish, int protegeId)
        {
            var dbProtege = _context.Proteges.FirstOrDefault(x => x.Id == protegeId && x.CreatedUserId == MyUser.Id && x.Active == true);

            if (dbProtege == null)
            {
                return NotFound("Protege does not exist");
            }

            var dbWish = new Wish 
            {
                Description = wish.Description,
                Active = true,
                ProtegeId = protegeId,
                Created = DateTime.UtcNow,
                CreatedUserId = MyUser.Id
            };

            var newDbWish = _context.Wishes.Add(dbWish);

            _context.SaveChanges();

            var newWish = new WishRead 
            {
                Id = newDbWish.Entity.Id,
                Description = wish.Description,
                DonatorName = newDbWish.Entity.DonatorName,
                DonatorPhone = newDbWish.Entity.DonatorPhone
            };

            return Created($"{BaseUrl}/proteges/{protegeId}/wishes/{newWish.Id}", newWish);
        }

        [HttpPut("{id}")]
        public IActionResult Put([FromBody] WishWrite wish, int id)
        {
            var dbWish = _context.Wishes.FirstOrDefault(x => x.Id == id && x.CreatedUserId == MyUser.Id && x.Active == true);

            if (dbWish == null)
            {
                return NotFound("Wish does not exist");
            }

            dbWish.Description = wish.Description;
            dbWish.Modified = DateTime.UtcNow;
            dbWish.ModifiedUserId = MyUser.Id;

            _context.Wishes.Update(dbWish);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var wish = _context.Wishes.FirstOrDefault(x => x.Id == id && x.CreatedUserId == MyUser.Id && x.Active == true);

            if (wish == null)
            {
                return NotFound("Wish does not exist");
            }

            wish.Modified = DateTime.UtcNow;
            wish.ModifiedUserId = MyUser.Id;
            wish.Active = false;

            _context.Wishes.Update(wish);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPut("{id}/donate")]
        public IActionResult Donate([FromBody] WishDonate wish, int id)
        {
            var dbWish = _context.Wishes.FirstOrDefault(x => x.Id == id && x.Active == true);

            if (dbWish == null)
            {
                return NotFound("Wish does not exist");
            }

            dbWish.DonatorName = wish.DonatorName;
            dbWish.DonatorPhone = wish.DonatorPhone;
            dbWish.Modified = DateTime.UtcNow;
            dbWish.ModifiedUserId = null;

            _context.Wishes.Update(dbWish);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
