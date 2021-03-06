﻿using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GObject.Tests
{
    [TestClass, TestCategory("IntegrationTest")]
    public class ValueTest
    {
        [DataTestMethod]
        [DataRow(5)]
        [DataRow(true)]
        [DataRow("TestString")]
        public void ValueFromDataShouldContainGivenData(object data)
        {
            var v = Value.From(data);
            v.Extract().Should().Be(data);
        }
    }
}
