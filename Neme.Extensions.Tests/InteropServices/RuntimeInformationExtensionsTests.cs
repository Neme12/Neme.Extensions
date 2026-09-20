using System.Runtime.InteropServices;
using Neme.Extensions.InteropServices;

namespace Neme.Extensions.Tests.InteropServices;

public sealed class RuntimeInformationExtensionsTests
{
#if NET10_0
    private const int CurrentNetMajor = 10;
    private const int CurrentNetMinor = 0;
#elif NET8_0
    private const int CurrentNetMajor = 8;
    private const int CurrentNetMinor = 0;
#elif NET6_0
    private const int CurrentNetMajor = 6;
    private const int CurrentNetMinor = 0;
#elif NET48
    private const int CurrentFrameworkMajor = 4;
    private const int CurrentFrameworkMinor = 8;
#else
#error Unsupported target framework.
#endif

    public sealed class IsNetVersionOrGreater
    {
#if NET48
        [Fact]
        public void NetFrameworkRuntime_ReturnsFalse()
        {
            Assert.False(RuntimeInformation.IsNetCoreVersionOrGreater(CurrentFrameworkMajor, CurrentFrameworkMinor));
        }
#else
        [Fact]
        public void CurrentVersion_ReturnsTrue()
        {
            Assert.True(RuntimeInformation.IsNetCoreVersionOrGreater(CurrentNetMajor, CurrentNetMinor));
        }

        [Fact]
        public void EarlierMajorVersion_ReturnsTrue()
        {
            Assert.True(RuntimeInformation.IsNetCoreVersionOrGreater(CurrentNetMajor - 1, CurrentNetMinor + 1));
        }

        [Fact]
        public void LaterMajorVersion_ReturnsFalse()
        {
            Assert.False(RuntimeInformation.IsNetCoreVersionOrGreater(CurrentNetMajor + 1, CurrentNetMinor));
        }

        [Fact]
        public void LaterMinorVersion_ReturnsFalse()
        {
            Assert.False(RuntimeInformation.IsNetCoreVersionOrGreater(CurrentNetMajor , CurrentNetMinor + 1));
        }
#endif
    }

    public sealed class IsNetFrameworkVersionOrGreater
    {
#if NET48
        [Fact]
        public void CurrentVersion_ReturnsTrue()
        {
            Assert.True(RuntimeInformation.IsNetFrameworkVersionOrGreater(CurrentFrameworkMajor, CurrentFrameworkMinor));
        }

        [Fact]
        public void EarlierMajorVersion_ReturnsTrue()
        {
            Assert.True(RuntimeInformation.IsNetFrameworkVersionOrGreater(CurrentFrameworkMajor - 1, CurrentFrameworkMinor + 1));
        }

        [Fact]
        public void EarlierMinorVersion_ReturnsTrue()
        {
            Assert.True(RuntimeInformation.IsNetFrameworkVersionOrGreater(CurrentFrameworkMajor, CurrentFrameworkMinor - 1));
        }

        [Fact]
        public void LaterMajorVersion_ReturnsFalse()
        {
            Assert.False(RuntimeInformation.IsNetFrameworkVersionOrGreater(CurrentFrameworkMajor + 1, CurrentFrameworkMinor));
        }

        [Fact]
        public void LaterMinorVersion_ReturnsFalse()
        {
            Assert.False(RuntimeInformation.IsNetFrameworkVersionOrGreater(CurrentFrameworkMajor, CurrentFrameworkMinor + 1));
        }
#else
        [Fact]
        public void NetRuntime_ReturnsFalse()
        {
            Assert.False(RuntimeInformation.IsNetFrameworkVersionOrGreater(4, 8));
        }
#endif
    }
}
