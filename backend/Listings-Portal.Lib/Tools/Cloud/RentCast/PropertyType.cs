using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Listings_Portal.Lib.Tools.Cloud.RentCast
{
    public enum ListingType
    {
        Rent,
        Sale
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PropertyType
    {
        [Description("Single Family")]
        SingleFamily,

        [Description("Condo")]
        Condo,

        [Description("Townhouse")]
        Townhouse,

        [Description("Manufactured")]
        Manufactured,

        [Description("Multi-Family")]
        MultiFamily,

        [Description("Apartment")]
        Apartment,

        [Description("Land")]
        Land,
    }
}
