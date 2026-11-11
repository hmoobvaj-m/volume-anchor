using VolumeAnchor.Core;
using Xunit;

namespace VolumeAnchor.UnitTests;

public sealed class VolumeLevelTests
{
    [Theory]
    [InlineData(0f)]
    [InlineData(0.25f)]
    [InlineData(0.5f)]
    [InlineData(0.75f)]
    [InlineData(1f)]
    public void ConstructorAcceptsValidValues(float value)
    {
        var volume = new VolumeLevel(value);

        Assert.Equal(value, volume.Value);
    }

    [Theory]
    [InlineData(-0.01f)]
    [InlineData(1.01f)]
    [InlineData(float.NaN)]
    [InlineData(float.PositiveInfinity)]
    [InlineData(float.NegativeInfinity)]
    public void ConstructorRejectsInvalidValues(float value)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new VolumeLevel(value));
    }

    [Fact]
    public void DefaultValueRepresentsZeroVolume()
    {
        var volume = default(VolumeLevel);

        Assert.Equal(0f, volume.Value);
    }

    [Fact]
    public void EqualValuesAreEqual()
    {
        var first = new VolumeLevel(0.5f);
        var second = new VolumeLevel(0.5f);

        Assert.Equal(first, second);
    }

    [Fact]
    public void DifferentValuesAreNotEqual()
    {
        var first = new VolumeLevel(0.25f);
        var second = new VolumeLevel(0.75f);

        Assert.NotEqual(first, second);
    }
}
