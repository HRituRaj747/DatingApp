using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOS;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public UserRepository(DataContext context, IMapper mapper)
        {
            this._mapper = mapper;
            this._context = context;

        }

        public async Task<MemberDto> GetMemberAsync(string username)
        {
            //if we dont want to use automapper then can proceed like thisHere member dto will be projected out as output 
            // return await _context.Users.Where(x => x.UserName == username)
            // 
            // .Select(user => new MemberDto
            // {
            //     Id = user.Id,
            //     UserName = user.UserName

            // }).SingleOrDefaultAsync();

            // Auto mapper basically optimizes our code and fetches only those data which are provided in dto thus improving performace and saves time
            //It should be done at repo level, the reason is it will save us from mapping in controller also the query will fetch only those data which are in dtos class

            return await _context.Users.Where(x => x.UserName == username)
                        .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                        .SingleOrDefaultAsync();

        }

        public async Task<IEnumerable<MemberDto>> GetMembersAsync()
        {
            return await _context.Users.ProjectTo<MemberDto>(_mapper.ConfigurationProvider).ToListAsync();
           // throw new System.NotImplementedException();
        }

        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<AppUser> GetuserByUsernameAsync(string username)
        {
            return await _context.Users
            .Include(p => p.Photos)
            .SingleOrDefaultAsync(x => x.UserName == username);
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await _context.Users
            .Include(p => p.Photos)
            .ToListAsync();
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public void Update(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }
    }
}