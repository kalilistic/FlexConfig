using System;
using System.IO;

using Xunit;

namespace FlexConfig.Test
{
    public class ConfigurationTest
    {
        private const string TestFilePath = "test.json";

        [Fact]
        public void ConstructorShouldThrowExceptionIfNullFilePath()
        {
            var exception = Assert.Throws<Exception>(() => new Configuration(null!));
            Assert.Equal("configFilePath is null or empty.", exception.Message);
        }

        [Fact]
        public void ConstructorShouldThrowExceptionIfEmptyFilePath()
        {
            var exception = Assert.Throws<Exception>(() => new Configuration(string.Empty));
            Assert.Equal("configFilePath is null or empty.", exception.Message);
        }

        [Fact]
        public void SaveShouldCreateFile()
        {
            var config = new Configuration(TestFilePath);
            config.Save();
            Assert.True(File.Exists(TestFilePath));
            File.Delete(TestFilePath);
        }

        [Fact]
        public void GetShouldRetrieveValue()
        {
            var c = new Configuration(TestFilePath);
            c.Set<bool>("Enabled", false);
            var t = c.Get<bool>("Enabled");
            Test(ref t.Reference);
            c["Enabled"] = t;
            Assert.True(c.Get<bool>("Enabled"));
        }

        private static void Test(ref bool val)
        {
            val = true;
        }
    }
}
