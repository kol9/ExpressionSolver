using System;
using System.Collections.Generic;
using System.Globalization;
using ExpressionParserLib.Parser;
using NUnit.Framework;
using TestPair = System.Tuple<string, System.Func<double, double, double, double>>;
using StringPair = System.Tuple<string, string>;

namespace ExpressionParserLib.Tests
{
    public class ParserTests
    {
        private List<Tuple<string, string>> _badTests;


        private List<TestPair> _goodTests;
        private ExpressionParser _parser;
        private Random _random;

        [SetUp]
        public void Setup()
        {
            _random = new Random();
            _parser = new ExpressionParser();
            _goodTests = new List<TestPair>
            {
                new TestPair("228", (x, y, z) => 228),
                new TestPair("x+y", (x, y, z) => x + y),
                new TestPair("x-y", (x, y, z) => x - y),
                new TestPair("x*y", (x, y, z) => x * y),
                new TestPair("x/y", (x, y, z) => x / y),
                new TestPair("0/x", (x, y, z) => 0 / x),
                new TestPair("x/0", (x, y, z) => x / 0),
                new TestPair("x /    -    y", (x, y, z) => x / -y),
                new TestPair("x* \t \n  y", (x, y, z) => x * y),
                new TestPair("--x--y--z", (x, y, z) => x + y + z),
                new TestPair("x*z  +(-4-y   )/z", (x, y, z) => x * z + (-4 - y) / z),
                new TestPair("-(-(-\t\t-5 + 23   *x*y) + 1 * z) -(((-11)))",
                    (x, y, z) => -(-(5 + 23 * x * y) + z) + 11),
                new TestPair("x--y--z", (x, y, z) => x + y + z),
                new TestPair("x/y/z", (x, y, z) => x / y / z),
                new TestPair("(((((x + y + (-10*-z))))))", (x, y, z) => x + y + -10 * -z)
            };

            _badTests = new List<StringPair>
            {
                new StringPair("No first argument", " + 2"),
                new StringPair("Missing argument", "- * 2"),
                new StringPair("No second argument", " 2 + "),
                new StringPair("No middle argument", " 2 +  + 3"),
                new StringPair("No middle argument", " 2 + (2 +  + 4) + 3"),
                new StringPair("No first argument", " 1 + (* 2 * 4) + 2"),
                new StringPair("No last argument", " 1 + (5 * 2 * ) + 2"),
                new StringPair("No last argument", " 1 + (5 * 2 * ) + 2"),
                new StringPair("No open parenthesis", "2*2)"),
                new StringPair("No close parenthesis", "(2*2"),
                new StringPair("Unexpected symbol in the beginning", "&2*2"),
                new StringPair("Unexpected symbol in the end", "2*2&"),
                new StringPair("Unexpected symbol in the middle", "2*^2"),
                new StringPair("Single plus", "+"),
                new StringPair("Single minus", "-"),
                new StringPair("Single unexpected symbol", "g"),
                new StringPair("Empty expression", "(())"),
                new StringPair("Wrong number", "2.,.,+4")
            };
        }


        [Test]
        public void CorrectTests()
        {
            for (var i = 0; i < 100; ++i)
                foreach (var test in _goodTests)
                {
                    var x = _random.NextDouble() + _random.Next();
                    var y = _random.NextDouble() + _random.Next();
                    var z = _random.NextDouble() + _random.Next();
                    var testString = new string(test.Item1);
                    testString = testString.Replace("x", x.ToString(CultureInfo.InvariantCulture));
                    testString = testString.Replace("y", y.ToString(CultureInfo.InvariantCulture));
                    testString = testString.Replace("z", z.ToString(CultureInfo.InvariantCulture));
                    var (item1, item2) = _parser.Parse(testString);
                    Assert.IsTrue(item2.Count == 0);
                    Assert.AreEqual(test.Item2(x, y, z), item1.Evaluate());
                }
        }

        [Test]
        public void WrongTests()
        {
            foreach (var (item1, item2) in _badTests)
            {
                var (_, exceptions) = _parser.Parse(item2);
                Assert.IsTrue(exceptions.Count != 0);
                Console.WriteLine("TESTING: " + item1);

                foreach (var err in exceptions) Console.WriteLine(err.ToString());
            }
        }
    }
}