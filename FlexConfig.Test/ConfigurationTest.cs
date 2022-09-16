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
        public void GetShouldRetrievePrimitiveValue()
        {
            const string key = "isEnabled";
            var config = new Configuration(TestFilePath);
            config.Set(key, false);
            var flex = config.Get<bool>(key);
            ref var flexRef = ref flex.Reference;
            flexRef = !flexRef;
            config[key] = flex;
            Assert.True(config.Get<bool>(key));
        }

        [Fact]
        public void GetShouldRetrieveUserDefinedObjectValue()
        {
            const string key = "myObject";
            var obj = new UserDefinedObject
            {
                Name = "Hello!",
            };
            var config = new Configuration(TestFilePath);
            config.Set(key, obj);
            var flex = config.Get<UserDefinedObject>(key);
            ref var flexRef = ref flex.Reference;
            flexRef.Name = "Goodbye!";
            config[key] = flex;
            Assert.Equal("Goodbye!", config.Get<UserDefinedObject>(key).Reference.Name);
        }

        private class UserDefinedObject
        {
            public string Name { get; set; } = null!;
        }
    }
}
