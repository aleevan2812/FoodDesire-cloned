﻿namespace FoodDesire.Models;
public class User : TrackedEntity {
    [Required]
    public string? FirstName { get; set; }
    [Required]
    public string? LastName { get; set; }
    [AllowNull]
    public DateTime? DateOfBirth {
        get => _dateOfBirth;
        set {
            _dateOfBirth = value;
            if (value == null) return;
            if (DateTime.Now.Year - value.Value.Year == 0) return;
            Age = DateTime.Now.AddYears(-value.Value.Year).Year;
        }
    }
    private DateTime? _dateOfBirth { get; set; }
    [AllowNull]
    public int? Age { get; private set; }
    [AllowNull]
    public Gender? Gender { get; set; }
    [AllowNull]
    public string PhoneNumber { get; set; } = "0705624764";
    [NotMapped]
    public Address Address { get; set; } = new();
    [Required]
    public int AccountId { get; set; }


    [ForeignKey(nameof(AccountId))]
    public Account? Account { get; set; }
}

public enum Gender {
    [Display(Name = "Male")]
    Male,
    [Display(Name = "Female")]
    Female
}
