using Listings_Portal.Lib.Models.Entities;
using System.Linq.Expressions;

namespace Listings_Portal.Lib.Models.Api.Dtos
{
    /// <summary>
    /// Realtor DTO.
    /// </summary>
    /// <param name="Name"> Realtor's name (i.e. "Zachary Barton"). </param>
    /// <param name="Phone"> Realtor's phone number (i.e. "5129948203"). </param>
    /// <param name="Email"> Realtor's email (i.e. "zak-barton@realtytexas.co"). </param>
    /// <param name="Website"> Realtor's wesbite (i.e. "https://zak-barton.realtytexas.homes"). </param>
    public sealed record RealtorDto(string Name, string? Phone, string? Email, string? Website)
    {
        /// <summary>
        /// Gets Realtor DTO model.
        /// </summary>
        /// <param name="entityModel"> Entity model. </param>
        /// <returns> Realtor DTO  model. </returns>
        public static RealtorDto? FromEntity(RealtorAgent? entityModel)
        {
            return entityModel == null ? null : new RealtorDto(
                entityModel.Name,
                entityModel.Phone,
                entityModel.Email,
                entityModel.Website);
        }

        /// <summary>
        /// Gets Realtor DTO model.
        /// </summary>
        /// <param name="entityModel"> Entity model. </param>
        /// <returns> Realtor DTO  model. </returns>
        public static RealtorDto? FromEntity(RealtorOffice? entityModel)
        {
            return entityModel == null ? null : new RealtorDto(
                entityModel.Name,
                entityModel.Phone,
                entityModel.Email,
                entityModel.Website);
        }
    }
}
