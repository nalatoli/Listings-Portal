using System.ComponentModel.DataAnnotations;

namespace Listings_Portal.Lib.Models.Api
{
    public class MessageResponse
    {
        /// <summary>
        /// Message in response.
        /// </summary>
        [Required]
        public required string Message { get; set; }

        /// <summary>
        /// Creates a message response.
        /// </summary>
        /// <param name="message"> Reponse's message. </param>
        /// <returns> Message response. </returns>
        public static MessageResponse Create(string message)
        {
            return new MessageResponse
            {
                Message = message
            };
        }
    }
}