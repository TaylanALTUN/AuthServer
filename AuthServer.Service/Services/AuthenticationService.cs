using AuthServer.Core.Configurations;
using AuthServer.Core.Dtos;
using AuthServer.Core.Models;
using AuthServer.Core.Repositories;
using AuthServer.Core.Services;
using AuthServer.Core.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using SharedLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Service.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly List<Client> _clients;
        private readonly ITokenService _tokenService;
        private readonly UserManager<UserApp> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<UserRefreshToken> _userRefreshTokenService;

        public AuthenticationService(IOptions<List<Client>> clients, ITokenService tokenService, UserManager<UserApp> userManager, IUnitOfWork unitOfWork, IGenericRepository<UserRefreshToken> userRefreshTokenService)
        {
            _clients = clients.Value;
            _tokenService = tokenService;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _userRefreshTokenService = userRefreshTokenService;
        }

        public async Task<ResponseDto<TokenDto>> CreateTokenAsync(LoginDto loginDto)
        {
            if (loginDto == null) throw new ArgumentNullException(nameof(loginDto));

            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            
            if (user == null) 
                return ResponseDto<TokenDto>.Fail(StatusCodes.Status400BadRequest, "Email or Password is wrong", true);

            if(!await _userManager.CheckPasswordAsync(user,loginDto.Password))
                return ResponseDto<TokenDto>.Fail(StatusCodes.Status400BadRequest, "Email or Password is wrong", true);

            var token = _tokenService.CreateToken(user);

            var userRefreshToken= await _userRefreshTokenService.Where(x=> x.UserId== user.Id).SingleOrDefaultAsync();

            if(userRefreshToken == null)
            {
                await _userRefreshTokenService.AddAsync(new UserRefreshToken { UserId = user.Id, Code = token.RefreshToken, Expiration = token.RefreshTokenExpiration });
            }
            else
            {
                userRefreshToken.Code = token.RefreshToken;
                userRefreshToken.Expiration = token.RefreshTokenExpiration;
            }

            await _unitOfWork.CommitAsync();

            return ResponseDto<TokenDto>.Success(StatusCodes.Status200OK, token);

        }

        public ResponseDto<ClientTokenDto> CreateTokenByClient(ClientLoginDto clientLoginDto)
        {
            var client = _clients.SingleOrDefault(x => x.Id == clientLoginDto.ClientId && x.Secret == clientLoginDto.ClientSecret);

            if (client == null)
                return ResponseDto<ClientTokenDto>.Fail(StatusCodes.Status404NotFound, "Client or ClientSecret not found", true);

            var token=_tokenService.CreateTokenByClient(client);

            return ResponseDto<ClientTokenDto>.Success(StatusCodes.Status200OK, token);
        }

        public async Task<ResponseDto<TokenDto>> CreateTokenByRefreshToken(string refreshToken)
        {
            var existRefreshToken = await _userRefreshTokenService.Where(x=> x.Code == refreshToken).SingleOrDefaultAsync();

            if(existRefreshToken == null)
                return ResponseDto<TokenDto>.Fail(StatusCodes.Status404NotFound, "Refrest token not found", true);

            var user = await _userManager.FindByIdAsync(existRefreshToken.UserId);

            if (user == null)
                return ResponseDto<TokenDto>.Fail(StatusCodes.Status404NotFound, "User Id not found", true);

            var token = _tokenService.CreateToken(user);

            existRefreshToken.Code = token.RefreshToken;
            existRefreshToken.Expiration = token.RefreshTokenExpiration;

            await _unitOfWork.CommitAsync();

            return ResponseDto<TokenDto>.Success(StatusCodes.Status200OK, token);
        }

        public async Task<ResponseDto<NoContentResult>> RevokeRefreshToken(string refreshToken)
        {
            var existRefreshToken = await _userRefreshTokenService.Where(x => x.Code == refreshToken).SingleOrDefaultAsync();

            if (existRefreshToken == null)
                return ResponseDto<NoContentResult>.Fail(StatusCodes.Status404NotFound, "Refrest token not found", true);

           _userRefreshTokenService.Remove(existRefreshToken);

            await _unitOfWork.CommitAsync();

            return ResponseDto<NoContentResult>.Success(StatusCodes.Status200OK);
        }
    }
}
