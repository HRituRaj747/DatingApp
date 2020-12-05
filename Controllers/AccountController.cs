using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using API.DTOS;
using Microsoft.EntityFrameworkCore;
using API.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _conn;
        private readonly ITokenService _token;

        public AccountController(DataContext conn,ITokenService token)
        {
            this._conn = conn;
            this._token = token;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDtos) 
        {
            if(await UserExists(registerDtos.Username))
            {
                return BadRequest("Username is taken");
            }
            using var hmac = new HMACSHA512();

            var user = new AppUser()
            {
                UserName = registerDtos.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDtos.Password)),
                PasswordSalt = hmac.Key
            };

            _conn.Users.Add(user);
            await _conn.SaveChangesAsync();


            return new UserDto {
                Username =user.UserName,
                Token = _token.CreateToken(user)
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login (loginDto login)
        {   //List<AppUser> user = new List<AppUser>();   
           var user= await _conn.Users.SingleOrDefaultAsync(x => x.UserName.ToLower()== login.Username.ToLower());
          //user =  _conn.Users.Select(s=>new { s.UserName,s.PasswordSalt,s.PasswordHash}).Where().ToList();
            if(user == null)
            {
                return Unauthorized("Invalid username");
            }

            using var hmac = new HMACSHA512(user.PasswordSalt);

            var ComputeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(login.Password));

            for(int i =0; i< ComputeHash.Length; i++)
            {
                if(ComputeHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Password");
            }
             return new UserDto {
                Username =user.UserName,
                Token = _token.CreateToken(user)
            };
        }

        [HttpDelete("delete")]

        public async Task<ActionResult<AppUser>> DeleteUser (loginDto dt)
        {

            var x = await  _conn.Users.FirstOrDefaultAsync(x=>x.UserName == dt.Username);

            if(x!= null)
            {
                 _conn.Users.Remove(x);

                 await _conn.SaveChangesAsync();

            }
            else{
                return Unauthorized("This user doest exist");
            }

            return Ok("User Deleted successfully");
        }
















        private async Task<bool> UserExists(string username)
        {
            return await _conn.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
    }
}