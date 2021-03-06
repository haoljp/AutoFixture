﻿using System;
using System.Reflection;
using AutoFixture.Kernel;
using TestTypeFoundation;
using Xunit;

namespace AutoFixtureUnitTest.Kernel
{
    public class PropertySpecificationTest
    {
        [Fact]
        public void SutIsRequestSpecification()
        {
            // Fixture setup
            // Exercise system
            var sut = new PropertySpecification(typeof(object), "someName");
            // Verify outcome
            Assert.IsAssignableFrom<IRequestSpecification>(sut);
            // Teardown
        }

        [Fact]
        [Obsolete]
        public void InitializeWithPropertyTypeAndNameShouldSetCorrespondingProperties()
        {
            // Fixture setup
            var type = typeof(string);
            var name = "someName";
            // Exercise system
            var sut = new PropertySpecification(type, name);
            // Verify outcome
#pragma warning disable 618
            Assert.Equal(type, sut.TargetType);
            Assert.Equal(name, sut.TargetName);
#pragma warning restore 618
            // Teardown
        }

        [Fact]
        public void InitializeWithNullPropertyTypeShouldThrowArgumentNullException()
        {
            // Fixture setup
            // Exercise system and verify outcome
            Assert.Throws<ArgumentNullException>(() =>
                new PropertySpecification(null, "someName"));
            // Teardown
        }

        [Fact]
        public void InitializeWithNullPropertyNameShouldThrowArgumentNullException()
        {
            // Fixture setup
            // Exercise system and verify outcome
            Assert.Throws<ArgumentNullException>(() =>
                new PropertySpecification(typeof(object), null));
            // Teardown
        }

        [Fact]
        public void InitializeWithNullTargetShouldThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(
                () => new PropertySpecification(null));
        }

        [Fact]
        public void IsSatisfiedByWithNullRequestShouldThrowArgumentNullException()
        {
            // Fixture setup
            var sut = new PropertySpecification(typeof(object), "someName");
            // Exercise system and verify outcome
            Assert.Throws<ArgumentNullException>(() =>
                sut.IsSatisfiedBy(null));
            // Teardown
        }

        [Theory]
        [InlineData("Property", "Property", true)]
        [InlineData("someName", "Property", false)]
        [InlineData("property", "Property", false)]
        public void IsSatisfiedByWithRequestForPropertyShouldReturnCorrectResult(
            string propertyName,
            string requestedName,
            bool expectedResult)
        {
            // Fixture setup
            var sut = new PropertySpecification(typeof(object), propertyName);
            var request = typeof(PropertyHolder<object>).GetProperty(requestedName);
            // Exercise system
            var result = sut.IsSatisfiedBy(request);
            // Verify outcome
            Assert.Equal(expectedResult, result);
            // Teardown
        }

        [Fact]
        public void IsSatisfiedByWithRequestForReadOnlyPropertyWithSameNameShouldReturnTrue()
        {
            // Fixture setup
            var propertyName = "Property";
            var requestedName = "Property";
            var sut = new PropertySpecification(typeof(object), propertyName);
            var request = typeof(ReadOnlyPropertyHolder<object>).GetProperty(requestedName);
            // Exercise system
            var result = sut.IsSatisfiedBy(request);
            // Verify outcome
            Assert.True(result);
            // Teardown
        }

        [Fact]
        public void IsSatisfiedByWithRequestForPropertyWithSameNameAndIncompatibleTypeShouldReturnFalse()
        {
            // Fixture setup
            var propertyName = "Property";
            var requestedName = "Property";
            var sut = new PropertySpecification(typeof(object), propertyName);
            var request = typeof(PropertyHolder<int>).GetProperty(requestedName);
            // Exercise system
            var result = sut.IsSatisfiedBy(request);
            // Verify outcome
            Assert.False(result);
            // Teardown
        }

        [Fact]
        public void IsSatisfiedByWithRequestForMemberOtherThanPropertyWithSameNameShouldReturnFalse()
        {
            // Fixture setup
            var propertyName = "Field";
            var requestedName = "Field";
            var sut = new PropertySpecification(typeof(object), propertyName);
            var request = typeof(FieldHolder<object>).GetField(requestedName);
            // Exercise system
            var result = sut.IsSatisfiedBy(request);
            // Verify outcome
            Assert.False(result);
            // Teardown
        }

        [Theory]
        [InlineData("aString")]
        [InlineData(9)]
        [InlineData(true)]
        [InlineData(typeof(object))]
        public void IsSatisfiedByWithInvalidRequestShouldReturnFalse(object request)
        {
            // Fixture setup
            var sut = new PropertySpecification(typeof(object), "someName");
            // Exercise system
            var result = sut.IsSatisfiedBy(request);
            // Verify outcome
            Assert.False(result);
            // Teardown
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void IsSatisfiedByReturnsCorrectResultAccordingToTarget(
            bool expected)
        {
            var prop = typeof(string).GetProperty("Length");
            var target = new DelegatingCriterion<PropertyInfo>
            {
                OnEquals = p =>
                {
                    Assert.Equal(prop, p);
                    return expected;
                }
            };
            var sut = new PropertySpecification(target);

            var actual = sut.IsSatisfiedBy(prop);

            Assert.Equal(expected, actual);
        }
    }
}
