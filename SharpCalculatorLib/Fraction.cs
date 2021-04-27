using System;

namespace SharpCalculatorLib
{
    public class Fraction
    {
        public int Numerator;
        public int Denominator;

        public Fraction(int numerator, int denominator = 1)
        {
            Numerator = numerator;
            Denominator = denominator;
            int pgcd = PGCD(Numerator, Denominator);
            if (pgcd > 1)
            {
                Numerator /= pgcd;
                Denominator /= pgcd;
            }
        }

        public static Fraction Parse(string expression)
        {
            int num;
            int den;

            if (expression.Contains("/"))
            {
                string[] expressions = expression.Split("/");
                num = int.Parse(expressions[0]);
                den = int.Parse(expressions[1]);
            }
            else
            {
                num = int.Parse(expression);
                den = 1;
            }

            //Logger.DebugLog($"{num} / {den}");

            return new Fraction(num, den);
        }

        public static Fraction operator +(Fraction a, Fraction b)
        {
            return new Fraction(a.Numerator * b.Denominator + b.Numerator * a.Denominator, a.Denominator * b.Denominator);
        }

        public static Fraction operator +(Fraction a) => a;

        public static Fraction operator -(Fraction a) => new Fraction(-a.Numerator, a.Denominator);

        public static Fraction operator -(Fraction a, Fraction b) => a + (-b);

        public static Fraction operator *(Fraction a, Fraction b)
        {
            return new Fraction(a.Numerator * b.Numerator, a.Denominator * b.Denominator);
        }

        public static Fraction operator /(Fraction a, Fraction b)
        {
            if (b.Numerator == 0)
            {
                throw new DivideByZeroException();
            }
            return new Fraction(a.Numerator * b.Denominator, a.Denominator * b.Numerator);
        }

        public override string ToString()
        {
            return (this.Denominator == 1 ? this.Numerator.ToString() : $"{this.Numerator} / {this.Denominator}");
        }

        private int PGCD(int a, int b)
        {
            if (b == 0) { return a; }
            if (b == 1) { return 1; }
            if (a % b == 0) { return b; }

            int r = a % b;

            while (a % b != 0)
            {
                r = a % b;
                a = b;
                b = r;
            }
            return r;
        }
    }
}