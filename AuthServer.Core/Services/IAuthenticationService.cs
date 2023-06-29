using AuthServer.Core.Dtos;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Services
{
    public interface IAuthenticationService
    {
        Task<ResponseDto<TokenDto>> CreateTokenAsync(LoginDto loginDto);

        Task<ResponseDto<TokenDto>> CreateTokenByRefreshToken(string refreshToken);

        Task<ResponseDto<NoContentResult>> RevokeRefreshToken(string refreshToken);

        ResponseDto<ClientTokenDto> CreateTokenByClient (ClientLoginDto clientLoginDto);
    }
}
