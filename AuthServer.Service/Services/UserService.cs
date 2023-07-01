using AuthServer.Core.Dtos;
using AuthServer.Core.Models;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using SharedLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Service.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<UserApp> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserService(UserManager<UserApp> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<ResponseDto<UserAppDto>> CreateUserAsync(CreateUserDto createUserDto)
        {
            var user = new UserApp { Email = createUserDto.Email, UserName = createUserDto.UserName };

            var result = await _userManager.CreateAsync(user, createUserDto.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(x => x.Description).ToList();

                return ResponseDto<UserAppDto>.Fail(StatusCodes.Status400BadRequest, new ErrorDto(errors, true));
            }

            return ResponseDto<UserAppDto>.Success(StatusCodes.Status200OK, ObjectMapper.Mapper.Map<UserAppDto>(user));
        }

        public async Task<ResponseDto<NoDataDto>> CreateUserRoles(string userName)
        {
            if (!await _roleManager.RoleExistsAsync("admin"))
            {
                await _roleManager.CreateAsync(new IdentityRole() { Name = "admin" });
                await _roleManager.CreateAsync(new IdentityRole() { Name = "manager" });
            }
            var user = await _userManager.FindByNameAsync(userName);

            await _userManager.AddToRoleAsync(user, "admin");
            await _userManager.AddToRoleAsync(user, "manager");

            return ResponseDto<NoDataDto>.Success(StatusCodes.Status201Created);
        }

        public async Task<ResponseDto<UserAppDto>> GetUSerByNameAsync(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);

            if (user == null)
                return ResponseDto<UserAppDto>.Fail(StatusCodes.Status404NotFound, "UserName not founs", true);

            return ResponseDto<UserAppDto>.Success(StatusCodes.Status200OK, ObjectMapper.Mapper.Map<UserAppDto>(user));
        }
    }
}
