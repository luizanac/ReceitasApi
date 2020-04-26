using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Luizanac.Utils.Extensions;
using Luizanac.Utils.Extensions.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ReceitasApi.Attributes;
using ReceitasApi.Constants;
using ReceitasApi.Controllers;
using ReceitasApi.Database;
using ReceitasApi.Dtos.User;
using ReceitasApi.Entities;

namespace Aplub.Api.Controllers {
    [ApiController]
    [Authorize (AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

    public class UserController : ApplicationController {
        public UserController (
            UserManager<User> userManager,
            IConfiguration configuration,
            ApplicationDbContext dbContext,
            IMapper mapper) : base (userManager, configuration, dbContext, mapper) { }

        [HttpPost]
        [ProducesResponseType (StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status400BadRequest)]
        [ProducesResponseType (StatusCodes.Status403Forbidden)]
        [Route ("api/users")]
        [Roles (UserTypes.Administrator)]
        public async Task<IActionResult> Post (CreateUserDto dto) {
            var currentUser = await GetCurrentUser ();

            var user = Mapper.Map<CreateUserDto, User> (dto);

            var createdResult = await UserManager.CreateAsync (user, dto.Password);
            if (createdResult.Succeeded) {
                return Ok (Mapper.Map<UserDto> (user));
            }

            return BadRequest (createdResult.Errors);
        }

        [HttpPatch]
        [ProducesResponseType (StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status400BadRequest)]
        [Route ("api/users/{id}")]
        [Roles (UserTypes.Administrator)]
        public async Task<IActionResult> Delete (Guid id) {
            var user = await UserManager.FindByIdAsync (id.ToString ());
            try {
                DbContext.Users.Remove (user);
                await DbContext.SaveChangesAsync ();
                return Ok ();
            } catch (Exception) {
                return BadRequest ();
            }
        }

        [HttpPut]
        [ProducesResponseType (StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status400BadRequest)]
        [Route ("api/users/{id}")]
        [Roles (UserTypes.Administrator)]
        public async Task<IActionResult> Put (Guid id, UpdateUserDto dto) {
            var user = await UserManager.FindByIdAsync (id.ToString ());
            if (user == null)
                return NotFound ();

            var testUserMail = await UserManager.FindByEmailAsync (dto.Email);
            if (testUserMail != null && testUserMail.Id != user.Id) {
                AddMessage ("email", "O e-mail informado já está em uso");
                return BadRequest (ModelState);
            }

            user = Mapper.Map<UpdateUserDto, User> (dto, user);

            await UserManager.UpdateAsync (user);
            return Ok (Mapper.Map<UserDto> (user));
        }

        [HttpGet]
        [ProducesResponseType (StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status400BadRequest)]
        [Route ("api/users/{id}")]
        [Roles (UserTypes.Administrator)]
        public async Task<IActionResult> Get (Guid id) {
            var user = await UserManager.FindByIdAsync (id.ToString ());
            if (user == null)
                return NotFound ();

            return Ok (Mapper.Map<UserDto> (user));
        }

        [HttpGet]
        [ProducesResponseType (StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status400BadRequest)]
        [Route ("api/users")]
        [Roles (UserTypes.Administrator)]
        public async Task<IActionResult> Get (
            EPaginate paginate = EPaginate.Yes,
            int page = 1,
            int size = DefaultPaginationSize,
            string search = null,
            string sort = null,
            bool? status = null,
            string userType = null
        ) {
            IList<User> users;
            if (paginate == EPaginate.Yes) {
                var paginatedUsers = await GetAllFilter (search, sort, status, userType).Paginate (page, size);
                users = paginatedUsers.Data;
                SetPaginationHeader (
                    $"api/users" +
                    $"?{nameof(paginate)}={(int)paginate}" +
                    $"&{nameof(search)}={search}" +
                    $"&{nameof(userType)}={userType}" +
                    $"&{nameof(status)}={status}",
                    paginatedUsers.TotalPages,
                    paginatedUsers.CurrentPage,
                    paginatedUsers.Size,
                    paginatedUsers.PrevPage,
                    paginatedUsers.NextPage,
                    paginatedUsers.TotalDataCount);
            } else
                users = await GetAllFilter (search, sort, status, userType).ToListAsync ();

            return Ok (Mapper.Map<IList<UserBasicDto>> (users));
        }

        private IQueryable<User> GetAllFilter (
            string search,
            string sort,
            bool? status,
            string userType) {
            var query = DbContext.Users
                .Where (x => EF.Functions.Like (x.Name, $"%{search}%"));

            if (status != null)
                query = query.Where (x => x.Active.Equals (status));

            if (userType != null)
                query = query.Where (x => x.UserType.Equals (userType));

            if (sort != null)
                query = query.OrderByString (sort);
            else
                query = query.OrderByDescending (x => x.Id);

            return query;
        }
    }
}