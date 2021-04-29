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
                Simplify();
            }
            else
            {
                RoundedValue = numerator;
            }
        }

        public Fraction(double num, double den)
        {
            int afterCommaMaxNum = 0;
            int afterCommaMaxDen = 0;

            string numString = num.ToString().Length >= 8 ? num.ToString()[0..10] : num.ToString();

            string denString = den.ToString().Length >= 8 ? den.ToString()[0..10] : den.ToString();

            if (numString.Contains("."))
            {
                afterCommaMaxNum = numString.Substring(numString.IndexOf(".")).Length - 1;
            }
            if (denString.Contains("."))
            {
                afterCommaMaxDen = denString.Substring(denString.IndexOf(".")).Length - 1;
            }

            int afterCommaMax = Math.Max(afterCommaMaxNum, afterCommaMaxDen);

            Numerator = (int)(num * (Math.Pow(10, afterCommaMax)));
            Denominator = (int)(den * (Math.Pow(10, afterCommaMax)));
            Simplify();
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

        private void Simplify()
        {
            int pgcd = PGCD(Numerator, Denominator);
            if (pgcd > 1)
            {
                Numerator /= pgcd;
                Denominator /= pgcd;
            }

            RoundedValue = (double)Numerator / (double)Denominator;
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

        public static Fraction Pow(Fraction a, Fraction b) => new Fraction(Math.Pow(b.Numerator, a.RoundedValue), Math.Pow(b.Denominator, a.RoundedValue));

        public override string ToString()
        {
            if (this.exact)
            {
                return this.RoundedValue.ToString();
            }

            return (this.Denominator == 1 ? this.Numerator.ToString() : $"{this.Numerator}/{this.Denominator}");
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