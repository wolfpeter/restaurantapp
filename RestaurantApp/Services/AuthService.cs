using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RestaurantApp.DataAccess.DbContexts;
using RestaurantApp.Models.DTOs;
using RestaurantApp.Models.Entities;
using RestaurantApp.Models.Enums;

namespace RestaurantApp.Services;

public class AuthService : IAuthService
{
    private readonly RestaurantAppDbContext _dbContext;
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;

    public AuthService(RestaurantAppDbContext dbContext, IConfiguration configuration, IMapper mapper)
    {
        _dbContext = dbContext;
        _configuration = configuration;
        _mapper = mapper;
    }
    
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        if (await _dbContext.Users.AnyAsync(c => c.Email == request.Email && c.Deleted == null))
        {
            throw new ArgumentException("Email is already registered.");
        }

        var customer = new User
        {
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            Address = request.Address,
            Role = Role.User
        };

        _dbContext.Users.Add(customer);
        await _dbContext.SaveChangesAsync();

        var token = GenerateJwtToken(customer);

        return new AuthResponse
        {
            Token = token,
            UserId = customer.Id,
            Email = customer.Email,
            Message = "Registration successful!"
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var customer = await _dbContext.Users.SingleOrDefaultAsync(c => c.Email == request.Email && c.Deleted == null);

        if (customer == null || !BCrypt.Net.BCrypt.Verify(request.Password, customer.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        var token = GenerateJwtToken(customer);

        return new AuthResponse
        {
            Token = token,
            UserId = customer.Id,
            Email = customer.Email,
            Message = "Login successful!"
        };
    }

    public async Task<UserDto> GetMeAsync(int userId)
    {
        var customer = await _dbContext.Users.FindAsync(userId);

        if (customer == null)
        {
            throw new KeyNotFoundException("User not found.");
        }

        return _mapper.Map<UserDto>(customer);
    }

    private string GenerateJwtToken(User user)
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]!);
        var tokenHandler = new JwtSecurityTokenHandler();

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        if (user.RestaurantId.HasValue)
        {
            claims.Add(new Claim("restaurantId", user.RestaurantId.Value.ToString()));
        }
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(int.Parse(jwtSettings["ExpiryMinutes"]!)),
            Issuer = jwtSettings["Issuer"],
            Audience = jwtSettings["Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}