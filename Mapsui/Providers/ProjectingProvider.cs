﻿using System;
using System.Collections.Generic;
using System.Linq;
using Mapsui.Extensions;
using Mapsui.Layers;
using Mapsui.Projection;

namespace Mapsui.Providers
{
    public class ProjectingProvider : IProvider<IFeature>
    {
        private readonly IProvider<IFeature> _provider;
        private readonly IProjection _projection;

        public ProjectingProvider(IProvider<IFeature> provider, IProjection? projection = null)
        {
            _provider = provider;
            _projection = projection ?? new MinimalProjection();
        }

        public string CRS { get; set; }

        public IEnumerable<IFeature> GetFeatures(FetchInfo fetchInfo)
        {
            if (fetchInfo.Extent == null) return new List<IFeature>();

            var copiedExtent = new MRect(fetchInfo.Extent);
            _projection.Project(CRS, _provider.CRS, copiedExtent);
            fetchInfo = new FetchInfo(copiedExtent, fetchInfo.Resolution, fetchInfo.CRS, fetchInfo.ChangeType);

            var features = _provider.GetFeatures(fetchInfo);
            if (!CrsHelper.IsProjectionNeeded(_provider.CRS, CRS)) return features;

            if (!CrsHelper.IsCrsProvided(_provider.CRS, CRS))
                throw new NotSupportedException($"CRS is not provided. From CRS: {_provider.CRS}. To CRS {CRS}");

            var copiedFeatures = features.Copy().ToList();
            _projection.Project(_provider.CRS, CRS, copiedFeatures);
            return copiedFeatures;
        }

        public MRect? GetExtent()
        {
            if (_provider.GetExtent() == null) return null;
            var extent = _provider.GetExtent()!;

            if (!CrsHelper.IsProjectionNeeded(_provider.CRS, CRS)) return extent;

            if (!CrsHelper.IsCrsProvided(_provider.CRS, CRS))
                throw new NotSupportedException($"CRS is not provided. From CRS: {_provider.CRS}. To CRS {CRS}");

            // This projects the full extent of the source. Usually the full extent of the source does not change,
            // so perhaps this should be calculated just once. Then again, there are probably situations where it does
            // change so a way to refresh this should be possible.
            var copiedExtent = new MRect(extent);
            _projection.Project(_provider.CRS, CRS, copiedExtent);
            return copiedExtent;
        }
    }
}
