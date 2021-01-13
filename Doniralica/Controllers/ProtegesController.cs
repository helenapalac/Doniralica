using Doniralica.Data;
using Doniralica.Data.Models;
using Doniralica.Models;
using Doniralica.Models.Read;
using Doniralica.Models.Write;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Doniralica.Controllers
{
    [ApiController]
    [Route("subjects/{subjectId}/proteges")]
    public class ProtegesController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public ProtegesController(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAsync(int subjectId, int page = 1, int pageSize = 10)
        {
            var dbProtege = _context.Proteges.Include(x => x.Wishes).Where(x => x.CreatedUserId == MyUser.Id && x.SubjectId == subjectId && x.Active == true);

            var count = dbProtege.Count();

            var result = new PageResult<ProtegeRead>
            {
                Count = count,
                PageIndex = page,
                PageSize = 10,
                Items = dbProtege
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(x => new ProtegeRead
                    {
                        Id = x.Id,
                        Initials = $"{x.FirstName[0]}.{x.LastName[0]}.",
                        Age = x.Age,
                        Sex = x.Sex,
                        Wishes = x.Wishes.Where(z => z.Active == true).Select(y => new WishRead
                        {
                            Id = y.Id,
                            Description = y.Description,
                            DonatorName = y.DonatorName,
                            DonatorPhone = y.DonatorPhone
                        }).ToList()
                    }).ToList()
            };

            return Ok(result);
        }

        [HttpGet("{id}")]
        public IActionResult GetAsync(int id)
        {
            var dbProtege = _context.Proteges.Include(x => x.Wishes).FirstOrDefault(x => x.Id == id && x.CreatedUserId == MyUser.Id && x.Active == true);

            if (dbProtege == null)
            {
                return NotFound("Protege does not exist");
            }

            var protege = new ProtegeRead
            {
                Id = dbProtege.Id,
                Initials = $"{dbProtege.FirstName[0]}.{dbProtege.LastName[0]}.",
                Age = dbProtege.Age,
                Sex = dbProtege.Sex,
                Wishes = dbProtege.Wishes.Where(z => z.Active == true).Select(y => new WishRead
                {
                    Id = y.Id,
                    Description = y.Description,
                    DonatorName = y.DonatorName,
                    DonatorPhone = y.DonatorPhone
                }).ToList()
            };

            return Ok(protege);
        }

        [HttpPost]
        public IActionResult Post([FromBody] ProtegeWrite protege, int subjectId)
        {
            var dbSubject = _context.Subjects.FirstOrDefault(x => x.Id == subjectId && x.CreatedUserId == MyUser.Id && x.Active == true);

            if (dbSubject == null)
            {
                return NotFound("Subject does not exist");
            }

            var newDbProtege = _context.Proteges.Add(new Protege
            {
                FirstName = protege.FirstName.Trim(),
                LastName = protege.LastName.Trim(),
                Age = protege.Age,
                Sex = protege.Sex,
                Active = true,
                SubjectId = subjectId,
                Created = DateTime.UtcNow,
                CreatedUserId = MyUser.Id
            });

            _context.SaveChanges();

            var newProtege = new ProtegeRead
            {
                Id = newDbProtege.Entity.Id,
                Initials = $"{newDbProtege.Entity.FirstName[0]}.{newDbProtege.Entity.LastName[0]}.",
                Age = newDbProtege.Entity.Age,
                Sex = newDbProtege.Entity.Sex,
                Wishes = newDbProtege.Entity.Wishes.Select(y => new WishRead
                {
                    Id = y.Id,
                    Description = y.Description,
                    DonatorName = y.DonatorName,
                    DonatorPhone = y.DonatorPhone
                }).ToList()
            };

            return Created($"{BaseUrl}/subjects/{subjectId}/proteges/{newProtege.Id}", newProtege);
        }

        [HttpPut("{id}")]
        public IActionResult Put([FromBody] ProtegeWrite protege, int subjectId, int id)
        {
            var dbProtege = _context.Proteges.Include(x =>x.Wishes).FirstOrDefault(x => x.Id == id && x.SubjectId == subjectId && x.CreatedUserId == MyUser.Id && x.Active == true);

            if (dbProtege == null)
            {
                return NotFound("Protege does not exist");
            }

            dbProtege.FirstName = protege.FirstName.Trim();
            dbProtege.LastName = protege.LastName.Trim();
            dbProtege.Age = protege.Age;
            dbProtege.Sex = protege.Sex;
            dbProtege.Modified = DateTime.UtcNow;
            dbProtege.ModifiedUserId = MyUser.Id;

            _context.Proteges.Update(dbProtege);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int subjectId, int id)
        {
            var protege = _context.Proteges.FirstOrDefault(x => x.Id == id && x.SubjectId == subjectId && x.CreatedUserId == MyUser.Id && x.Active == true);

            if (protege == null)
            {
                return NotFound("Protege does not exist");
            }

            protege.Modified = DateTime.UtcNow;
            protege.ModifiedUserId = MyUser.Id;
            protege.Active = false;

            _context.Proteges.Update(protege);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
