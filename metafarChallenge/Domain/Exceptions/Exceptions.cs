namespace Domain.Exceptions
{
    public class CardNotFoundException : Exception
    {
        public CardNotFoundException() : base("No se encuentra tarjeta con el nro ingresado.")
        {
        }
    }

    public class CardBlocked : Exception
    {
        public CardBlocked() : base("Su tarjeta fue bloqueada.")
        {
        }
    }

    public class BadPin : Exception
    {
        public BadPin() : base("Pin incorrecto.")
        {
        }
    }
    public class BadAmount : Exception
    {
        public BadAmount() : base("Su cuenta no tiene suficientes fondos.")
        {
        }
    }
}