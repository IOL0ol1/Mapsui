﻿// Copyright (c) The Mapsui authors.
// The Mapsui authors licensed this file under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mapsui.Layers;
using Mapsui.Samples.Common;
using Mapsui.Samples.Common.Desktop;
using Mapsui.Samples.Common.Maps;
using Mapsui.Samples.Common.Maps.Animations;
using Mapsui.Samples.Common.Maps.Callouts;
using Mapsui.Samples.Common.Maps.Data;
using Mapsui.Samples.Common.Maps.Projection;
using Mapsui.Tiling;
using Mapsui.UI;
using NUnit.Framework;

namespace Mapsui.Rendering.Skia.Tests;

[TestFixture, Apartment(ApartmentState.STA)]
public class MapRegressionTests
{
    static MapRegressionTests()
    {
        // Load Desktop Samples
        Console.WriteLine(typeof(ShapefileSample));
    }

    private static ISample[]? _excludedSamples;
    private static ISample[]? _regressionSamples;

    public MapRegressionTests()
    {
        // Tile Cache
        OpenStreetMap.DefaultCache ??= File.ReadFromCacheFolder("OpenStreetMap");
        BingArial.DefaultCache ??= File.ReadFromCacheFolder("BingArial");
        BingHybrid.DefaultCache ??= File.ReadFromCacheFolder("BingHybrid");
        Michelin.DefaultCache ??= File.ReadFromCacheFolder("Michelin");
        TiledWmsSample.DefaultCache ??= File.ReadFromCacheFolder("TiledWmsSample");
        TmsSample.DefaultCache ??= File.ReadFromCacheFolder("TmsSample");
        WmtsSample.DefaultCache ??= File.ReadFromCacheFolder("WmtsSample");

        // Url Cache
        WmsSample.DefaultCache ??= File.ReadFromCacheFolder("WmsSample");
        WfsSample.DefaultCache ??= File.ReadFromCacheFolder("WfsSample");
    }

    public static object[] RegressionSamples => _regressionSamples ??= AllSamples.GetSamples().Where(f => ExcludedSamples.All(e => e.GetType() != f.GetType())).OrderBy(f => f.GetType().FullName).ToArray();

    public static object[] ExcludedSamples => _excludedSamples ??= new ISample[] {
    };

    [Test]
    [Retry(5)]
    [TestCaseSource(nameof(RegressionSamples))]
    public async Task TestSample(ISample sample)
    {
        await TestSample(sample, true).ConfigureAwait(false);
    }

    public async Task TestSample(ISample sample, bool compareImages)
    {
        try
        {
            var fileName = sample.GetType().Name + ".Regression.png";
            var mapControl = await InitMap(sample).ConfigureAwait(true);
            var map = mapControl.Map;
            await DisplayMap(mapControl).ConfigureAwait(false);

            if (map != null)
            {
                // act
                using var bitmap = new MapRenderer().RenderToBitmapStream(mapControl.Viewport, map.Layers, map.BackColor, 2);

                // aside
                if (bitmap is { Length: > 0 })
                {
                    File.WriteToGeneratedRegressionFolder(fileName, bitmap);
                }
                else
                {
                    Assert.Fail("Should generate Image");
                }

                // assert
                if (compareImages)
                {
                    using var originalStream = File.ReadFromOriginalRegressionFolder(fileName);
                    if (originalStream == null)
                    {
                        Assert.Inconclusive($"No Regression Test Data for {sample.Name}");
                    }
                    else
                    {
                        Assert.IsTrue(MapRendererTests.CompareBitmaps(originalStream, bitmap, 1, 0.99));
                    }
                }
                else
                {
                    // Don't compare images here because to unreliable
                    Assert.True(true);
                }
            }
        }
        finally
        {
            if (sample is IDisposable disposable)
            {
#pragma warning disable IDISP007 // Don't dispose injected
                disposable.Dispose();
#pragma warning restore IDISP007 // Don't dispose injected
            }
        }
    }

    [Test]
    [TestCaseSource(nameof(ExcludedSamples))]
    public async Task ExcludedTestSample(ISample sample)
    {
        await TestSample(sample, false);
    }

    private static async Task<RegressionMapControl> InitMap(ISample sample)
    {
        var mapControl = new RegressionMapControl();
        mapControl.SetSize(800, 600);

        if (sample is IPrepareSampleTest prepareTest)
        {
            prepareTest.PrepareTest();
        }

        sample.Setup(mapControl);
        if (sample is ISampleTest sampleTest)
        {
            await sampleTest.InitializeTest().ConfigureAwait(true);
        }

        var fetchInfo = new FetchInfo(mapControl.Viewport.Extent!, mapControl.Viewport.Resolution, mapControl.Map?.CRS);
        mapControl.Map?.RefreshData(fetchInfo);

        // TODO: MapView should be available for all Targets
        ////if (sample is IFormsSample formsSample)
        ////{
        ////    var mReadOnlyPoint = mapControl.Viewport.Center;
        ////    var position = new Position(mReadOnlyPoint.X, mReadOnlyPoint.Y);
        ////    var eventArgs = new MapClickedEventArgs(position, 1);
        ////    formsSample.OnClick(mapControl, eventArgs);
        ////}

        return mapControl;
    }

    private async Task DisplayMap(IMapControl mapControl)
    {
        await WaitForLoading(mapControl).ConfigureAwait(false);

        // wait for rendering to finish to make the Tests more reliable
        await Task.Delay(300).ConfigureAwait(false);
    }

    private async Task WaitForLoading(IMapControl mapControl)
    {
        if (mapControl.Map?.Layers != null)
        {
            foreach (var layer in mapControl.Map.Layers)
            {
                await WaitForLoading(layer).ConfigureAwait(false);
            }
        }
    }

    private async Task WaitForLoading(ILayer layer)
    {
        while (layer.Busy)
        {
            await Task.Delay(100).ConfigureAwait(false);
        }
    }
}