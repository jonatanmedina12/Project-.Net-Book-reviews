using BookReviews.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookReviews.Application.Interfaces
{
    public interface IIdentityService
    {
        Task<UserDto> RegisterUserAsync(string username, string email, string password);
        Task<TokenDto> AuthenticateAsync(string email, string password);
        Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
        Task ResetPasswordAsync(string email, string resetToken, string newPassword);
    }
}
