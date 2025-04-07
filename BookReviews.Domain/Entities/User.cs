using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookReviews.Domain.Entities
{
    public class User
    {
        public int Id { get; private set; }
        public string Username { get; private set; }
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }
        public string ProfilePictureUrl { get; private set; }
        public DateTime RegisterDate { get; private set; }
        public ICollection<Review> Reviews { get; private set; } = new List<Review>();

        private User() { }

        public User(string username, string email, string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("El nombre de usuario no puede estar vacío", nameof(username));

            if (string.IsNullOrWhiteSpace(email) || !IsValidEmail(email))
                throw new ArgumentException("El correo electrónico no es válido", nameof(email));

            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("La contraseña hash no puede estar vacía", nameof(passwordHash));

            Username = username;
            Email = email;
            PasswordHash = passwordHash;
            RegisterDate = DateTime.UtcNow;
        }

        public void UpdateProfile(string username, string email, string profilePictureUrl)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("El nombre de usuario no puede estar vacío", nameof(username));

            if (string.IsNullOrWhiteSpace(email) || !IsValidEmail(email))
                throw new ArgumentException("El correo electrónico no es válido", nameof(email));

            Username = username;
            Email = email;
            ProfilePictureUrl = profilePictureUrl;
        }

        public void UpdatePassword(string newPasswordHash)
        {
            if (string.IsNullOrWhiteSpace(newPasswordHash))
                throw new ArgumentException("La nueva contraseña hash no puede estar vacía", nameof(newPasswordHash));

            PasswordHash = newPasswordHash;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
