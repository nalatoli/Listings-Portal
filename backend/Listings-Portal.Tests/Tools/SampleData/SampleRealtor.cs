using Listings_Portal.Lib.Models.Api.Dtos;
using Listings_Portal.Lib.Models.Entities;
using Listings_Portal.Tests.Tools.Managers;

namespace Listings_Portal.Tests.Tools
{
    internal partial class SampleData
    {
        public static Realtor CreateRealtorCloud(Action<Realtor>? configure = null)
        {
            return new Realtor
            {
                Name = "Agent Name",
                Phone = "1234567890",
                Email = "dummyemail@woohoo.com",
                Website = "www.coobeans.com",
            }.GetConfigured(configure);
        }

        public static Realtor CreateRealtor(Action<Realtor>? configure = null)
        {
            return new Realtor
            {
                Id = 1,
                Name = "Agent Name",
                Phone = "1234567890",
                Email = "dummyemail@woohoo.com",
                Website = "www.coobeans.com",
            }.GetConfigured(configure);
        }
    }
}