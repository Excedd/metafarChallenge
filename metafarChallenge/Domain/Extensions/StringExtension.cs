using Domain.Exceptions;

namespace Domain.Extensions
{
    public static class StringExtension
    {
        public static void IsValidCardNumber(this string @this)
        {
            if (!long.TryParse(@this, out _))
                throw new CardNumberIsNotValid();
        }
    }
}