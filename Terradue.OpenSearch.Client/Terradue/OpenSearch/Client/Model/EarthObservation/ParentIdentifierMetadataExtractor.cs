using System;
using Terradue.OpenSearch.Client.Model.GeoTime;
using Terradue.OpenSearch.Result;

namespace Terradue.OpenSearch.Client.Model.EarthObservation
{
    class ParentIdentifierMetadataExtractor : IMetadataExtractor
	{
        #region IMetadataExtractor implementation

        public string GetMetadata(IOpenSearchResultItem item) {

            return Terradue.Metadata.EarthObservation.OpenSearch.EarthObservationOpenSearchResultHelpers.FindParentIdentifierFromOpenSearchResultItem(item);
        }

        public string Description {
            get {
                return "A string identifying the parent of the entry in a hierarchy of resources (e.g. ER02_SAR_IM__0P, MER_RR__1P, SM_SLC__1S, GES_DISC_AIRH3STD_V005)";
            }
        }

        #endregion
	}

}
