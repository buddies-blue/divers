using Builders.Divers.Abstractions.Entities.Enums;
using Builders.Divers.Abstractions.Helpers;

namespace Builders.Divers.Abstractions.Entities
{
	/// <summary>
	/// Diver profile
	/// </summary>
	public record class Profile
	{
		// Backing Fields
		private string? _firstName;
		private string? _lastName;
		private DateTime? _dateOfBirth;

		/// <summary>
		/// First name
		/// </summary>
		public string FirstName
		{
			get => AssertionHelpers.NotNull(_firstName, "First name");
			set => _firstName = value;
		}

		/// <summary>
		/// Last name
		/// </summary>
		public string LastName
		{
			get => AssertionHelpers.NotNull(_lastName, "Last name");
			set => _lastName = value;
		}

		/// <summary>
		/// Date of birth
		/// </summary>
		public DateTime DateOfBirth
		{
			get => AssertionHelpers.NotNull(_dateOfBirth, "Date of birth");
			set => _dateOfBirth = value;
		}

		/// <summary>
		/// Gender identity
		/// </summary>
		public GenderType? Gender { get; set; }
	}
}
