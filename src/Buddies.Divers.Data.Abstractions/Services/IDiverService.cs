using Builders.Divers.Abstractions.Entities;

namespace Buddies.Divers.Data.Abstractions.Services
{
	public interface IDiverService
	{
		/// <summary>
		/// Persists changes to an <see cref="Account"/> entity.
		/// </summary>
		/// <param name="diverId">Diver ID</param>
		/// <param name="account">Account</param>
		Task CreateOrUpdateAsync(string diverId, Account account);

		/// <summary>
		/// Deletes a diver's account.
		/// </summary>
		/// <param name="diverId">Diver ID</param>
		Task DeleteAsync(string diverId);
	}
}
