﻿using Microsoft.AspNetCore.Identity;

namespace TT.Server.Features.Identity;

public class User : IdentityUser
{
    public DateTime? CreateDate { get; set; }
    public string ConfirmationToken { get; set; }
    public bool? IsConfirmed { get; set; }
    public DateTime? LastPasswordFailureDate { get; set; }
    public int? PasswordFailuresSinceLastSuccess { get; set; }
    public DateTime? PasswordChangedDate { get; set; }
    public string PasswordVerificationToken { get; set; }
    public DateTime? PasswordVerificationTokenExpirationDate { get; set; }
}