using System.ComponentModel.DataAnnotations;

namespace SocleRHHBESSAIH.Models
{
    public class EntityBase
    {
        [Key]
        public Guid Id { get; set; }
    }
}
