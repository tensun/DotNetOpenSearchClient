﻿using System;
using Terradue.OpenSearch.Model.GeoTime;
using Mono.Addins;
using System.Collections.Specialized;
using System.Collections.Generic;
using Terradue.OpenSearch.Engine;
using System.Net;
using Terradue.OpenSearch.Model.EarthObservation.OpenSearchable;
using log4net;

namespace Terradue.OpenSearch.Model.EarthObservation {
    
    [Extension(typeof(IOpenSearchClientDataModelExtension))]
    public class EarthObservationDataModelExtension : GeoTimeDataModelExtension, IOpenSearchClientDataModelExtension {

        private static readonly ILog log = LogManager.GetLogger(typeof(EarthObservationDataModelExtension));

        protected override void InitializeExtractors() {

            base.InitializeExtractors();

            metadataExtractors.Remove("wkt");
            metadataExtractors.Add("wkt", new GeometryMetadataExtractor());
            metadataExtractors.Remove("identifier");
            metadataExtractors.Add("identifier", new IdentifierMetadataExtractor());
            metadataExtractors.Remove("startdate");
            metadataExtractors.Add("startdate", new StartDateMetadataExtractor());
            metadataExtractors.Remove("enddate");
            metadataExtractors.Add("enddate", new EndDateMetadataExtractor());
            metadataExtractors.Add("productType", new ProductTypeMetadataExtractor());
            metadataExtractors.Add("parentIdentifier", new ParentIdentifierMetadataExtractor());
            metadataExtractors.Add("orbitNumber", new OrbitNumberMetadataExtractor());
            metadataExtractors.Add("orbitDirection", new OrbitDirectionMetadataExtractor());
            metadataExtractors.Add("track", new TrackMetadataExtractor());
            metadataExtractors.Add("frame", new FrameMetadataExtractor());
            metadataExtractors.Add("swathIdentifier", new SwathIdentifierMetadataExtractor());
            metadataExtractors.Add("platform", new PlatformShortNameMetadataExtractor());
            metadataExtractors.Add("operationalMode", new OperationalModeMetadataExtractor());
            metadataExtractors.Add("polarisationChannels", new PolarisationChannelsMetadataExtractor());
            metadataExtractors.Add("wrsLongitudeGrid", new WrsLongitudeGridMetadataExtractor());
            metadataExtractors.Add("processingLevel", new ProcessingLevelMetadataExtractor());

        }

        public override string Name {
            get {
                return "EOP";
            }
        }

        public override string Description {
            get {
                return "Data model to handle Earth Observation dataset according to OGC® OpenSearch Extension for Earth Observation OGC 13 026\n" +
                "Some parameters allows to override or to filter metadata:" +
                "enclosure:scheme filters enclosure with corresponding scheme" +
                "enclosure:host filters enclosure with corresponding hostname";
            }
        }

        public override void SetQueryParameters(NameValueCollection nvc) {
            
        }

        public override IOpenSearchable CreateOpenSearchable(List<Uri> baseUrls, string queryFormatArg, OpenSearchEngine ose, NetworkCredential netCreds) {
            List<IOpenSearchable> entities = new List<IOpenSearchable>();

            IOpenSearchEngineExtension ext;

            if (string.IsNullOrEmpty(queryFormatArg)) {
                ext = ose.GetExtensionByExtensionName("atom");
            } else {
                ext = ose.GetExtensionByExtensionName(queryFormatArg);
            }

            foreach (var url in baseUrls) {
                var e = OpenSearchFactory.FindOpenSearchable(ose, url, ext.DiscoveryContentType);
                if (!e.DefaultMimeType.Contains("profile=http://earth.esa.int/eop")) {
                    try {
                        e = OpenSearchFactory.FindOpenSearchable(ose, url, "application/atom+xml; profile=http://earth.esa.int/eop/2.1");
                    } catch (InvalidOperationException){
                        e = OpenSearchFactory.FindOpenSearchable(ose, url, "application/atom+xml");
                    }
                    if (!e.DefaultMimeType.Contains("xml"))
                        throw new InvalidOperationException("No Url in the OpenSearch Description Document that could fit the EOP data model");
                }
                // Fedeo case
                if (url.Host == "fedeo.esa.int" && e.DefaultMimeType == "application/atom+xml" && e is Terradue.OpenSearch.GenericOpenSearchable) {
                    log.DebugFormat("Fedeo source. Trying to get the earthobservation profile");
                    e = FedeoOpenSearchable.CreateFrom((Terradue.OpenSearch.GenericOpenSearchable)e, ose);
                }
                entities.Add(e);
            }

            IOpenSearchable entity;

            if (entities.Count > 1) {
                entity = new MultiGenericOpenSearchable(entities, ose);
            } else {
                entity = entities[0];
            }

            return entity;
        }

    }
}

