using System;
using Google.Maps;
using Google.Maps.Direction;
using Google.Maps.Geocoding;
using Google.Maps.Places;
using Google.Maps.Places.Details;
using Google.Maps.Shared;
using Xunit;

namespace GoogleMap.Test
{
    public class GoogleMapAPIUnitTest1
    {
        private readonly string _apiKey = "AIzaSyAEEAf68Ghsvq49H-mO3ZmFazCQrb0-C6A";
        private readonly string _language = "zh-TW";
        private readonly LatLng _latlng = new LatLng(25.044308271193106,121.52936895061339);
        GoogleSigned _testingApiKey;

        [Fact(DisplayName = "地址找座標")]
        public void Address2Location()
        {
            
            GoogleSigned.AssignAllServices(new GoogleSigned(_apiKey));
            GeocodingRequest request = new GeocodingRequest
            {
                Address = "台北市南京東路一段9號",
                Language = _language
            };
            GeocodeResponse response = new GeocodingService().GetResponse(request);
            Result[] result = response.Results;
            Result result0 = response.Results[0];
            Geometry geometry = response.Results[0].Geometry;
            LatLng location = response.Results[0].Geometry.Location;
            string placeID = response.Results[0].PlaceId;

            PlacesRequest placeRequest = new TextSearchRequest()
            {
                Query = "台北市信義區",
                Radius = 10000,
                Language = _language
            };

            PlacesResponse placeRResponse = new PlacesService().GetResponse(placeRequest);


            PlacesRequest nearByRequest = new NearbySearchRequest()
            {
                Language = _language,
                Location = _latlng,
                Radius = 10000,
            };

            PlacesResponse nearByResponse = new PlacesService().GetResponse(nearByRequest);

            PlaceDetailsRequest placeDetailRequest = new PlaceDetailsRequest
            {
                Language = _language,
                PlaceID = placeID
            };
            PlaceDetailsResponse placeResponse = new PlaceDetailsService().GetResponse(placeDetailRequest);
            
        }
        public static class SigningHelper
        {
            static SigningHelper()
            {
                //during testing, get api key from the GOOGLE_API_KEY environment variable, to enable more flexibility and try to prevent OverQuotaLimit 
                S_ApiKey = Environment.GetEnvironmentVariable("GOOGLE_API_KEY");

                //Log.Info("GetEnvironmentVariable(\"GOOGLE_API_KEY\")='{0}'.",S_ApiKey);

                //for local testing, you can either use the Debugger to modify the S_ApiKey static variable
                //or set the Debugging Environment variable via the Visual Stdio Debugging properties dialog. 
                //for more info, see https://stackoverflow.com/a/155363/323456

                //for nunit test, might need to use the NUnit Test Settings file

                //if env["GOOGLE_API_KEY"] is empty, use a default key
                if (string.IsNullOrEmpty(S_ApiKey))
                {
                    //Log.Info("Setting ApiKey to a default key.");

                    //this api key can be used for individual developer machines
                    //you may get OverQueryLimit responses though
                    S_ApiKey = "AIzaSyAEEAf68Ghsvq49H-mO3ZmFazCQrb0-C6A";
                }

                //Note: don't specifically say code or api key or something here.  The key should be IP restricted but lets keep it obscure.
                Console.WriteLine("SigningHelper: Initialized using '{0}' for testing.", S_ApiKey);
            }

            /// <summary>Holds the ApiKey used for testing</summary>
            private static string S_ApiKey;

            public static GoogleSigned GetPrivateKey()
            {
                throw new NotImplementedException();
            }

            public static GoogleSigned GetApiKey()
            {
                return new GoogleSigned(S_ApiKey);
            }
        }
    }
}
