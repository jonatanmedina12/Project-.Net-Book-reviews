using AutoMapper;
using BookReviews.Application.DTOs;
using BookReviews.Application.Interfaces;
using BookReviews.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookReviews.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IIdentityService _identityService;
        private readonly IMapper _mapper;
        private readonly ILoggerService _logger;

        public AuthService(
            IIdentityService identityService,
            IMapper mapper,
            ILoggerService logger)
        {
            _identityService = identityService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<UserDto> RegisterAsync(RegisterDto registerDto)
        {
            try
            {
                return await _identityService.RegisterUserAsync(
                    registerDto.Username,
                    registerDto.Email,
                    registerDto.Password);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar usuario {Username}", registerDto.Username);
                throw;
            }
        }

        public async Task<TokenDto> LoginAsync(LoginDto loginDto)
        {
            try
            {
                return await _identityService.AuthenticateAsync(
                    loginDto.Email,
                    loginDto.Password);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al iniciar sesión para {Email}", loginDto.Email);
                throw;
            }
        }

        public async Task ForgotPasswordAsync(string email)
        {
            try
            {
                // En una implementación real, aquí enviarías un correo con un token
                // Por ahora, solo registramos la solicitud
                _logger.LogInformation("Solicitud de restablecimiento de contraseña para {Email}", email);

                // Para una implementación real, crea un token de restablecimiento y envía un correo
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar solicitud de restablecimiento de contraseña para {Email}", email);
                throw;
            }
        }

        public async Task ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            try
            {
                await _identityService.ResetPasswordAsync(
                    resetPasswordDto.Email,
                    resetPasswordDto.Token,
                    resetPasswordDto.Password);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al restablecer contraseña para {Email}", resetPasswordDto.Email);
                throw;
            }
        }
    }
}
