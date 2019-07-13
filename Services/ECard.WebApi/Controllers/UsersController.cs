using AutoMapper;
using ECard.Entities.Entities;
using ECard.WebApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApi.Dtos;
using WebApi.Helpers;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private IUserService _userService;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;

        public UsersController(
            IUserService userService,
            IMapper mapper,
            IOptions<AppSettings> appSettings)
        {
            _userService = userService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        [AllowAnonymous]
        [HttpPost("Authenticate")]
        public IActionResult Authenticate([FromBody]UserDto userDto)
        {
            var user = _userService.Authenticate(userDto.Username, userDto.Password);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] 
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // return basic user info (without password) and token to store client side
            return Ok(new {
                Id = user.Id,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Token = tokenString
            });
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody]UserDto userDto)
        {
            // map dto to entity
            var user = _mapper.Map<User>(userDto);

            try 
            {
                // save 
                _userService.Create(user, userDto.Password);
                return Ok();
            } 
            catch(AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var users =  _userService.GetAll();
            var userDtos = _mapper.Map<IList<UserDto>>(users);
            return Ok(userDtos);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var user =  _userService.GetById(id);
            var userDto = _mapper.Map<UserDto>(user);
            return Ok(userDto);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody]UserDto userDto)
        {
            // map dto to entity and set id
            var user = _mapper.Map<User>(userDto);
            user.Id = id;

            try 
            {
                // save 
                _userService.Update(user, userDto.Password);
                return Ok();
            } 
            catch(AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _userService.Delete(id);
            return Ok();
        }

        class ClaimDTO
        {
            public string Issuer { get; set; }
            public string OriginalIssuer { get; set; }
            public string Type { get; set; }
            public string Value { get; set; }
            public string ValueType { get; set; }
        }
        class ClaimsIdentityDTO
        {
            public string Name { get; set; }
            public string AuthenticationType { get; set; }
            public bool IsAuthenticated { get; set; }
            public string NameClaimType { get; set; }
            public string RoleClaimType { get; set; }
            public string Label { get; set; }

            public List<ClaimDTO> Claims { get; set; }

            public ClaimsIdentityDTO()
            {
                Claims = new List<ClaimDTO>();
            }
        }
        static ClaimsIdentityDTO CreateFrom(ClaimsIdentity ci)
        {
            ClaimsIdentityDTO ciDTO = new ClaimsIdentityDTO()
            {
                Name = ci.Name,
                AuthenticationType = ci.AuthenticationType,
                IsAuthenticated = ci.IsAuthenticated,
                Label = ci.Label,
                NameClaimType = ci.NameClaimType,
                RoleClaimType = ci.RoleClaimType
            };
            foreach (var claim in ci.Claims)
            {
                var claimDTO = new ClaimDTO()
                {
                    Issuer = claim.Issuer,
                    OriginalIssuer = claim.OriginalIssuer,
                    Type = claim.Type,
                    Value = claim.Value,
                    ValueType = claim.ValueType
                };
                ciDTO.Claims.Add(claimDTO);
            }
            return ciDTO;
        }

        [Route("GetClaimDetails")]
        public string GetClaimDetails()
        {
            var identity = User.Identity as ClaimsIdentity;

            // Create DTO object
            var ciDTO = CreateFrom(identity);
            // Serialize it to json
            var json = JsonConvert.SerializeObject(ciDTO);

            return json;
        }
    }
}
