using BookReviews.Application.DTOs;
using BookReviews.Application.Interfaces;
using BookReviews.Core.Interfaces;
using BookReviews.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BC = BCrypt.Net.BCrypt;

namespace BookReviews.Infrastructure.Identity
{
    public class IdentityService : IIdentityService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly ILoggerService _logger;

        public IdentityService(
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            IConfiguration configuration,
            ILoggerService logger)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _logger = logger;
        }
        public async Task<bool> AssignRoleAsync(int userId, string role)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return false;

            user.AssignRole(role);
            await _userRepository.UpdateAsync(user);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation($"Rol '{role}' asignado al usuario {userId}");

            return true;
        }
        public async Task<UserDto> RegisterAsync(RegisterDto registerDto)
        {
            // Verificar si el usuario ya existe
            if (await _userRepository.GetByUsernameAsync(registerDto.Username) != null)
                throw new ArgumentException($"El nombre de usuario '{registerDto.Username}' ya está en uso");

            if (await _userRepository.GetByEmailAsync(registerDto.Email) != null)
                throw new ArgumentException($"El correo electrónico '{registerDto.Email}' ya está registrado");

            // Crear hash de la contraseña
            string passwordHash = BC.HashPassword(registerDto.Password);

            // Crear nuevo usuario
            var user = new User(registerDto.Username, registerDto.Email, passwordHash);
            await _userRepository.AddAsync(user);
            await _unitOfWork.CompleteAsync();

            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                ProfilePictureUrl = user.ProfilePictureUrl,
                RegisterDate = user.RegisterDate,
                Role = user.Role // Incluir el rol en el DTO

            };
        }

        public async Task<TokenDto> AuthenticateAsync(string email, string password)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
                throw new UnauthorizedAccessException("Credenciales inválidas");

            // Verificar contraseña
            if (!BC.Verify(password, user.PasswordHash))
                throw new UnauthorizedAccessException("Credenciales inválidas");

            // Generar token JWT
            var token = GenerateJwtToken(user);

            return new TokenDto
            {
                Token = token,
                ExpiresIn = int.Parse(_configuration["JWT:ExpiryInMinutes"]) * 60
            };
        }

        public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return false;

            // Verificar contraseña actual
            if (!BC.Verify(currentPassword, user.PasswordHash))
                throw new UnauthorizedAccessException("La contraseña actual es incorrecta");

            // Crear hash de la nueva contraseña
            string newPasswordHash = BC.HashPassword(newPassword);
            user.UpdatePassword(newPasswordHash);

            await _userRepository.UpdateAsync(user);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        public async Task ResetPasswordAsync(string email, string resetToken, string newPassword)
        {
            // En una implementación real, validaríamos el token de restablecimiento
            // Aquí simplemente cambiamos la contraseña

            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
                throw new ArgumentException("Usuario no encontrado");

            // Crear hash de la nueva contraseña
            string newPasswordHash = BC.HashPassword(newPassword);
            user.UpdatePassword(newPasswordHash);

            await _userRepository.UpdateAsync(user);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation($"Contraseña restablecida para el usuario {user.Id}");
        }

        private string GenerateJwtToken(User user)
        {
            var key = Encoding.ASCII.GetBytes(_configuration["JWT:Secret"]);
            var tokenHandler = new JwtSecurityTokenHandler();
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                // Añadir el rol como claim
                new Claim(ClaimTypes.Role, user.Role ?? "User") // Valor predeterminado "User" si es null
            };

                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(claims),
                        Expires = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["JWT:ExpiryInMinutes"])),
                        SigningCredentials = new SigningCredentials(
                            new SymmetricSecurityKey(key),
                            SecurityAlgorithms.HmacSha256Signature)
                    };

                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    return tokenHandler.WriteToken(token);
        }
    }
}
