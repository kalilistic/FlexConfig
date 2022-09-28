using System;
using System.Collections.Generic;
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
        public void GetShouldThrowExceptionIfInvalidCast()
        {
            const string key = "isEnabled";
            var config = new Configuration(TestFilePath);
            config.Set(key, false);
            config.Set(key, "UpdatedValue");
            Assert.Throws<InvalidCastException>(() => config.Get<bool>(key).Reference);
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
        public void RemoveShouldRemoveValue()
        {
            const string key = "isEnabled";
            var config = new Configuration(TestFilePath);
            config.Set(key, false);
            config.Remove(key);
            Assert.Throws<KeyNotFoundException>(() => config.Get<bool>(key));
        }

        [Fact]
        public void GetImmediateShouldRetrievePrimitiveValue()
        {
            const string key = "isEnabled";
            var config = new Configuration(TestFilePath);
            config.Set(key, true);
            var flex = config.Get(key);
            Assert.True(flex);
        }

        [Fact]
        public void GetImmediateShouldRetrieveUserDefinedObjectValue()
        {
            const string key = "myObject";
            var obj = new UserDefinedObject
            {
                Name = "InitialValue",
            };
            var config = new Configuration(TestFilePath);
            config.Set(key, obj);
            Assert.Equal("InitialValue", config.Get(key).Name);
        }

        [Fact]
        public void GetShouldRetrieveUserDefinedObjectValue()
        {
            const string key = "myObject";
            var obj = new UserDefinedObject
            {
                Name = "InitialValue",
            };
            var config = new Configuration(TestFilePath);
            config.Set(key, obj);
            var flex = config.Get<UserDefinedObject>(key);
            ref var flexRef = ref flex.Reference;
            flexRef.Name = "UpdatedValue";
            config[key] = flex;
            Assert.Equal("UpdatedValue", config.Get<UserDefinedObject>(key).Reference.Name);
        }

        [Fact]
        public void SetShouldSupportFlexObjects()
        {
            const string key = "myObject";
            var obj = new UserDefinedObject
            {
                Name = "InitialValue",
            };
            var config = new Configuration(TestFilePath);
            var flexIn = Configuration.Create(obj);
            config.Set(key, flexIn);
            Assert.Equal("InitialValue", config.Get<UserDefinedObject>(key).Reference.Name);
        }

        [Fact]
        public void LoadShouldRetrievePrimitiveValueFromFile()
        {
            const string key = "isEnabled";
            var config = new Configuration(TestFilePath);
            config.Set(key, true);
            config.Save();
            config = new Configuration(TestFilePath);
            config.Load();
            Assert.True(config.Get<bool>(key).Reference);
            File.Delete(TestFilePath);
        }

        [Fact]
        public void LoadShouldRetrieveUserDefinedObjectValue()
        {
            const string key = "myObject";
            var obj = new UserDefinedObject
            {
                Name = "InitialValue",
            };
            var config = new Configuration(TestFilePath);
            config.Set(key, obj);
            config.Save();
            config = new Configuration(TestFilePath);
            config.Load();
            Assert.Equal("InitialValue", config.Get<UserDefinedObject>(key).Reference.Name);
            File.Delete(TestFilePath);
        }

        [Fact]
        public void GetContainsShouldReturnTrueIfContained()
        {
            const string key = "isEnabled";
            var config = new Configuration(TestFilePath);
            config.Set(key, true);
            Assert.True(config.ContainsKey(key));
        }

        [Fact]
        public void GetContainsShouldReturnFalseIfNotContained()
        {
            const string key = "isEnabled";
            var config = new Configuration(TestFilePath);
            Assert.False(config.ContainsKey(key));
        }

        private class UserDefinedObject
        {
            public string Name { get; set; } = null!;
        }
    }
}
