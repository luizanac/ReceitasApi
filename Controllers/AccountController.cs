using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Aplub.Domain.Dtos.Account;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ReceitasApi.Authentication;
using ReceitasApi.Constants;
using ReceitasApi.Controllers;
using ReceitasApi.Database;
using ReceitasApi.Dtos.Account;
using ReceitasApi.Entities;

namespace Aplub.Api.Controllers {

    [ApiController]
    [Authorize (AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

    public class AccountController : ApplicationController {

        private readonly SignInManager<User> _signInManager;
        private readonly ITokenFactory _tokenFactory;

        public AccountController (
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IConfiguration configuration,
            ApplicationDbContext dbContext,
            IMapper mapper,
            ITokenFactory tokenFactory) : base (userManager, configuration, dbContext, mapper) {
            _signInManager = signInManager;
            _tokenFactory = tokenFactory;
        }

        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType (typeof (LoginResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status400BadRequest)]
        [Route ("api/accounts/auth")]
        public async Task<IActionResult> Auth (LoginDto dto) {
            var user = await UserManager.FindByEmailAsync (dto.Email);
            if (user == null)
                return BadRequest ();

            var result = await _signInManager.CheckPasswordSignInAsync (user, dto.Password, false);

            if (!result.Succeeded)
                return BadRequest ();

            var expiration = 30;
            var token = JwtHelper.GenerateToken (
                user,
                Environment.GetEnvironmentVariable (EnvVars.JwtKey),
                Configuration["JwtTokenConfig:Issuer"],
                Configuration["JwtTokenConfig:Audience"], expiration);

            var refreshToken = _tokenFactory.GenerateRefreshToken ();
            user.AddRefreshToken (refreshToken, 10);
            await DbContext.SaveChangesAsync ();
            return Ok (new LoginResultDto ("Bearer", token, refreshToken, DateTime.Now.AddDays (expiration), user.UserType, user.Name));
        }

        [HttpPost]
        [ProducesResponseType (StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status400BadRequest)]
        [AllowAnonymous]
        [Route ("api/accounts/refreshToken")]
        public async Task<IActionResult> Post (RefreshTokenDto dto) {
            var handler = new JwtSecurityTokenHandler ();
            var cp = handler.ReadJwtToken (dto.OldJwtToken);
            if (string.IsNullOrEmpty (cp.Id)) {
                var user = await UserManager.FindByIdAsync (cp.Payload.Claims.FirstOrDefault (x => x.Type.Equals (JwtRegisteredClaimNames.Sid)).Value);
                if (user != null && user.RefreshToken.Equals (dto.RefreshToken) && user.RefreshTokenExpiration >= DateTime.Now) {
                    var expiration = 30;
                    var token = JwtHelper.GenerateToken (
                        user,
                        Environment.GetEnvironmentVariable (EnvVars.JwtKey),
                        Configuration["JwtTokenConfig:Issuer"],
                        Configuration["JwtTokenConfig:Audience"], expiration);

                    var refreshToken = _tokenFactory.GenerateRefreshToken ();
                    user.AddRefreshToken (refreshToken, 10);
                    await DbContext.SaveChangesAsync ();
                    return Ok (new LoginResultDto ("Bearer", token, refreshToken, DateTime.Now.AddDays (expiration), user.UserType, user.Name));
                }
            }

            AddMessage ("Token", "Token inválido");
            return BadRequest ();
        }

        [HttpPatch]
        [ProducesResponseType (StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status400BadRequest)]
        [Authorize ()]
        [Route ("api/accounts")]
        public async Task<IActionResult> Patch (ChangePasswordDto dto) {
            var user = await GetCurrentUser ();
            var result = await UserManager.ChangePasswordAsync (user, dto.CurrentPassword, dto.NewPassword);
            if (result.Succeeded) {
                await UserManager.UpdateAsync (user);
                return Ok (Mapper.Map<CurrentUserDto> (user));
            }

            return BadRequest (ModelState);
        }

        [HttpGet]
        [ProducesResponseType (typeof (CurrentUserDto), StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status400BadRequest)]
        [Route ("api/accounts")]
        [Authorize]
        public async Task<IActionResult> CurrentUser () {
            var user = await GetCurrentUser ();
            if (user == null)
                return NotFound ();

            return Ok (Mapper.Map<CurrentUserDto> (user));
        }
    }
}