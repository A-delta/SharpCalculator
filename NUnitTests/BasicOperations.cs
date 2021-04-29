using NUnit.Framework;
using SharpCalculatorLib;
using System;
using SharpCalculatorLib.Exceptions;

namespace NUnitTests
{
    public class BasicOperations
    {
        private Calculator _calc;

        [SetUp]
        public void Setup()
        {
            _calc = new Calculator(false);
        }

        [Test]
        public void Integers()
        {
            Assert.IsTrue(_calc.ProcessExpression("1-1") == "0");
            Assert.IsTrue(_calc.ProcessExpression("27+3") == "30");
            Assert.IsTrue(_calc.ProcessExpression("28/4") == "7");
            Assert.IsTrue(_calc.ProcessExpression("3*4") == "12");
        }

        [Test]
        public void ExactValues()
        {
            Assert.IsTrue(_calc.ProcessExpression("dec(1/4)") == "0.25");
            Assert.IsTrue(_calc.ProcessExpression("dec(1/4) * 4") == "1");
        }

        [Test]
        public void Fractions()
        {
            Assert.IsTrue(_calc.ProcessExpression("1/2 + 2/2") == "3/2");
            Assert.IsTrue(_calc.ProcessExpression("1/4/4") == "1/16");
            Assert.IsTrue(_calc.ProcessExpression("1/2 - 8/2") == "-7/2");
            Assert.IsTrue(_calc.ProcessExpression("1/4 * 1/4") == "1/16");
            Assert.IsTrue(_calc.ProcessExpression("1/(4/4)") == "1");
            Assert.IsTrue(_calc.ProcessExpression("(1/4)/4") == "1/16");
        }

        [Test]
        public void Simplify()
        {
            Assert.IsTrue(_calc.ProcessExpression("2/4") == "1/2");
        }
    }
}