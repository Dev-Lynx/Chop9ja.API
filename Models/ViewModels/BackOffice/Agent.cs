
using Chop9ja.API.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Chop9ja.API.Models.ViewModels.BackOffice
{
    public class AgentViewModel
    {
		[Required]
		public Guid Id { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }

        public string Initials => (string.IsNullOrWhiteSpace(LastName) && string.IsNullOrWhiteSpace(FirstName)) ? $"{LastName.FirstOrDefault()}{FirstName.FirstOrDefault()}" : "";

        [Required]
        public DateTime DateOfBirth { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public string StateOfOrigin { get; set; }

        [Phone]
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string PhoneNumberConfirmed { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string EmailConfirmed { get; set; }

        public bool LockoutEnabled { get; set; }

        public string Address { get; set; }
        public string Landmark { get; set; }
        public string LocalGovernment { get; set; }
		
		public AgentValidationStatus ValidationStatus { get; set; }
    }

    public class AgentValidationStatusViewModel
    {
        public Guid Id { get; set; }
        public AgentValidationStatus Status { get; set; }
        public DateTime InspectionDate { get; set; }
    }
}
