﻿namespace Domain.Exceptions
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
    public class NegativeAmount : Exception
    {
        public NegativeAmount() : base("El monto a retirar no puede ser negativo o cero.")
        {
        }
    }
    public class CardNumberIsNotValid: Exception
    {
        public CardNumberIsNotValid() : base("El numero de tarjeta ingresado no es válido.")
        {
        }
    }
    public class PageNotValid : Exception
    {
        public PageNotValid() : base("No se admite el valor 0 en la página.")
        {
        }
    }
}