using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stateful_Auth_Sample.Authentication;
using Stateful_Auth_Sample.Models;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Stateful_Auth_Sample.Controllers
{
    [Route("/api/[controller]"), Authorize(AuthenticationSchemes = CustomAuthenticationExtensions.CustomScheme)]
    public class UsersController : ControllerBase
    {
        private DataContext _data;

        public UsersController(DataContext data) => _data = data;

        // You'd likely create DTOs for this instead of requesting a database model straight from the user,
        // plus you'd also want to sanitize input before adding it to the database or updating data.

        [HttpPost("[action]"), AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] User user)
        {
            // I generally use BCrypt but for simplicity's sake I'm just going to store & retrieve plain-text
            // passwords.
            var dbUser = _data.Users.FirstOrDefault(s => s.Username == user.Password && s.Username == user.Username);
            if (dbUser == null)
                return BadRequest(new { Error = "Login failed (incorrect password & username combination)" });

            dbUser.AccessToken = RandomString(8);
            _data.Update(dbUser);
            await _data.SaveChangesAsync();
            return Ok(new { AccessToken = dbUser.AccessToken });
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Logout()
        {
            var userId = Convert.ToInt32(HttpContext.User.Claims.FirstOrDefault(s => s.Type == "Id")?.Value ?? "-1");
            var user = _data.Users.FirstOrDefault(s => s.Id == userId);
            user.AccessToken = null;
            _data.Update(user);
            await _data.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("[action]"), Authorize(Policy = Policies.CanAddUsers)]
        public async Task<IActionResult> Add([FromBody] User user)
        {
            _data.Add(user);
            await _data.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("[action]/{id}"), Authorize(Policy = Policies.CanDeleteUsers)]
        public async Task<IActionResult> Delete(int id)
        {
            var user = _data.Users.FirstOrDefault(s => s.Id == id);
            if (user == null)
                return BadRequest(new { Error = "The requested user doesn't exist" });
            _data.Remove(user);
            await _data.SaveChangesAsync();
            return Ok();
        }

        #region Overkill random string generator for a sample project.
        private static string RandomString(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyz1234567890";
            var res = new StringBuilder();
            using (RNGCryptoServiceProvider rnd = new RNGCryptoServiceProvider())
            {
                while (length-- > 0)
                {
                    res.Append(valid[GetInt(rnd, valid.Length)]);
                }
            }
            return res.ToString();
        }

        private static int GetInt(RNGCryptoServiceProvider rnd, int max)
        {
            byte[] r = new byte[4];
            int value;
            do
            {
                rnd.GetBytes(r);
                value = BitConverter.ToInt32(r, 0) & int.MaxValue;
            } while (value >= max * (int.MaxValue / max));
            return value % max;
        }
        #endregion
    }
}
