namespace Builders.Divers.Abstractions.Entities
{
	/// <summary>
	/// Diver account
	/// </summary>
	public record class Account
	{
		/// <summary>
		/// Email address
		/// </summary>
		public string? EmailAddress { get; set; }

		/// <summary>
		/// Facebook OAuth ID
		/// </summary>
		public string? FacebookID { get; set; }

		/// <summary>
		/// Google OAuth ID
		/// </summary>
		public string? GoogleID { get; set; }

		/// <summary>
		/// Diver profile
		/// </summary>
		public Profile? Profile { get; set; }
	}
}
