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
    [Route("subjects")]
    public class SubjectsController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public SubjectsController(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAsync(int page = 1, int pageSize = 10)
        {
            var count = _context.Subjects.Where(x => x.CreatedUserId == MyUser.Id && x.Active == true).Count();
            
            var result = new PageResult<SubjectRead>
            {
                Count = count,
                PageIndex = page,
                PageSize = pageSize,
                Items = _context.Subjects
                    .Where(x => x.CreatedUserId == MyUser.Id && x.Active == true)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(x => new SubjectRead 
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Address = x.Address,
                        ResponsiblePersonFullName = x.ResponsiblePersonFullName,
                        PhoneNumber = x.PhoneNumber,
                        Proteges = x.Proteges.Where(z => z.Active == true).Select(y => new ProtegeRead 
                        {
                            Id = y.Id,
                            Initials = $"{y.FirstName[0]}.{y.LastName[0]}.",
                            Age = y.Age,
                            Sex = y.Sex,
                            Wishes = y.Wishes.Where(k => k.Active == true).Select(z => new WishRead 
                            { 
                                Id = z.Id,
                                Description = z.Description,
                                DonatorName = z.DonatorName,
                                DonatorPhone = z.DonatorPhone
                            }).ToList()
                        }).ToList()
                    }).ToList()
            };

            return Ok(result);
        }

        [HttpGet("{id}")]
        public IActionResult GetAsync(int id)
        {
            var dbSubject = _context.Subjects.Include(x =>x.Proteges).FirstOrDefault(x => x.Id == id && x.CreatedUserId == MyUser.Id && x.Active == true);

            if (dbSubject == null)
            {
                return NotFound("Subject does not exist");
            }

            var subject = new SubjectRead
            {
                Id = dbSubject.Id,
                Name = dbSubject.Name,
                Address = dbSubject.Address,
                ResponsiblePersonFullName = dbSubject.ResponsiblePersonFullName,
                PhoneNumber = dbSubject.PhoneNumber,
                Proteges = dbSubject.Proteges.Where(z => z.Active == true).Select(y => new ProtegeRead
                {
                    Id = y.Id,
                    Initials = $"{y.FirstName[0]}.{y.LastName[0]}.",
                    Age = y.Age,
                    Sex = y.Sex,
                    Wishes = y.Wishes.Where(z => z.Active == true).Select(z => new WishRead
                    {
                        Id = z.Id,
                        Description = z.Description,
                        DonatorName = z.DonatorName,
                        DonatorPhone = z.DonatorPhone

                    }).ToList()
                }).ToList()
            };

            return Ok(subject);
        }

        [HttpPost]
        public IActionResult Post([FromBody] SubjectWrite subject)
        {
            var dbSubject = new Subject 
            { 
                Name = subject.Name,
                Address = subject.Address,
                ResponsiblePersonFullName = subject.ResponsiblePersonFullName,
                PhoneNumber = subject.PhoneNumber,
                Active = true,
                Created = DateTime.UtcNow,
                CreatedUserId = MyUser.Id
            };

            var newDbSubject = _context.Subjects.Add(dbSubject);

            _context.SaveChanges();

            var newSubject = new SubjectRead 
            {
                Id = newDbSubject.Entity.Id,
                Name = newDbSubject.Entity.Name,
                Address = newDbSubject.Entity.Address,
                ResponsiblePersonFullName = newDbSubject.Entity.ResponsiblePersonFullName,
                PhoneNumber = newDbSubject.Entity.PhoneNumber,
                Proteges = newDbSubject.Entity.Proteges.Select(x => new ProtegeRead 
                {
                    Id = x.Id,
                    Initials = $"{x.FirstName[0]}.{x.LastName[0]}.",
                    Age = x.Age,
                    Sex = x.Sex,
                    Wishes = x.Wishes.Select(y => new WishRead 
                    {
                        Id = y.Id,
                        Description = y.Description,
                        DonatorName = y.DonatorName,
                        DonatorPhone = y.DonatorPhone
                    }).ToList()
                }).ToList()
            };

            return Created($"{BaseUrl}/subjects/{newSubject.Id}", newSubject);
        }

        [HttpPut("{id}")]
        public IActionResult Put([FromBody] SubjectWrite subject, int id)
        {
            var dbSubject = _context.Subjects.FirstOrDefault(x => x.Id == id && x.CreatedUserId == MyUser.Id && x.Active == true);

            if (dbSubject == null)
            {
                return NotFound("Subject does not exist");
            }

            dbSubject.Name = subject.Name;
            dbSubject.Address = subject.Address;
            dbSubject.ResponsiblePersonFullName = subject.ResponsiblePersonFullName;
            dbSubject.PhoneNumber = subject.PhoneNumber;
            dbSubject.Modified = DateTime.UtcNow;
            dbSubject.ModifiedUserId = MyUser.Id;

            _context.Subjects.Update(dbSubject);

            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var subject = _context.Subjects.FirstOrDefault(x => x.Id == id && x.CreatedUserId == MyUser.Id && x.Active == true);

            if (subject == null)
            {
                return NotFound("Subject does not exist");
            }

            subject.Modified = DateTime.UtcNow;
            subject.ModifiedUserId = MyUser.Id;
            subject.Active = false;

            _context.Subjects.Update(subject);

            _context.SaveChanges();

            return NoContent();
        }
    }
}
