using Google.Maps;
using Google.Maps.Geocoding;
using Xunit;

namespace GoogleMap.Test
{
    public class UnitTest1
    {
        private readonly string _apiKey = "AIzaSyAEEAf68Ghsvq49H-mO3ZmFazCQrb0-C6A";

        [Fact(DisplayName = "Test 1")]
        public void TestMethod1()
        {
            GoogleSigned.AssignAllServices(new GoogleSigned(_apiKey));
            GeocodingRequest request = new GeocodingRequest { Address = "台北市松山區南京東路五段" };
            GeocodeResponse response = new GeocodingService().GetResponse(request);
        }
    }
}
