using Builders.Divers.Abstractions.Entities;
using Orleans;

namespace Buddies.Divers.Grains.Abstractions
{
	/// <summary>
	/// A grain that represents a diver that's responsible
	/// for performing CRUD operations.
	/// </summary>
	public interface IDiverGrain : IGrainWithStringKey
	{
		/// <summary>
		/// Creates an account for the diver if it doesn't exist.
		/// </summary>
		/// <param name="account">Diver account</param>
		/// <remarks>
		/// Throws an <see cref="InvalidOperationException"/>
		/// when the diver is already associated with an account.
		/// </remarks>
		Task CreateAsync(Account account);

		/// <summary>
		/// Updates the diver's profile.
		/// </summary>
		/// <param name="profile">Diver profile</param>
		/// <remarks>
		/// Throws an <see cref="InvalidOperationException"/>
		/// when the profile does not reflect that of an
		/// existing diver.
		/// </remarks>
		Task UpdateProfileAsync(Profile profile);

		/// <summary>
		/// Deletes the diver's account from the system.
		/// </summary>
		/// <remarks>
		/// Throws an <see cref="InvalidOperationException"/>
		/// when the diver does not exist.
		/// </remarks>
		Task DeleteAsync();
	}
}
