using FluentAssertions;
using Xunit;
using Zinc.DeveloperCenter.Domain;

namespace Zinc.DeveloperCenter.UnitTests
{
    public class ScottSaysItShould
    {
        [Fact]
        public void ReturnDescriptionValue()
        {
            Domain.Services.GitHub.FileFormat.Raw.ToDescription().Should().Be("application/vnd.github.VERSION.raw");
        }
    }
}
