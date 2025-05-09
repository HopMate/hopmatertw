using Microsoft.AspNetCore.Identity;
using hopmate.Server.Models;
using hopmate.Server.Models.Dto;
using hopmate.Server.Data;
using hopmate.Server.Models.Entities;

namespace hopmate.Server.Services
{
    public class AuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtService _jwtService;
        private readonly ApplicationDbContext _context;

        public AuthService(UserManager<ApplicationUser> userManager, JwtService jwtService, ApplicationDbContext context)
        {
            _userManager = userManager;
            _jwtService = jwtService;
            _context = context;
        }

        public async Task<(bool Success, Dictionary<string, string> Errors)> RegisterAsync(RegisterDto registerDto)
        {
            var errors = new Dictionary<string, string>();

            var existingUser = await _userManager.FindByNameAsync(registerDto.Username);
            if (existingUser != null)
            {
                errors["username"] = "Username already exists.";
            }

            var existingEmail = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existingEmail != null)
            {
                errors["email"] = "Email is already taken.";
            }

            if (errors.Count > 0)
            {
                return (false, errors);
            }

            var user = new ApplicationUser
            {
                UserName = registerDto.Username,
                Email = registerDto.Email,
                Name = registerDto.Name,
                DateOfBirth = registerDto.DateOfBirth,
                HasDrivingLicense = registerDto.HasDrivingLicense
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (result.Succeeded)
            {
                var passenger = new Passenger
                {
                    IdUser = user.Id,
                    User = user
                };

                _context.Passengers.Add(passenger);
                await _context.SaveChangesAsync();

                return (true, new Dictionary<string, string>());
            }

            foreach (var error in result.Errors)
            {
                errors["password"] = error.Description;
            }

            return (false, errors);
        }
        public async Task<(string Token, string Message, Guid? IdUser)> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.Username);
            if (user != null && await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                var token = _jwtService.GenerateToken(user);
                return (token, "Login successful.", user.Id);
            }

            return (string.Empty, "Invalid credentials.", null);
        }
    }
}