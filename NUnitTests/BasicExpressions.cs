using NUnit.Framework;
using SharpCalculatorLib;
using System;

namespace NUnitTests
{
    public class BasicExpressions
    {
        Calculator _calc;
        string _result;

        [SetUp]
        public void Setup()
        {
            _calc = new Calculator(false);
        }

        [Test]
        public void Calculate()
        {
            _result = _calc.ProcessExpression("2*4-4");
            Assert.IsTrue(_result == "4");
        }

        [Test]
        public void Functions()
        {
            _result = _calc.ProcessExpression("sqrt(4)");
            Assert.IsTrue(_result == "2", "Basic");

            _result = _calc.ProcessExpression("sqrt(sqrt(16))");
            Assert.IsTrue(_result == "2", "Function in function");



            _result = _calc.ProcessExpression("max(1, 2)");
            Assert.IsTrue(_result == "2", "Multiple arguments");

            _result = _calc.ProcessExpression("max(sqrt(4), sqrt(2))");
            Assert.IsTrue(_result == "2", "Multiple args and function in function");

            Assert.Throws<ArgumentException>(() => _calc.ProcessExpression("sqrt(4,4)"));

            Assert.Throws<ArgumentException>(() => _calc.ProcessExpression("max(sqrt(4),min(4, 2), add(1, 1))"));

        }

        [Test]
        public void Variables()
        {
            _result = _calc.ProcessExpression("a = 4");
            Assert.IsTrue(_result == "4", "Assignment");

            _result = _calc.ProcessExpression("a");
            Assert.IsTrue(_result == "4", "Get");


            _result = _calc.ProcessExpression("b = a + 4");
            Assert.IsTrue(_result == "8", "var of var ?");

            _result = _calc.ProcessExpression("b");
            Assert.IsTrue(_result == "8", "get var of var ?");

            _result = _calc.ProcessExpression("a = b + 4");
            Assert.IsTrue(_result == "12", "get var of var of var ? ._. ");


            _result = _calc.ProcessExpression("a = sqrt(4)");
            Assert.IsTrue(_result == "2", "caused a bug");


            _result = _calc.ProcessExpression("c = a == 4*2-6");
            Assert.IsTrue(_result == "True", "was a priority problem");

            _result = _calc.ProcessExpression("c");
            Assert.IsTrue(_result == "True", "Get bool value");




            Assert.Throws<ArgumentException>(() => _calc.ProcessExpression("x"));

            Assert.Throws<ArgumentException>(() => _calc.ProcessExpression("a = 4 = 8"));

            Assert.Throws<NotSupportedException>(() => _calc.ProcessExpression("a*/*- = 8"));

            Assert.Throws<NotSupportedException>(() => _calc.ProcessExpression("a6 = 8"));
            

        }

        [Test]
        public void ImplicitProducts()
        {
            _result = _calc.ProcessExpression("2(4)");
            Assert.IsTrue(_result == "8", "Implicit products in a(b) form is false");

            _result = _calc.ProcessExpression("2(4)");
            Assert.IsTrue(_result == "8", "Implicit products in (a)b form is false");

            _result = _calc.ProcessExpression("(4)(2)");
            Assert.IsTrue(_result == "8", "Implicit products in (a)(b) form is false");

            _result = _calc.ProcessExpression("((((4))))((((2))))");
            Assert.IsTrue(_result == "8", "Implicit products in (abusive) (((a)))(((b))) form is false");



            _result = _calc.ProcessExpression("sqrt(16)2");
            Assert.IsTrue(_result == "8", "Implicit products in fcn()a form is false");

            _result = _calc.ProcessExpression("2sqrt(16)");
            Assert.IsTrue(_result == "8", "Implicit products in afcn() form is false");

            _result = _calc.ProcessExpression("sqrt(16)(2)");
            Assert.IsTrue(_result == "8", "Implicit products in fcn()(a) form is false");

            _result = _calc.ProcessExpression("(2)sqrt(16)");
            Assert.IsTrue(_result == "8", "Implicit products in (a)fcn() form is false");
        }


    }
}