﻿using System.Linq;
using Microsoft.Xna.Framework.Content.Pipeline;
using MonoGame.Extended.Content.Pipeline.Tiled;
using NSubstitute;
using NUnit.Framework;

namespace MonoGame.Extended.Content.Pipeline.Tests
{
    [TestFixture]
    public class TiledMapImporterProcessorTests
    {
        [Test]
        public void TiledMapImporter_Import_Test()
        {
            const string filename = @"TestData\level01.tmx";
            var importer = new TiledMapImporter();
            var map = importer.Import(filename, Substitute.For<ContentImporterContext>());

            Assert.AreEqual("1.0", map.Version);
            Assert.AreEqual(TmxOrientation.Orthogonal, map.Orientation);
            Assert.AreEqual(TmxRenderOrder.RightDown, map.RenderOrder);
            Assert.AreEqual(20, map.Width);
            Assert.AreEqual(10, map.Height);
            Assert.AreEqual(128, map.TileWidth);
            Assert.AreEqual(128, map.TileHeight);
            Assert.AreEqual("#7d7d7d", map.BackgroundColor);
            Assert.AreEqual("awesome", map.Properties[0].Name);
            Assert.AreEqual("42", map.Properties[0].Value);
            Assert.AreEqual(1, map.Tilesets.Count);
        }

        [Test]
        public void TiledMapImporter_Xml_Test()
        {
            const string filename = @"TestData\test-tileset-xml.tmx";
            var map = ImportAndProcessMap(filename);
            var actualData = map.Layers[0].Data.Tiles.Select(i => i.Gid).ToArray();

            Assert.IsNull(map.Layers[0].Data.Encoding);
            Assert.IsNull(map.Layers[0].Data.Compression);
            Assert.IsTrue(new [] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }.SequenceEqual(actualData));
        }
        
        [Test]
        public void TiledMapImporter_Csv_Test()
        {
            const string filename = @"TestData\test-tileset-csv.tmx";
            var map = ImportAndProcessMap(filename);
            var data = map.Layers[0].Data.Tiles.Select(i => i.Gid).ToArray();

            Assert.AreEqual("csv", map.Layers[0].Data.Encoding);
            Assert.IsNull(map.Layers[0].Data.Compression);
            Assert.IsTrue(new [] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }.SequenceEqual(data));
        }

        [Test]
        public void TiledMapImporter_Base64_Test()
        {
            const string filename = @"TestData\test-tileset-base64.tmx";
            var map = ImportAndProcessMap(filename);
            var data = map.Layers[0].Data.Tiles.Select(i => i.Gid).ToArray();

            Assert.AreEqual("base64", map.Layers[0].Data.Encoding);
            Assert.IsNull(map.Layers[0].Data.Compression);
            Assert.IsTrue(new [] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }.SequenceEqual(data));
        }

        [Test]
        public void TiledMapImporter_Gzip_Test()
        {
            const string filename = @"TestData\test-tileset-gzip.tmx";
            var map = ImportAndProcessMap(filename);
            var data = map.Layers[0].Data.Tiles.Select(i => i.Gid).ToArray();

            Assert.AreEqual("base64", map.Layers[0].Data.Encoding);
            Assert.AreEqual("gzip", map.Layers[0].Data.Compression);
            Assert.IsTrue(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }.SequenceEqual(data));
        }


        [Test]
        public void TiledMapImporter_Zlib_Test()
        {
            const string filename = @"TestData\test-tileset-zlib.tmx";
            var map = ImportAndProcessMap(filename);
            var data = map.Layers[0].Data.Tiles.Select(i => i.Gid).ToArray();

            Assert.AreEqual("base64", map.Layers[0].Data.Encoding);
            Assert.AreEqual("zlib", map.Layers[0].Data.Compression);
            Assert.IsTrue(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }.SequenceEqual(data));
        }

        private static TmxMap ImportAndProcessMap(string filename)
        {
            var importer = new TiledMapImporter();
            var processor = new TiledMapProcessor();
            var import = importer.Import(filename, Substitute.For<ContentImporterContext>());
            var result = processor.Process(import, Substitute.For<ContentProcessorContext>());
            return result.Map;
        }
    }
}
