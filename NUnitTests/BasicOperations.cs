using NUnit.Framework;
using SharpCalculatorLib;
using System;
using SharpCalculatorLib.Exceptions;

namespace NUnitTests
{
    public class BasicOperations
    {
        private Calculator _calc;
        private string _result;

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
            Assert.IsTrue(_calc.ProcessExpression("exact(1/4)") == "0.25");
            Assert.IsTrue(_calc.ProcessExpression("exact(1441 / 48)") == "30.02083396911621");
        }
    }
}