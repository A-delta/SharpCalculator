using System;

namespace SharpCalculatorLib
{
    public class Fraction
    {
        public int Numerator;
        public int Denominator;
        public double RoundedValue;
        public bool exact;

        public Fraction(int numerator, int denominator = 1)
        {
            Numerator = numerator;
            Denominator = denominator;
            exact = false;

            if (Denominator != 1)
            {
                int pgcd = PGCD(Numerator, Denominator);
                if (pgcd > 1)
                {
                    Numerator /= pgcd;
                    Denominator /= pgcd;
                }

                RoundedValue = (double)Numerator / (double)Denominator;
            }
            else
            {
                RoundedValue = numerator;
            }
        }

        public Fraction(double roundedValue)
        {
            Numerator = 0;
            Denominator = 0;
            exact = true;

            RoundedValue = roundedValue;
        }

        public static Fraction Parse(string expression, bool wantRounded = false)
        {
            if (expression.Contains("."))
            {
                double value;
                if (expression.Contains("/"))
                {
                    string[] expressions = expression.Split("/");
                    value = double.Parse(expressions[0]) * double.Parse(expressions[1]);
                }
                else
                {
                    value = double.Parse(expression);
                }

                return new Fraction(value);
            }
            else
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

                return wantRounded ? new Fraction((double)num / (double)den) : new Fraction(num, den);
            }
        }

        public static Fraction operator +(Fraction a, Fraction b)
        {
            if (a.exact || b.exact)
            {
                return new Fraction(a.RoundedValue + b.RoundedValue);
            }
            else
            {
                return new Fraction(a.Numerator * b.Denominator + b.Numerator * a.Denominator, a.Denominator * b.Denominator);
            }
        }

        public static Fraction operator +(Fraction a) => a;

        public static Fraction operator -(Fraction a)
        {
            if (a.exact)
            {
                return new Fraction(-a.RoundedValue);
            }
            else
            {
                return new Fraction(-a.Numerator, a.Denominator);
            }
        }

        public static Fraction operator -(Fraction a, Fraction b) => a + (-b);

        public static Fraction operator *(Fraction a, Fraction b)
        {
            Logger.DebugLog(a.exact.ToString());
            Logger.DebugLog(b.exact.ToString());

            //Logger.DebugLog(a.RoundedValue.ToString());
            //Logger.DebugLog(b.RoundedValue.ToString());
            if (a.exact || b.exact)
            {
                return new Fraction(a.RoundedValue * b.RoundedValue);
            }
            else
            {
                return new Fraction(a.Numerator * b.Numerator, a.Denominator * b.Denominator);
            }
        }

        public static Fraction operator /(Fraction a, Fraction b)
        {
            if (a.exact || b.exact)
            {
                return new Fraction(a.RoundedValue / b.RoundedValue);
            }

            if (b.Numerator == 0)
            {
                throw new DivideByZeroException();
            }
            return new Fraction(a.Numerator * b.Denominator, a.Denominator * b.Numerator);
        }

        public override string ToString()
        {
            if (this.exact)
            {
                return this.RoundedValue.ToString();
            }

            return (this.Denominator == 1 ? this.Numerator.ToString() : $"{this.Numerator}/{this.Denominator}");
        }

        //public static implicit operator string(Fraction a) => $"{a.Numerator}/{a.Denominator}";

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