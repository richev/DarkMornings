using System.Xml.Linq;
using NUnit.Framework;
using Richev.DarkMornings.Web.Services;

namespace Richev.DarkMornings.Web.Tests.Service
{
    [TestFixture]
    public class ResponseParserTests
    {
        /// <summary>
        /// Response from http://api.hostip.info/get_xml.php?ip=77.96.85.169&position=true
        /// </summary>
        private const string SuccessResponse = @"<?xml version=""1.0"" encoding=""ISO-8859-1"" ?>
<HostipLookupResultSet version=""1.0.1"" xmlns:gml=""http://www.opengis.net/gml"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:noNamespaceSchemaLocation=""http://www.hostip.info/api/hostip-1.0.1.xsd"">
 <gml:description>This is the Hostip Lookup Service</gml:description>
 <gml:name>hostip</gml:name>
 <gml:boundedBy>
  <gml:Null>inapplicable</gml:Null>
 </gml:boundedBy>
 <gml:featureMember>
  <Hostip>
   <ip>77.96.85.169</ip>
   <gml:name>Woking</gml:name>
   <countryName>UNITED KINGDOM</countryName>
   <countryAbbrev>GB</countryAbbrev>
   <!-- Co-ordinates are available as lng,lat -->
   <ipLocation>
    <gml:pointProperty>
     <gml:Point srsName=""http://www.opengis.net/gml/srs/epsg.xml#4326"">
      <gml:coordinates>-0.5333,51.3167</gml:coordinates>
     </gml:Point>
    </gml:pointProperty>
   </ipLocation>
  </Hostip>
 </gml:featureMember>
</HostipLookupResultSet>";

        /// <summary>
        /// Response from http://api.hostip.info/get_xml.php?ip=foo&position=true
        /// </summary>
        private const string FailureResponse = @"<?xml version=""1.0"" encoding=""ISO-8859-1"" ?>
<HostipLookupResultSet version=""1.0.1"" xmlns:gml=""http://www.opengis.net/gml"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:noNamespaceSchemaLocation=""http://www.hostip.info/api/hostip-1.0.1.xsd"">
 <gml:description>This is the Hostip Lookup Service</gml:description>
 <gml:name>hostip</gml:name>
 <gml:boundedBy>
  <gml:Null>inapplicable</gml:Null>
 </gml:boundedBy>
 <gml:featureMember>
  <Hostip>
   <ip>foo</ip>
   <gml:name>(Private Address)</gml:name>
   <countryName>(Private Address)</countryName>
   <countryAbbrev>XX</countryAbbrev>
   <!-- Co-ordinates are unavailable -->
  </Hostip>
 </gml:featureMember>
</HostipLookupResultSet>";

        [Test]
        public void ParseShouldSetCoordsWithSuccessResponse()
        {
            var response = XDocument.Parse(SuccessResponse);


            var location = ResponseParser.Parse(response);

            Assert.IsTrue(location.HasValue);
            Assert.AreEqual(51.3167, location.Value.Latitude);
            Assert.AreEqual(-0.5333, location.Value.Longitude);
        }

        [Test]
        public void ParseShouldSetCoordsToNullWithFailureResponse()
        {
            var response = XDocument.Parse(FailureResponse);

            var location = ResponseParser.Parse(response);

            Assert.IsFalse(location.HasValue);
        }
    }
}
