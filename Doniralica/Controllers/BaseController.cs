using Doniralica.Data;
using Doniralica.Data.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Doniralica.Controllers
{
    public class BaseController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BaseController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Logged in user
        /// </summary>
        public User MyUser
        {
            get
            {
                int.TryParse(HttpContext.Request.Headers["userId"], out int userId);

                return _context.Users.FirstOrDefault(x => x.Id == userId && x.Active == true);
            }
        }

        public string BaseUrl { get { return $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}"; } }
    }
}
