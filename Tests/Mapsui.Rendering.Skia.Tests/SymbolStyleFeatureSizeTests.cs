using System;
using Mapsui.Rendering.Skia.Cache;
using Mapsui.Styles;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Mapsui.Rendering.Skia.Tests;

[TestFixture]
public class SymbolStyleFeatureSizeTests
{
    [Test]
    public void DefaultSizeFeatureSize()
    {
        var symbolStyle = new SymbolStyle
        {
            SymbolType = SymbolType.Rectangle,
        };

        using var renderService = new RenderService();
        var size = SymbolStyleRenderer.FeatureSize(symbolStyle, renderService);

        ClassicAssert.AreEqual(size, Math.Max(SymbolStyle.DefaultHeight, SymbolStyle.DefaultWidth) + 1);
    }

    [Test]
    public void DefaultSizeFeatureSize_Scaling()
    {
        var symbolStyle = new SymbolStyle
        {
            SymbolType = SymbolType.Rectangle,
            SymbolScale = 2,
        };

        using var renderService = new RenderService();
        var size = SymbolStyleRenderer.FeatureSize(symbolStyle, renderService);

        ClassicAssert.AreEqual(size, (Math.Max(SymbolStyle.DefaultHeight, SymbolStyle.DefaultWidth) + 1) * 2);
    }

    [Test]
    public void DefaultSizeFeatureSize_Offset_x()
    {
        var symbolStyle = new SymbolStyle
        {
            SymbolType = SymbolType.Rectangle,
            SymbolOffset = new Offset(2, 0),
        };

        using var renderService = new RenderService();
        var size = SymbolStyleRenderer.FeatureSize(symbolStyle, renderService);

        ClassicAssert.AreEqual(size, Math.Max(SymbolStyle.DefaultHeight, SymbolStyle.DefaultWidth) + 2 * 2 + 1);
    }

    [Test]
    public void DefaultSizeFeatureSize_Offset_y()
    {
        var symbolStyle = new SymbolStyle
        {
            SymbolType = SymbolType.Rectangle,
            SymbolOffset = new Offset(0, 2),
        };

        using var renderService = new RenderService();
        var size = SymbolStyleRenderer.FeatureSize(symbolStyle, renderService);

        ClassicAssert.AreEqual(size, Math.Max(SymbolStyle.DefaultHeight, SymbolStyle.DefaultWidth) + 2 * 2 + 1);
    }

    [Test]
    public void DefaultSizeFeatureSize_Offset_x_y()
    {
        var symbolStyle = new SymbolStyle
        {
            SymbolType = SymbolType.Rectangle,
            SymbolOffset = new Offset(2, 2),
        };

        using var renderService = new RenderService();
        var size = SymbolStyleRenderer.FeatureSize(symbolStyle, renderService);

        ClassicAssert.AreEqual(size, Math.Max(SymbolStyle.DefaultHeight, SymbolStyle.DefaultWidth) + 1 + Math.Sqrt(2 * 2 + 2 * 2) * 2);
    }

    [Test]
    public void BitmapInfoFeatureSize()
    {
        // Arrange
        using var renderService = new RenderService();
        var imagePath = new Uri("embeddedresource://Mapsui.Resources.Images.Pin.svg");
        var symbolStyle = new SymbolStyle
        {
            BitmapPath = imagePath,
        };

        BitmapPathInitializer.InitializeWhenNeeded((r) =>
        {
            // Act
            var size = SymbolStyleRenderer.FeatureSize(symbolStyle, renderService);

            // Assert
            ClassicAssert.AreEqual(size, 32);
        });
    }
}
