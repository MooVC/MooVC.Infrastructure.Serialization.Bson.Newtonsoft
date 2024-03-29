﻿namespace MooVC.Infrastructure.Serialization.Bson.Newtonsoft;

using System;
using System.Collections;
using System.IO;
using System.Text;
using global::Newtonsoft.Json;
using global::Newtonsoft.Json.Bson;
using MooVC.Compression;
using MooVC.Serialization;
using static System.String;
using static MooVC.Infrastructure.Serialization.Bson.Newtonsoft.Resources;

public sealed class Serializer
    : SynchronousSerializer
{
    public static readonly Encoding DefaultEncoding = Encoding.UTF8;
    private readonly Lazy<JsonSerializer> json;

    public Serializer(
        ICompressor? compressor = default,
        Encoding? encoding = default,
        DateTimeKind kind = DateTimeKind.Unspecified,
        JsonSerializerSettings? settings = default)
        : base(compressor: compressor)
    {
        Encoding = encoding ?? DefaultEncoding;
        json = new Lazy<JsonSerializer>(() => JsonSerializer.CreateDefault(settings));
        Kind = kind;
    }

    public Encoding Encoding { get; }

    public JsonSerializer Json => json.Value;

    public DateTimeKind Kind { get; }

    protected override T PerformDeserialize<T>(Stream source)
    {
        Type type = typeof(T);
        bool readRootValueAsArray = typeof(IEnumerable).IsAssignableFrom(type) || type.IsArray;

        using var binary = new BinaryReader(source, Encoding, true);
        using var reader = new BsonDataReader(source, readRootValueAsArray, Kind);

        T? result = Json.Deserialize<T>(reader);

        if (result is null)
        {
            throw new JsonSerializationException(Format(SerializerDeserializeFailure, typeof(T).AssemblyQualifiedName));
        }

        return result;
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