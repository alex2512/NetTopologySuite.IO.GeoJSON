﻿using System;
using System.Text.Json;
using NetTopologySuite.Geometries;

namespace NetTopologySuite.IO.Converters
{
    internal partial class StjGeometryConverter
    {
        internal static Envelope ReadBBox(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            Envelope res = null;

            if (reader.TokenType != JsonTokenType.Null)
            {
                reader.ReadToken(JsonTokenType.StartArray);

                double minX = reader.GetDouble();
                reader.Read();
                double minY = reader.GetDouble();
                reader.Read();
                double maxX = reader.GetDouble();
                reader.Read();
                double maxY = reader.GetDouble();
                reader.Read();

                if (reader.TokenType == JsonTokenType.Number)
                {
                    maxX = maxY;
                    maxY = reader.GetDouble();
                    reader.Read();
                    reader.Read();
                }

                reader.ReadToken(JsonTokenType.EndArray);

                res = new Envelope(minX, maxX, minY, maxY);
            }

            //reader.Read(); // move away from array end
            return res;
        }

        internal static void WriteBBox(Utf8JsonWriter writer, Envelope value, JsonSerializerOptions options, Geometry geometry)
        {
            // if we don't want to write "null" bounding boxes, bail out.
            if ((value == null || value.IsNull) && options.IgnoreNullValues)
                return;

            // if value == null, try to get it from geometry
            if (value == null)
                value = geometry?.EnvelopeInternal ?? new Envelope();

            writer.WritePropertyName("bbox");
            if (value.IsNull)
            {
                writer.WriteNullValue();
                return;
            }

            writer.WriteStartArray();
            writer.WriteNumberValue(value.MinX);
            writer.WriteNumberValue(value.MinY);
            writer.WriteNumberValue(value.MaxX);
            writer.WriteNumberValue(value.MaxY);
            writer.WriteEndArray();
        }
    }
}
