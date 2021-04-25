using System.ComponentModel.DataAnnotations;

namespace AspNetCoreRateLimitAbpDemo.Users.Dto
{
    public class ChangeUserLanguageDto
    {
        [Required]
        public string LanguageName { get; set; }
    }
}