using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BugTracker_API.Data;
using BugTracker_API.Dtos;
using BugTracker_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BugTracker_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : Controller
    {
        private readonly IAuthenticationRepository _repository;
        private readonly IConfiguration _configuration;

        public AuthenticationController(
            IAuthenticationRepository repository, IConfiguration configuration)
        {
            _repository = repository;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userToRegisterDto)
        {
            userToRegisterDto.Email = userToRegisterDto.Email.ToLower();

            if (await _repository.EmailExists(userToRegisterDto.Email))
                return BadRequest("Email já existe");

            var userToRegister = new User
            {
                Name = userToRegisterDto.Name,
                Email = userToRegisterDto.Email,
                Type = "Normal"
            };
            
            var registeredUser = await _repository.RegisterUser(
                userToRegister, userToRegisterDto.Password);

            return StatusCode(201);
        }

        [HttpGet("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            var userFromRepository = await _repository.LoginUser(
                userForLoginDto.Email, userForLoginDto.Password);

            if (userFromRepository == null)
                return Unauthorized();

            var claims = new []
            {
                new Claim(ClaimTypes.NameIdentifier, userFromRepository.Id.ToString()),
                new Claim(ClaimTypes.Email, userFromRepository.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value));

            var credentials = new SigningCredentials(
                key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new {
                token = tokenHandler.WriteToken(token)
            });
        }
    }
}