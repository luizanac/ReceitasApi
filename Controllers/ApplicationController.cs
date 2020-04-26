using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ReceitasApi.Database;
using ReceitasApi.Entities;

namespace ReceitasApi.Controllers {

    public class ApplicationController : ControllerBase {
        protected readonly UserManager<User> UserManager;
        protected readonly IConfiguration Configuration;
        protected readonly ApplicationDbContext DbContext;
        protected readonly IMapper Mapper;
        protected const int DefaultPaginationSize = 10;

        public ApplicationController (
            UserManager<User> userManager,
            IConfiguration configuration,
            ApplicationDbContext dbContext,
            IMapper mapper
        ) {
            UserManager = userManager;
            Configuration = configuration;
            DbContext = dbContext;
            Mapper = mapper;
        }

        protected virtual void SetPaginationHeader (string route, int totalPages, int currentPage, int size, int prevPage, int nextPage, int totalDataCount) {
            HttpContext.Response.Headers.Add ("X-Total-Count", totalDataCount.ToString ());
            HttpContext.Response.Headers.Add ("Link",
                $"<{route}&page={currentPage}>; rel=\"first\", <{route}&page={nextPage}>; rel=\"next\", <{route}&page={totalPages}>; rel=\"last\""
            );
        }

        protected async Task<User> GetCurrentUser () {
            return await UserManager.FindByIdAsync (HttpContext.User.FindFirstValue (JwtRegisteredClaimNames.Sid));
        }

        protected Guid GetCurrentUserId () {
            return Guid.Parse (HttpContext.User.FindFirstValue (JwtRegisteredClaimNames.Sid));
        }

        protected void AddMessage (string key, string message) {
            ModelState.AddModelError (key, message);
        }

    }
}