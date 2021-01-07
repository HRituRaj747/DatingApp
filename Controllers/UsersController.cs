using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Data;
using API.DTOS;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{

    //[Authorize]
    public class UsersController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UsersController(DataContext context, IUserRepository userRepository, IMapper mapper)
        {
            this._mapper = mapper;
            this._userRepository = userRepository;
            this._context = context;
        }
        [Authorize]
        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            // var user = await _context.Users.FindAsync(id);
            // return user;
            var user = await _userRepository.GetMemberAsync(username);

           // var usertoreturn=  _mapper.Map<MemberDto>(user);

            return user;

        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
        {
            var user = await _userRepository.GetMembersAsync();

            // Mapping is removed as mapping is done at repo level 

          //  var usertoreturn = _mapper.Map<IEnumerable<MemberDto>>(user);

            return Ok(user);
        }
        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var user =await _userRepository.GetuserByUsernameAsync(username);

            

            _mapper.Map(memberUpdateDto, user);

            _userRepository.Update(user);

            if(await _userRepository.SaveAllAsync())
            return NoContent();

            return BadRequest("Failed to update the user");
        }
    }
}