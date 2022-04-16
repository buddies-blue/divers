using Buddies.Divers.Grains.Abstractions;
using Builders.Divers.Abstractions.Entities;
using Orleans;

namespace Buddies.Divers.Grains
{
	internal class DiverGrain : Grain, IDiverGrain
	{
		#region Properties

		/// <summary>
		/// Diver identifier
		/// </summary>
		public string DiverID { get; private set; } = null!;

		/// <summary>
		/// Diver account and profile
		/// </summary>
		public Account? Account { get; private set; }

		#endregion

		#region Grain Lifetime

		public override async Task OnActivateAsync()
		{
			await base.OnActivateAsync();

			// Retrieve identifier
			DiverID = this.GetPrimaryKeyString();

			// TODO - Get diver account and profile from database
		}

		#endregion

		public async Task CreateAsync(Account account)
		{
			// Ensure that diver does not exist
			if (Account is not null)
			{
				throw new InvalidOperationException($"Account '{DiverID}' already exists.");
			}

			// Update account record
			Account = account;

			// Persist changes
			await CreateOrUpdateAsync();
		}

		public async Task UpdateProfileAsync(Profile profile)
		{
			// Ensure that diver exists
			if (Account is null)
			{
				throw new InvalidOperationException($"Account '{DiverID}' does not exist.");
			}

			// Update profile record
			Account.Profile = profile;

			// Persist changes
			await CreateOrUpdateAsync();
		}

		public Task DeleteAsync()
		{
			// Ensure that diver exists
			if (Account is null)
			{
				throw new InvalidOperationException($"Account '{DiverID}' does not exist.");
			}

			throw new NotImplementedException();
		}

		private async Task CreateOrUpdateAsync()
		{
			throw new NotImplementedException();
		}
	}
}
