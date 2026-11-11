
using VolumeAnchor.Core;
using Xunit;

namespace VolumeAnchor.UnitTests;

public sealed class AppIdTests
{
    [Theory]
    [InlineData(@"C:\Program Files\Example\app.exe")]
    [InlineData(@"D:\Games\Example\game.exe")]
    [InlineData(@"\\server\share\app.exe")]
    public void ValidExecutablePathsAreAccepted(string path)
    {
        var identity = AppId.FromExecutablePath(path);

        Assert.Equal(AppIdKind.ExecutablePath, identity.Kind);
        Assert.Equal(path, identity.Value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("app.exe")]
    [InlineData(@"Apps\app.exe")]
    [InlineData(@"C:Apps\app.exe")]
    [InlineData(@"C:\Apps\..\app.exe")]
    [InlineData(@"\\?\C:\Apps\app.exe")]
    [InlineData(@"C:\Apps\")]
    public void InvalidExecutablePathsAreRejected(string? path)
    {
        Assert.Throws<ArgumentException>(() => AppId.FromExecutablePath(path));
    }

    [Fact]
    public void IdenticalExecutablePathsMatch()
    {
        var first = AppId.FromExecutablePath(@"C:\Apps\app.exe");
        var second = AppId.FromExecutablePath(@"C:\Apps\app.exe");

        Assert.Equal(first, second);
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }

    [Fact]
    public void DifferentExecutablePathsDoNotMatch()
    {
        var first = AppId.FromExecutablePath(@"C:\Apps\app.exe");
        var second = AppId.FromExecutablePath(@"D:\Apps\app.exe");

        Assert.NotEqual(first, second);
    }

    [Fact]
    public void ExecutablePathCaseDifferencesDoNotMatch()
    {
        var first = AppId.FromExecutablePath(@"C:\Apps\app.exe");
        var second = AppId.FromExecutablePath(@"C:\Apps\App.exe");

        Assert.NotEqual(first, second);
    }

    [Fact]
    public void IdenticalPackagedApplicationsMatchIgnoringCase()
    {
        var first = AppId.FromAppUserModelId("Example_8wekyb3d8bbwe!App");
        var second = AppId.FromAppUserModelId("example_8wekyb3d8bbwe!app");

        Assert.Equal(first, second);
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }

    [Fact]
    public void DifferentApplicationsInSamePackageDoNotMatch()
    {
        var first = AppId.FromAppUserModelId("Example_8wekyb3d8bbwe!Player");
        var second = AppId.FromAppUserModelId("Example_8wekyb3d8bbwe!Editor");

        Assert.NotEqual(first, second);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("ExamplePackage")]
    [InlineData("ExamplePackage!")]
    [InlineData("!Application")]
    [InlineData("Package!Application!Other")]
    [InlineData(" Package!Application ")]
    public void InvalidAppUserModelIdsAreRejected(string? value)
    {
        Assert.Throws<ArgumentException>(() => AppId.FromAppUserModelId(value));
    }

    [Fact]
    public void DifferentIdentityKindsDoNotMatch()
    {
        var executable = AppId.FromExecutablePath(@"C:\Apps\app.exe");
        var packaged = AppId.FromAppUserModelId("Example_8wekyb3d8bbwe!App");

        Assert.NotEqual(executable, packaged);
    }
}
