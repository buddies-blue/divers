using Buddies.Divers.Data.Abstractions.Services;
using Buddies.Divers.Grains.Abstractions;
using Builders.Divers.Abstractions.Entities;
using Orleans;

namespace Buddies.Divers.Grains
{
	internal class DiverGrain : Grain, IDiverGrain
	{
		private readonly IDiverService _diverService;

		public DiverGrain(IDiverService diverService)
		{
			_diverService = diverService;
		}

		#region Properties

		/// <summary>
		/// Diver identifier
		/// </summary>
		public string DiverId { get; private set; } = null!;

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
			DiverId = this.GetPrimaryKeyString();

			// TODO - Get diver account and profile from database
		}

		#endregion

		public async Task CreateAsync(Account account)
		{
			// Ensure that diver does not exist
			if (Account is not null)
			{
				throw new InvalidOperationException($"Account '{DiverId}' already exists.");
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
				throw new InvalidOperationException($"Account '{DiverId}' does not exist.");
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
				throw new InvalidOperationException($"Account '{DiverId}' does not exist.");
			}

			// Delete diver account
			return _diverService.DeleteAsync(DiverId);
		}

		private Task CreateOrUpdateAsync()
		{
			// Ensure that diver exists
			if (Account is null)
			{
				throw new InvalidOperationException($"Account '{DiverId}' does not exist.");
			}

			// Create or update diver account
			return _diverService.CreateOrUpdateAsync(DiverId, Account);
		}
	}
}
