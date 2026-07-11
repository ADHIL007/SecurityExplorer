using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SecurityExplorer.SampleWebApi.DTOs
{
    public class CreateUserRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(4)]
        public required string Password { get; set; }

        [Required]
        public String Department { get; set; } = string.Empty;
    }
}
