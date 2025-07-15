using FluentAssertions;
using Listings_Portal.Lib.Models.Api.Dtos;
using Listings_Portal.Lib.Models.Entities;
using Listings_Portal.Tests.Tools;

namespace Listings_Portal.Tests.Unit
{
    public class ModelTests
    {
        [Fact]
        public void Listing_Cloud_To_Entity_Should_Map()
        {
            var data = SampleData.CreateListingCloud();
            var result = Listing.FromCloud(data);
            data.Should().BeEquivalentTo(result, options => options
                .Excluding(r => r.Id)
                .Excluding(r => r.Guid)
                .Excluding(r => r.Type)
                .Excluding(r => r.Location)
                .Excluding(r => r.Hoa!.Id));
            result.Guid.Should().Be(data.Id);
            result.Location.Y.Should().Be(data.Latitude);
            result.Location.X.Should().Be(data.Longitude);
        }

        [Fact]
        public void Listing_Entity_To_Dto_Should_Map()
        {
            var data = SampleData.CreateListing();
            var result = ListingDto.FromEntity(data);
            result.Should().BeEquivalentTo(data, options => options
                .Excluding(d => d.Guid)
                .Excluding(d => d.Type)
                .Excluding(d => d.Location)
                .Excluding(d => d.Hoa!.Id)
                .Excluding(d => d.Status)
                .Excluding(d => d.ListingAgent!.Id)
                .Excluding(d => d.ListingOffice!.Id));
            data.Location.X.Should().Be(result.Latitude);
            data.Location.Y.Should().Be(result.Longitude);
        }

        [Fact]
        public void Hoa_Cloud_To_Entity_Should_Map()
        {
            var data = SampleData.CreateHoaCloud();
            var result = Hoa.FromCloud(data);
            result.Should().BeEquivalentTo(data);
        }

        [Fact]
        public void Hoa_Entity_To_Dto_Should_Map()
        {
            var data = SampleData.CreateHoa();
            var result = HoaDto.FromEntity(data);
            result.Should().BeEquivalentTo(data, options => options
                .Excluding(d => d.Id));
        }

        [Fact]
        public void Realtor_Cloud_To_Entity_Should_Map()
        {
            var data = SampleData.CreateRealtorCloud();
            var result = Realtor.FromCloud(data);
            result.Should().BeEquivalentTo(data);
        }

        [Fact]
        public void Realtor_Entity_To_Dto_Should_Map()
        {
            var data = SampleData.CreateRealtor();
            var result = RealtorDto.FromEntity(data);
            result.Should().BeEquivalentTo(data, options => options
                .Excluding(d => d.Id));
        }
    }
}
