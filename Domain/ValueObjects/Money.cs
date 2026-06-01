using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.ValueObjects
{
    public class Money
    {
        private Money() { } // EF
        public decimal Amount { get; }

        public Money(decimal amount)
        {
            if (amount < 0)
                throw new Exception("Money cannot be negative");

            Amount = amount;
        }

        public Money Add(Money other)
            => new Money(this.Amount + other.Amount);

        public Money Subtract(Money other)
        {
            if (other.Amount > this.Amount)
                throw new Exception("Invalid subtraction");

            return new Money(this.Amount - other.Amount);
        }

        public override bool Equals(object obj)
            => obj is Money money && money.Amount == Amount;

        public override int GetHashCode()
            => Amount.GetHashCode();

        public static Money Zero => new Money(0);

        public static Money operator +(Money left, Money right)
        {
            return left.Add(right);
        }

        public static Money operator -(Money left, Money right)
        {
            return left.Subtract(right);
        }
    }
}
