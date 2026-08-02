using System.ComponentModel.DataAnnotations;

namespace EpicMock.DTOs
{
    public class PatientUpdateDTO
    {
        [Required, MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public DateOnly DateOfBirth { get; set; }

        [MaxLength(1)]
        public string? Sex { get; set; }

        [Phone]
        public string? Phone { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        public string? AddressLine1 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? InsuranceProvider { get; set; }
        public string? InsurancePolicyNo { get; set; }
    }
}
