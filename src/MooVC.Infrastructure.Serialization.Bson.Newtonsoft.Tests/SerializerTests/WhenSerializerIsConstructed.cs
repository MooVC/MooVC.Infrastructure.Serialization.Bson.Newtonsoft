namespace MooVC.Infrastructure.Serialization.Bson.Newtonsoft.SerializerTests
{
    using System;
    using System.Text;
    using global::Newtonsoft.Json;
    using Xunit;

    public sealed class WhenSerializerIsConstructed
    {
        [Fact]
        public void GivenNoSettingsThenADefaultSerializerIsCreated()
        {
            var serializer = new Serializer();
            var settings = new JsonSerializerSettings();

            AssertEqual(
                Serializer.DefaultEncoding,
                DateTimeKind.Unspecified,
                serializer,
                settings);
        }

        [Fact]
        public void GivenAEncodingThenASerializerIsCreatedWithTheEncodingApplied()
        {
            Encoding encoding = Encoding.ASCII;
            var serializer = new Serializer(encoding: encoding);
            var settings = new JsonSerializerSettings();

            AssertEqual(
                encoding,
                DateTimeKind.Unspecified,
                serializer,
                settings);
        }

        [Theory]
        [InlineData(DateTimeKind.Utc)]
        [InlineData(DateTimeKind.Local)]
        public void GivenAKindThenASerializerIsCreatedWithTheKindApplied(DateTimeKind kind)
        {
            var serializer = new Serializer(kind: kind);
            var settings = new JsonSerializerSettings();

            AssertEqual(
                Serializer.DefaultEncoding,
                kind,
                serializer,
                settings);
        }

        [Fact]
        public void GivenSettingsThenASerializerIsCreatedWithTheSettingsApplied()
        {
            var settings = new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                TypeNameHandling = TypeNameHandling.All,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
            };

            var serializer = new Serializer(settings: settings);

            AssertEqual(
                Serializer.DefaultEncoding,
                DateTimeKind.Unspecified,
                serializer,
                settings);
        }

        private static void AssertEqual(
            Encoding encoding,
            DateTimeKind kind,
            Serializer serializer,
            JsonSerializerSettings settings)
        {
            Assert.Equal(encoding, serializer.Encoding);
            Assert.Equal(kind, serializer.Kind);
            Assert.Equal(settings.DateTimeZoneHandling, serializer.Json.DateTimeZoneHandling);

            AssertEqual(settings, serializer);
        }

        private static void AssertEqual(JsonSerializerSettings expected, Serializer serializer)
        {
            Assert.Equal(expected.DefaultValueHandling, serializer.Json.DefaultValueHandling);
            Assert.Equal(expected.NullValueHandling, serializer.Json.NullValueHandling);
            Assert.Equal(expected.ReferenceLoopHandling, serializer.Json.ReferenceLoopHandling);
            Assert.Equal(expected.TypeNameHandling, serializer.Json.TypeNameHandling);
            Assert.Equal(expected.TypeNameAssemblyFormatHandling, serializer.Json.TypeNameAssemblyFormatHandling);
        }
    }
}