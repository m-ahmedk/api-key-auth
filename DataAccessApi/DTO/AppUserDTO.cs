namespace DataAccessApi.DTO
{
    public class AppUserDTO : AppUser
    {
        [SwaggerIgnore]
        public int UserId { get; set; }

        /// <summary>
        ///  First name of the user
        /// </summary>
        /// <example> Muhammad Ahmed Villa Khan </example>
        public string? FirstName { get; set; }

        /// <summary>
        ///  Last name of the user
        /// </summary>
        /// <example> Khan </example>
        public string? LastName { get; set; }

        /// <summary>
        /// Birthdate of the user
        /// </summary>
        /// <example>1995-03-12T00:00:00.000Z</example>
        public DateTime? BirthDate { get; set; }

        /// <summary>
        /// User's email
        /// </summary>
        /// <example>m.ahmedk287@gmail.com</example>
        public string? Email { get; set; }

        /// <summary>
        /// User's phone number
        /// </summary>
        /// <example>+923418380518</example>
        public string? Phone { get; set; }

        /// <summary>
        /// User's zipcode
        /// </summary>
        /// <example>75500</example>
        public string? Zipcode { get; set; }

        [SwaggerIgnore]
        public DateTime? CreatedAt { get; set; }
    }
}
