﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Utilities.Functional;
using System.Linq;
using System.Reactive;

namespace Utilities.Tests
{
    [TestClass]
    public class TestOption
    {

        [TestMethod]
        public void TestAsOptionSome()
        {
            var option = Option<Unit>.AsOption(Unit.Default);

            Assert.AreNotEqual(Option<Unit>.None, option);
        }

        [TestMethod]
        public void TestAsOptionNone()
        {
            var option = Option<string>.AsOption(null);

            Assert.AreEqual(Option<string>.None, option);
        }

        [TestMethod]
        public void TestSelectSome()
        {
            var val = "Test";
            var option = Option<string>.AsOption(val)
                                       .Select(str => str.Length);

            Assert.AreEqual(val.Length, option.GetUnsafe);
        }

        [TestMethod]
        public void TestSelectNone()
        {
            var option = Option<string>.None
                                       .Select(str => str.Length);

            Assert.AreEqual(Option<int>.None, option);
        }

        [TestMethod]
        public void TestSelectManySome()
        {
            var val = "Test";
            var option = Option<string>.AsOption(val)
                                       .SelectMany(str => Option<int>.AsOption(str.Length));

            Assert.AreEqual(val.Length, option.GetUnsafe);
        }

        [TestMethod]
        public void TestSelectManyNone()
        {
            var val = "Test";
            var option = Option<string>.AsOption(val)
                                       .SelectMany(str => Option<string>.None);

            Assert.AreEqual(Option<string>.None, option);
        }

        [TestMethod]
        public void TestWhereSomePositive()
        {
            var val = "Test";
            var option = Option<string>.AsOption(val)
                                       .Where(str => str.StartsWith(val));

            Assert.AreNotEqual(Option<string>.None, option);
            Assert.AreEqual(val, option.GetUnsafe);
        }

        [TestMethod]
        public void TestWhereSomeNegative()
        {
            var val = "Test";
            var option = Option<string>.AsOption(val)
                                       .Where(str => str.StartsWith("Wrong"));

            Assert.AreEqual(Option<string>.None, option);
        }

        [TestMethod]
        public void TestWhereNone()
        {
            var option = Option<string>.None
                                       .Where(str => str.StartsWith("Wrong"));

            Assert.AreEqual(Option<string>.None, option);
        }

        [TestMethod]
        public void TestIterSome()
        {
            string valueToSet = "";

            var val = "Test";
            Option<string>.AsOption(val)
                          .Iter(i => valueToSet = i);

            Assert.AreEqual(val, valueToSet);
        }

        [TestMethod]
        public void TestIterNone()
        {
            var val = "";
            string valueToSet = val;
            Option<string>.None
                          .Iter(i => valueToSet = "Test");

            Assert.AreEqual(val, valueToSet);
        }

        [TestMethod]
        public void TestTrySome()
        {
            var number = 12;
            var option = Option<int>.Try(() => int.Parse("" + number));

            Assert.AreNotEqual(Option<string>.None, option);
            Assert.AreEqual(number, option.GetUnsafe);
        }

        [TestMethod]
        public void TestTryNone()
        {
            var notNumber = "strange number";
            var option = Option<int>.Try(() => int.Parse(notNumber));

            Assert.AreEqual(Option<int>.None, option);
        }

        [TestMethod]
        public void TestMatchSome()
        {
            var val = "Test";
            var result = Option<string>.AsOption(val)
                                       .Match(Fp.Id, () => string.Empty);

            Assert.AreEqual(val, result);
        }

        [TestMethod]
        public void TestMatchNone()
        {
            var result = Option<string>.None
                                       .Match(Fp.Id, () => string.Empty);

            Assert.AreEqual(string.Empty, result);
        }

        [TestMethod]
        public void TestOrDefaultSome()
        {
            var val = "Test";
            var option = Option<string>.AsOption(val)
                                       .OrDefault(() => "Something");

            Assert.AreEqual(val, option);
        }

        [TestMethod]
        public void TestOrDefaultNone()
        {
            var val = "Test";
            var option = Option<string>.None
                                       .OrDefault(() => val);

            Assert.AreEqual(val, option);
        }

        [TestMethod]
        public void TestOfTypeSomePositive()
        {
            var option = Option<string>.AsOption("Test")
                                       .OfType<ICloneable>();

            Assert.AreNotEqual(Option<ICloneable>.None, option);
        }

        [TestMethod]
        public void TestOfTypeSomeNegative()
        {
            var option = Option<string>.AsOption("Test")
                                       .OfType<int>();

            Assert.AreEqual(Option<int>.None, option);
        }

        [TestMethod]
        public void TestOfTypeNone()
        {
            var option = Option<string>.None
                                       .OfType<ICloneable>();

            Assert.AreEqual(Option<ICloneable>.None, option);
        }

        [TestMethod]
        public void TestOrOptionSome()
        {
            var val = "Test";
            var option = Option<string>.AsOption(val)
                                       .Or(() => Option<string>.AsOption("Something"));

            Assert.AreNotEqual(Option<ICloneable>.None, option);
            Assert.AreEqual(val, option.GetUnsafe);
        }

        [TestMethod]
        public void TestOrOptionNone()
        {
            var val = "Test";
            var option = Option<string>.None
                                       .Or(() => Option<string>.AsOption(val));

            Assert.AreNotEqual(Option<ICloneable>.None, option);
            Assert.AreEqual(val, option.GetUnsafe);
        }

        [TestMethod]
        public void TestToEnumerableSome()
        {
            var sequence = Option<string>.AsOption("Test")
                                       .ToEnumerable();

            Assert.AreEqual(1, sequence.Count());
        }

        [TestMethod]
        public void TestToEnumerableNone()
        {
            var sequence = Option<string>.None
                                         .ToEnumerable();

            Assert.AreEqual(0, sequence.Count());
        }

        [TestMethod]
        public void TestTryNeverReturnsSomeWithNull()
        {
            var op = Option<object>.Try(() => (object)null);

            Assert.IsFalse(op.IsSome);
        }
    }
}