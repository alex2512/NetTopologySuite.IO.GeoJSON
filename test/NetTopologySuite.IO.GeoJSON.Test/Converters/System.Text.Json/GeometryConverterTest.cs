﻿using System;
using System.IO;
using System.Text;
using NetTopologySuite.Geometries;
using NUnit.Framework;

namespace NetTopologySuite.IO.GeoJSON.Test.Converters.System.Text.Json
{
    public class GeometryConverterTest : SandDTest<Geometry>
    {
        [Test]
        public void TestReadPoint2D()
        {
            string geoJson = @"{ ""type"" : ""Point"", ""coordinates"": [102.0, 0.5] }";
            var options = DefaultOptions;
            var geom = Deserialize(geoJson, options);

            Assert.That(geom != null);
        }

        [Test]
        public void TestReadPoint3D()
        {
            string geoJson = @"{ ""type"" : ""Point"", ""coordinates"": [102.0, 0.5, 6.2] }";
            var options = DefaultOptions;
            var geom = Deserialize(geoJson, options);

            Assert.That(geom != null);
            Assert.That(geom, Is.InstanceOf(typeof(Point)));
            Assert.That(geom.Coordinate, Is.InstanceOf(typeof(CoordinateZ)));
        }

        [Test]
        public void TestReadLineString2D()
        {
            string geoJson = @"{ ""type"" : ""LineString"", ""coordinates"": [[102.0, 0.5],[112.7, 2.1]] }";
            var options = DefaultOptions;
            var geom = Deserialize(geoJson, options);

            Assert.That(geom != null);
            Assert.That(geom, Is.InstanceOf(typeof(LineString)));
        }

        [Test]
        public void TestReadLineString3D()
        {
            string geoJson = @"{ ""type"" : ""LineString"", ""coordinates"": [[102.0, 0.5, 2.45],[112.7, 2.1, 2.34]] }";
            var options = DefaultOptions;
            var geom = Deserialize(geoJson, options);

            Assert.That(geom != null);
            Assert.That(geom, Is.InstanceOf(typeof(LineString)));
            Assert.That(geom.Coordinate, Is.InstanceOf(typeof(CoordinateZ)));
        }

        [Test]
        public void TestReadPolygon2D()
        {
            string geoJson = @"{ ""type"" : ""Polygon"", ""coordinates"": [[[0, 0],[10, 0],[10, 10],[0, 0]],[[1, 1],[9, 9],[1, 9],[1, 1]]] }";
            var options = DefaultOptions;
            var geom = Deserialize(geoJson, options);

            Assert.That(geom != null);
            Assert.That(geom, Is.InstanceOf(typeof(Polygon)));
            Assert.That(((Polygon)geom).NumInteriorRings, Is.EqualTo(1));
        }

        [Test]
        public void TestReadMultiPoint2D()
        {
            string geoJson = @"{ ""type"" : ""MultiPoint"", ""coordinates"": [[102.0, 0.5],[112.7, 2.1],[102.0, 1.5],[112.7, 3.1]] }";
            var options = DefaultOptions;
            var geom = Deserialize(geoJson, options);

            Assert.That(geom != null);
            Assert.That(geom, Is.InstanceOf(typeof(MultiPoint)));
            Assert.That(geom.NumGeometries, Is.EqualTo(4));
        }

        [Test]
        public void TestReadMultiLineString2D()
        {
            string geoJson = @"{ ""type"" : ""MultiLineString"", ""coordinates"": [[[102.0, 0.5],[112.7, 2.1]],[[102.0, 1.5],[112.7, 3.1]]] }";
            var options = DefaultOptions;
            var geom = Deserialize(geoJson, options);

            Assert.That(geom != null);
            Assert.That(geom, Is.InstanceOf(typeof(MultiLineString)));
            Assert.That(geom.NumGeometries, Is.EqualTo(2));
        }

        [Test]
        public void TestReadMultiPolygon2D()
        {
            string geoJson = @"{ ""type"" : ""MultiPolygon"", ""coordinates"": [
[[[0, 0],[10, 0],[10, 10],[0, 0]],[[1, 1],[9, 9],[1, 9],[1, 1]]],
[[[20, 20],[30, 20],[30, 30],[20, 20]],[[21, 21],[29, 29],[21, 29],[21, 21]]]
] }";
            var options = DefaultOptions;
            var geom = Deserialize(geoJson, options);

            Assert.That(geom != null);
            Assert.That(geom, Is.InstanceOf(typeof(MultiPolygon)));
            Assert.That(geom.NumGeometries, Is.EqualTo(2));
        }

        [Test]
        public void TestReadGeometryCollection()
        {
            string geoJson = @"{ ""type"" : ""GeometryCollection"", ""geometries"": [
{ ""type"" : ""Polygon"", ""coordinates"": [[[0, 0],[10, 0],[10, 10],[0, 0]],[[1, 1],[9, 9],[1, 9],[1, 1]]] },
{ ""type"" : ""LineString"", ""coordinates"": [[102.0, 0.5],[112.7, 2.1]] },
{ ""type"" : ""Point"", ""coordinates"": [102.0, 0.5] }
] }";
            var options = DefaultOptions;
            var geom = Deserialize(geoJson, options);

            Assert.That(geom != null);
            Assert.That(geom, Is.InstanceOf(typeof(GeometryCollection)));
            Assert.That(geom.NumGeometries, Is.EqualTo(3));
        }


        [TestCase("POINT (1 2)")]
        [TestCase("POINT Z (1 2 3)")]
        [TestCase("LINESTRING (1 2, 2 2)")]
        [TestCase("LINESTRING Z (1 2 0, 2 2 0)")]
        [TestCase("POLYGON ((0 0, 10 10, 0 10, 0 0))")]
        [TestCase("POLYGON Z ((0 0 1, 10 10 1, 0 10 1, 0 0 1))")]
        [TestCase("POLYGON ((0 0, 10 10, 0 10, 0 0), (1 2, 1 9, 8 9, 1 2))")]
        [TestCase("POLYGON Z ((0 0 1, 10 10 1, 0 10 1, 0 0 1), (1 2.4 1, 1 9 1, 7.6 9 1, 1 2.4 1))")]

        public void TestWriteReadWkt(string wkt)
        {
            var wktReader = new WKTReader(NtsGeometryServices.Instance.CreateGeometryFactory(4326));
            var geomS = wktReader.Read(wkt);
            var options = DefaultOptions;

            Geometry geomD = null;
            using (var ms = new MemoryStream())
            {
                Serialize(ms, geomS, options);
                geomD = Deserialize(ms, options);

            }
            Assert.That(geomS.EqualsTopologically(geomD));
        }
    }
}
