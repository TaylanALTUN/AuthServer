using AuthServer.Core.Dtos;
using SharedLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Services
{
    public interface IUserService
    {
        Task<ResponseDto<UserAppDto>> CreateUserAsync(CreateUserDto createUserDto);

        Task<ResponseDto<UserAppDto>> GetUSerByNameAsync(string userName);

        Task<ResponseDto<NoDataDto>> CreateUserRoles(string userName);
    }
}
