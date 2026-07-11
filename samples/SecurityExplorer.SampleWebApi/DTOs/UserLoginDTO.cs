using System.ComponentModel.DataAnnotations;

namespace SecurityExplorer.SampleWebApi.DTOs
{
    public class UserLoginDTO
    {
        [Required]
        public required String email { get; set; }
        [Required]
        public required string password { get; set; }

    }

}
