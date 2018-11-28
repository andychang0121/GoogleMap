using System;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Google.Maps;
using Google.Maps.Direction;
using Google.Maps.Geocoding;
using Google.Maps.Places;
using Google.Maps.Places.Details;
using Google.Maps.Shared;
using Newtonsoft.Json;
using Xunit;

namespace GoogleMap.Test
{
    public class GoogleMapAPIUnitTest1
    {
        private readonly string _language = "zh-TW";
        private readonly LatLng _latlng = new LatLng(25.044308271193106,121.52936895061339);
        private readonly string _textSearchRequest = "大安森林公園";

        [Fact(DisplayName = "地址找座標")]
        public void Address2Location()
        {   
            GeocodingRequest request = new GeocodingRequest
            {
                Address = "台北市南京東路一段9號",
                Language = _language
            };
            GeocodeResponse response = new GeocodingService(SigningHelper.GetApiKey()).GetResponse(request);
            Result[] result = response.Results;
            Result result0 = response.Results[0];
            Geometry geometry = response.Results[0].Geometry;
            LatLng location = response.Results[0].Geometry.Location;
            string placeID = response.Results[0].PlaceId;

            PlaceDetailsRequest placeDetailRequest = new PlaceDetailsRequest
            {
                Language = _language,
                PlaceID = placeID
            };
            PlaceDetailsResponse placeDetailResponse = new PlaceDetailsService(SigningHelper.GetApiKey()).GetResponse(placeDetailRequest);
            
        }

        [Fact(DisplayName = "依照輸入地點找出")]
        public void PlaceQuery()
        {
            PlacesRequest placeRequest = new TextSearchRequest()
            {
                Query = _textSearchRequest,
                Radius = 10000,
                Language = _language
            };
            PlacesResponse placeResponse = new PlacesService(SigningHelper.GetApiKey()).GetResponse(placeRequest);
        }
        [Fact(DisplayName = "依照座標，再找出靠近的目標")]
        public void NearBySearch()
        {
            // 定點尋找附近的 by Type
            PlacesRequest nearByRequest = new NearbySearchRequest()
            {                  
                Language = _language,
                Location = _latlng,
                Radius = 10000,
            };
            PlacesResponse nearByResponse = new PlacesService(SigningHelper.GetApiKey()).GetResponse(nearByRequest);
            foreach (PlacesResult item in nearByResponse.Results)
            {
                string name = item.Name;
                Geometry geo = item.Geometry;
                LocationType locationType = geo.LocationType;
                Viewport viewport = item.Geometry.Viewport;
                var placeTypes = item.Types;
                var isLocality = item.Types.FirstOrDefault(x => x.Equals(PlaceType.Locality));
            }

            var jsonStr = JsonConvert.SerializeObject(nearByResponse.Results);
        }
        public static class SigningHelper
        {
            /// <summary>
            /// Holds the ApiKey used for testing
            /// </summary>
            private static readonly string s_ApiKey;
            static SigningHelper()
            {
                //during testing, get api key from the GOOGLE_API_KEY environment variable, to enable more flexibility and try to prevent OverQuotaLimit 
                s_ApiKey = Environment.GetEnvironmentVariable("GOOGLE_API_KEY");

                //Log.Info("GetEnvironmentVariable(\"GOOGLE_API_KEY\")='{0}'.",S_ApiKey);

                //for local testing, you can either use the Debugger to modify the S_ApiKey static variable
                //or set the Debugging Environment variable via the Visual Stdio Debugging properties dialog. 
                //for more info, see https://stackoverflow.com/a/155363/323456

                //for nunit test, might need to use the NUnit Test Settings file

                //if env["GOOGLE_API_KEY"] is empty, use a default key
                if (string.IsNullOrEmpty(s_ApiKey))
                {
                    //Log.Info("Setting ApiKey to a default key.");

                    //this api key can be used for individual developer machines
                    //you may get OverQueryLimit responses though
                    s_ApiKey = "AIzaSyAEEAf68Ghsvq49H-mO3ZmFazCQrb0-C6A";
                }

                //Note: don't specifically say code or api key or something here.  The key should be IP restricted but lets keep it obscure.
                //Console.WriteLine("SigningHelper: Initialized using '{0}' for testing.", S_ApiKey);
            }

            public static GoogleSigned GetPrivateKey()
            {
                throw new NotImplementedException();
            }

            public static GoogleSigned GetApiKey()
            {
                GoogleSigned returnResult = new GoogleSigned(s_ApiKey);
                return returnResult;
            }
        }
    }
}
