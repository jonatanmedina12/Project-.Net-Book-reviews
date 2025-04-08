using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookReviews.Application.Interfaces
{
    public interface IJwtGenerator
    {
        string GenerateToken(int userId, string username, string email, string role);
    }
}
