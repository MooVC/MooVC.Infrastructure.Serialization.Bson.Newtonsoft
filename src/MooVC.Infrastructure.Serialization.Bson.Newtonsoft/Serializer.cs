namespace MooVC.Infrastructure.Serialization.Bson.Newtonsoft
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using global::Newtonsoft.Json;
    using global::Newtonsoft.Json.Bson;
    using MooVC.Serialization;
    using static System.String;
    using static MooVC.Infrastructure.Serialization.Bson.Newtonsoft.Resources;

    public sealed class Serializer
        : SynchronousSerializer
    {
        public static readonly Encoding DefaultEncoding = new UTF8Encoding(false, true);
        private readonly Lazy<JsonSerializer> json;

        public Serializer(
            Encoding? encoding = default,
            DateTimeKind kind = DateTimeKind.Unspecified,
            JsonSerializerSettings? settings = default)
        {
            Encoding = encoding ?? DefaultEncoding;
            json = new Lazy<JsonSerializer>(() => JsonSerializer.CreateDefault(settings));
            Kind = kind;
        }

        public Encoding Encoding { get; }

        public JsonSerializer Json => json.Value;

        public DateTimeKind Kind { get; }

        protected override T PerformDeserialize<T>(IEnumerable<byte> data)
        {
            using var stream = new MemoryStream(data.ToArray());

            return PerformDeserialize<T>(stream);
        }

        protected override T PerformDeserialize<T>(Stream source)
        {
            using var binary = new BinaryReader(source, Encoding, true);
            using var reader = new BsonDataReader(source)
            {
                DateTimeKindHandling = Kind,
            };

            T? result = Json.Deserialize<T>(reader);

            if (result is null)
            {
                throw new JsonSerializationException(Format(
                    SerializerDeserializeFailure,
                    typeof(T).AssemblyQualifiedName));
            }

            return result;
        }

        protected override IEnumerable<byte> PerformSerialize<T>(T instance)
        {
            using var stream = new MemoryStream();

            PerformSerialize(instance, stream);

            return stream.ToArray();
        }

        protected override void PerformSerialize<T>(T instance, Stream target)
        {
            using var binary = new BinaryWriter(target, Encoding, true);
            using var writer = new BsonDataWriter(binary)
            {
                DateTimeKindHandling = Kind,
            };

            Json.Serialize(writer, instance);

            writer.Flush();
            binary.Flush();
        }
    }
}