using Listings_Portal.Lib.Models.Entities;

namespace Listings_Portal.Lib.Models.Api.Dtos
{
    /// <summary>
    /// HOA DTO.
    /// </summary>
    /// <param name="Fee"> HOA fee. </param>
    public sealed record HoaDto(int Fee)
    {
        /// <summary>
        /// Gets HOA DTO model.
        /// </summary>
        /// <param name="entityModel"> Entity model. </param>
        /// <returns> HOA DTO  model. </returns>
        public static HoaDto? FromEntity(Hoa? entityModel)
        {
            return entityModel == null ? null : new HoaDto(
                Fee: entityModel.Fee);
        }
    }
}
