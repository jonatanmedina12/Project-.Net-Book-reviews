using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookReviews.Domain.ValueObjects
{
    public class Rating
    {
        public int Value { get; private set; }

        // Constructor privado para EF Core
        private Rating() { }

        public Rating(int value)
        {
            if (value < 1 || value > 5)
                throw new ArgumentException("La calificación debe estar entre 1 y 5", nameof(value));

            Value = value;
        }
    }
}
