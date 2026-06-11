using System.ComponentModel.DataAnnotations;

namespace AuthService.Application.Dtos.Tenant
{
    public sealed class RegisterTenantRequest : IValidatableObject
    {
        [Required]
        [StringLength(200, MinimumLength = 1)]
        public string Name { get; set; } = null!;

        public string? PlanCode { get; set; }

        [Required]
        public RegisterTenantOwnerRequest Owner { get; set; } = null!;

        private static readonly HashSet<string> AllowedPlanCodes = new(StringComparer.OrdinalIgnoreCase)
        {
            "starter",
            "growth",
            "premium"
        };

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(PlanCode))
                yield break;

            var normalizedPlanCode = PlanCode.Trim().ToLowerInvariant();
            if (!AllowedPlanCodes.Contains(normalizedPlanCode))
            {
                yield return new ValidationResult(
                    "PlanCode must be one of: starter, growth, premium.",
                    [nameof(PlanCode)]);
            }
        }
    }

    public sealed class RegisterTenantOwnerRequest
    {
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Name { get; set; } = null!;

        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Surname { get; set; } = null!;

        [Required]
        [EmailAddress]
        [StringLength(256)]
        public string Email { get; set; } = null!;

        [Required]
        [MinLength(5)]
        public string Password { get; set; } = null!;
    }
}