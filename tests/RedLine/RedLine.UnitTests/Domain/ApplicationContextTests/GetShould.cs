using System;
using FluentAssertions;
using RedLine.Domain;
using RedLine.Domain.Exceptions;
using Xunit;

namespace RedLine.UnitTests.Domain.ApplicationContextTests
{
    [Collection(nameof(UnitTestCollection))]
    public class GetShould
    {
        [Fact]
        public void ReturnExistingValue()
        {
            // Arrange
            // Act
            var result = ApplicationContext.Get("key1");

            // Assert
            result.Should().Be("value1");
        }

        [Fact]
        public void ThrowInvalidConfigurationExceptionForNonExistingValue()
        {
            // Arrange
            // Act
            Action action = () => ApplicationContext.Get("non-existing");

            // Assert
            action.Should().Throw<InvalidConfigurationException>();
        }

        [Fact]
        public void ExpandValuesDelimitedWithPercentCharacters()
        {
            // Arrange
            // Act
            var result = ApplicationContext.Get("key2");

            // Assert
            result.Should().Be("value2: 'value1'");
        }

        [Fact]
        public void RecursivelyExpandValues()
        {
            // Arrange
            // Act
            var result = ApplicationContext.Get("key3");

            // Assert
            result.Should().Be("value3: 'value2: 'value1''");
        }

        [Fact]
        public void ThrowInvalidConfigurationExceptionWhenCircularReferenceDetected()
        {
            // Arrange
            // Act
            Action act = () => ApplicationContext.Get("key5");

            // Assert
            act.Should().Throw<InvalidConfigurationException>();
        }
    }
}
