using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookReviews.Application.DTOs
{
    public class TokenDto
    {
        public string Token { get; set; }
        public int ExpiresIn { get; set; }
    }
}
