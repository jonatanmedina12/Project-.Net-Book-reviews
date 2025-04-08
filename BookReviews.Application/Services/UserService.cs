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
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IIdentityService _identityService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserService(
            IUserRepository userRepository,
            IIdentityService identityService,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _identityService = identityService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<UserDto> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return null;

            return _mapper.Map<UserDto>(user);
        }

        public async Task<bool> UpdateProfileAsync(int userId, UpdateProfileDto updateProfileDto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return false;

            // Verificar si el nombre de usuario ya existe (si se está cambiando)
            if (user.Username != updateProfileDto.Username)
            {
                var existingUser = await _userRepository.GetByUsernameAsync(updateProfileDto.Username);
                if (existingUser != null)
                    throw new ArgumentException($"El nombre de usuario '{updateProfileDto.Username}' ya está en uso");
            }

            // Verificar si el correo electrónico ya existe (si se está cambiando)
            if (user.Email != updateProfileDto.Email)
            {
                var existingUser = await _userRepository.GetByEmailAsync(updateProfileDto.Email);
                if (existingUser != null)
                    throw new ArgumentException($"El correo electrónico '{updateProfileDto.Email}' ya está registrado");
            }

            user.UpdateProfile(
                updateProfileDto.Username,
                updateProfileDto.Email,
                updateProfileDto.ProfilePictureUrl);

            await _userRepository.UpdateAsync(user);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto)
        {
            return await _identityService.ChangePasswordAsync(
                userId,
                changePasswordDto.CurrentPassword,
                changePasswordDto.NewPassword);
        }
    }
}
