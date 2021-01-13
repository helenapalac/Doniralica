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
    [Route("roles")]
    public class RolesController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public RolesController(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAsync(int page = 1, int pageSize = 10)
        {
            var count = _context.Roles.Where(x => MyUser.IsAdminUser == true && x.Active == true).Count();

            var result = new PageResult<RoleRead>
            {
                Count = count,
                PageIndex = page,
                PageSize = pageSize,
                Items = _context.Roles
                    .Where(x => MyUser.IsAdminUser == true && x.Active == true)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(x => new RoleRead 
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Description = x.Description
                    })
                    .ToList()
            };

            return Ok(result);
        }

        [HttpGet("{id}")]
        public IActionResult GetAsync(int id)
        {
            var dbRole = _context.Roles.FirstOrDefault(x => x.Id == id && MyUser.IsAdminUser == true && x.Active == true);

            if (dbRole == null)
            {
                return NotFound("Role does not exist");
            }

            var role = new RoleRead 
            {
                Id = dbRole.Id,
                Name = dbRole.Name,
                Description = dbRole.Description
            };

            return Ok(role);
        }

        [HttpPost]
        public IActionResult Post([FromBody] RoleWrite role)
        {
            var dbRole = new Role
            {
                Name = role.Name,
                Description = role.Description,
                Active = true,
                Created = DateTime.UtcNow,
                CreatedUserId = MyUser.Id
            };

            var newDbRole = _context.Roles.Add(dbRole);

            _context.SaveChanges();

            var newRole = new RoleRead 
            { 
                Id = newDbRole.Entity.Id,
                Name = newDbRole.Entity.Name,
                Description = newDbRole.Entity.Description
            };

            return Created($"{BaseUrl}/roles/{newRole.Id}", newRole);
        }

        [HttpPut("{id}")]
        public IActionResult Put([FromBody] RoleWrite role, int id)
        {
            var dbRole = _context.Roles.FirstOrDefault(x => x.Id == id && MyUser.IsAdminUser == true && x.Active == true);

            if (dbRole == null)
            {
                return NotFound("Role does not exist");
            }

            dbRole.Name = role.Name;
            dbRole.Description = role.Description;
            dbRole.Modified = DateTime.UtcNow;
            dbRole.ModifiedUserId = MyUser.Id;

            _context.Roles.Update(dbRole);

            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var dbRole = _context.Roles.FirstOrDefault(x => x.Id == id && MyUser.IsAdminUser == true && x.Active == true);

            if (dbRole == null)
            {
                return NotFound("Role does not exist");
            }

            dbRole.Modified = DateTime.UtcNow;
            dbRole.ModifiedUserId = MyUser.Id;
            dbRole.Active = false;

            _context.Roles.Update(dbRole);

            _context.SaveChanges();

            return NoContent();
        }
    }
}
