using Google.Cloud.Location;
using System;

namespace Vouchee.Business.Utils
{
    public static class DistanceHelper
    {
        public static decimal CalculateDistance(decimal lat1, decimal lon1, decimal lat2, decimal lon2)
        {
            const decimal EarthRadiusKm = 6371; 

            decimal dLat = DegreesToRadians(lat2 - lat1);
            decimal dLon = DegreesToRadians(lon2 - lon1);

            decimal a = (decimal)(Math.Sin((double)dLat / 2) * Math.Sin((double)dLat / 2) +
                                   Math.Cos((double)DegreesToRadians(lat1)) * Math.Cos((double)DegreesToRadians(lat2)) *
                                   Math.Sin((double)dLon / 2) * Math.Sin((double)dLon / 2));
            decimal c = 2 * (decimal)Math.Atan2(Math.Sqrt((double)a), Math.Sqrt((double)(1 - a)));

            decimal distance = EarthRadiusKm * c; 

            return Math.Round(distance, 2); 
        }

        public static decimal DegreesToRadians(decimal degrees)
        {
            return degrees * (decimal)(Math.PI / 180);
        }
    }

}
