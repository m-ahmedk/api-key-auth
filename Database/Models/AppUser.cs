using System;
using System.Collections.Generic;

namespace Database.Models;

public partial class AppUser
{
    public int UserId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public DateTime? BirthDate { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? Zipcode { get; set; }

    public DateTime? CreatedAt { get; set; }
}
