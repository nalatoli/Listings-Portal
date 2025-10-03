using Listings_Portal.Lib.Models.Api.Dtos;
using Listings_Portal.Lib.Models.Entities;
using Listings_Portal.Tests.Tools.Managers;

namespace Listings_Portal.Tests.Tools
{
    internal partial class SampleData
    {
        public static RealtorAgent CreateRealtorCloud(Action<RealtorAgent>? configure = null)
        {
            return new RealtorAgent
            {
                Name = "Agent Name",
                Phone = "1234567890",
                Email = "dummyemail@woohoo.com",
                Website = "www.coobeans.com",
            }.GetConfigured(configure);
        }

        public static RealtorAgent CreateRealtorAgent(Action<RealtorAgent>? configure = null)
        {
            return new RealtorAgent
            {
                Name = "Agent Name",
                Phone = "1234567890",
                Email = "dummyemail@woohoo.com",
                Website = "www.coobeans.com",
            }.GetConfigured(configure);
        }

        public static RealtorOffice CreateRealtorOffice(Action<RealtorOffice>? configure = null)
        {
            return new RealtorOffice
            {
                Name = "Agent Name",
                Phone = "1234567890",
                Email = "dummyemail@woohoo.com",
                Website = "www.coobeans.com",
            }.GetConfigured(configure);
        }
    }
}