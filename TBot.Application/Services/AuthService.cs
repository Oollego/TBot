using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBot.Domain.Entity;
using TBot.Domain.Interfaces.Repositories;
using TBot.Domain.Services;

namespace TBot.Application.Services
{
    internal class AuthService
    {
        private readonly IHashService _hashService;
        private readonly IBaseRepository<User> _userRepository;

        public AuthService(IHashService hashService, IBaseRepository<User> userRepository)
        {
            _userRepository = userRepository;
            _hashService = hashService;
        }

        public async Task Register(string username, string password, long userId)
        {
            if (username is null || password is null)
            {
                return;
            }

            var user = await _userRepository.GetAll().AsNoTracking().Where(u => u.Id == userId).FirstOrDefaultAsync();

            if (user is not null)
            {
                return;
            }

            string salt = _hashService.HexString(Guid.NewGuid().ToString());
            string dk = _hashService.HexString(salt + password);

            user = new User()
            {
                Id = userId,
                UserName = username,
                PasswordSalt = salt,
                PasswordDk = dk
            };

            await _userRepository.CreateAsync(user);
            await _userRepository.SaveChangesAsync();

        }



    }
}
