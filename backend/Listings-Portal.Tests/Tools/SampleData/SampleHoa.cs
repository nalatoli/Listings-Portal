using Listings_Portal.Lib.Models.Api.Dtos;
using Listings_Portal.Lib.Models.Cloud;
using Listings_Portal.Lib.Models.Entities;
using Listings_Portal.Tests.Tools.Managers;

namespace Listings_Portal.Tests.Tools
{
    internal partial class SampleData
    {
        public static HoaCloud CreateHoaCloud(Action<HoaCloud>? configure = null)
        {
            return new HoaCloud
            {
                Fee = 50,
            }.GetConfigured(configure);
        }

        public static Hoa CreateHoa(Action<Hoa>? configure = null)
        {
            return new Hoa
            {
                Fee = 50,
            }.GetConfigured(configure);
        }
    }
}