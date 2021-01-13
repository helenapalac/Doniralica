using BC = BCrypt.Net.BCrypt;
using Doniralica.Data;
using Doniralica.Data.Models;
using Doniralica.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using Doniralica.Models.Write;
using Doniralica.Models.Read;
using Microsoft.EntityFrameworkCore;

namespace Doniralica.Controllers
{
    [ApiController]
    [Route("users")]
    public class UsersController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAsync(int page = 1, int pageSize = 10)
        {
            var count = _context.Users.Where(x => MyUser.IsAdminUser == true && x.Active == true).Count();

            var result = new PageResult<UserRead>
            {
                Count = count,
                PageIndex = page,
                PageSize = pageSize,
                Items = _context.Users
                    .Where(x => MyUser.IsAdminUser == true && x.Active == true)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(x => new UserRead
                    {
                        Id = x.Id,
                        UserName = x.UserName,
                        PhoneNumber = x.PhoneNumber,
                        Role = new RoleRead
                        {
                            Id = x.Role.Id,
                            Name = x.Role.Name,
                            Description = x.Role.Description
                        }
                    })
                    .ToList()
            };

            return Ok(result);
        }

        [HttpGet("{id}")]
        public IActionResult GetAsync(int id)
        {
            var dbUser = _context.Users
                            .Include(x => x.Role)
                            .FirstOrDefault(x => x.Id == id && MyUser.IsAdminUser == true && x.Active == true);

            if (dbUser == null)
            {
                return NotFound("User does not exist");
            }

            var user = new UserRead
            {
                Id = dbUser.Id,
                UserName = dbUser.UserName,
                PhoneNumber = dbUser.PhoneNumber,
                Role = new RoleRead
                {
                    Id = dbUser.Role.Id,
                    Name = dbUser.Role.Name,
                    Description = dbUser.Role.Description
                }
            };

            return Ok(user);
        }

        [HttpPost]
        public IActionResult Post([FromBody] UserWrite user)
        {
            var duplicateUser = _context.Users.FirstOrDefault(x => x.UserName.Equals(user.UserName));

            if (duplicateUser != null)
            {
                return BadRequest("User with the same username already exists");
            }

            var role = _context.Roles.FirstOrDefault(x => x.Id == user.Role.Id && MyUser.IsAdminUser == true && x.Active == true);

            if (role == null)
            {
                return NotFound("Role does not exist");
            }

            var password = BC.HashPassword(user.Password);

            var dbUser = new User
            {
                UserName = user.UserName,
                PasswordHash = password,
                PhoneNumber = user.PhoneNumber,
                Active = true,
                Role = role,
                Created = DateTime.UtcNow,
                CreatedUserId = MyUser.Id
            };

            var newDbUser = _context.Users.Add(dbUser);

            _context.SaveChanges();

            var newUser = new UserRead 
            {
                Id = newDbUser.Entity.Id,
                UserName = newDbUser.Entity.UserName,
                PhoneNumber = newDbUser.Entity.PhoneNumber,
                Role = new RoleRead 
                {
                    Id = newDbUser.Entity.Role.Id,
                    Name = newDbUser.Entity.Role.Name,
                    Description = newDbUser.Entity.Role.Description
                }
            };

            return Created($"{BaseUrl}/users/{newUser.Id}", newUser);
        }

        [HttpPut("{id}")]
        public IActionResult Put([FromBody] UserWrite user, int id)
        {
            var data = _context.Users.FirstOrDefault(x => x.Id == id && MyUser.IsAdminUser == true && x.Active == true);

            if (data == null)
            {
                return NotFound("User does not exist");
            }

            var role = _context.Roles.FirstOrDefault(x => x.Id == user.Role.Id && MyUser.IsAdminUser == true && x.Active == true);

            if (role == null)
            {
                return NotFound("Role does not exist");
            }

            data.UserName = user.UserName;
            data.PhoneNumber = user.PhoneNumber;
            data.Role = role;
            data.Modified = DateTime.UtcNow;
            data.ModifiedUserId = MyUser.Id;

            if (data.PasswordHash != BC.HashPassword(user.Password))
            {
                data.PasswordHash = BC.HashPassword(user.Password);
            }

            _context.Users.Update(data);

            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var user = _context.Users.FirstOrDefault(x => x.Id == id && MyUser.IsAdminUser == true && x.Active == true);

            if (user == null)
            {
                return NotFound("User does not exist");
            }

            user.Modified = DateTime.UtcNow;
            user.ModifiedUserId = MyUser.Id;
            user.Active = false;

            _context.Users.Update(user);

            _context.SaveChanges();

            return NoContent();
        }
    }
}
