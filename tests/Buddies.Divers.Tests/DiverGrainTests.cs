using Buddies.Divers.Data.Abstractions.Services;
using Buddies.Divers.Grains.Abstractions;
using Builders.Divers.Abstractions.Entities;
using Builders.Divers.Abstractions.Entities.Enums;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Orleans;
using Orleans.Hosting;
using Orleans.TestingHost;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Buddies.Divers.Tests
{
	public class DiverGrainTests : IAsyncLifetime
	{
		protected TestCluster Cluster { get; private set; } = null!;

		protected IGrainFactory GrainFactory => Cluster.GrainFactory;

		protected static IDiverService DiverService => DiverGrainTests_SiloBuilderConfigurator.DiverService;

		#region CreateAsync

		[Fact]
		public async Task DiverGrain_CreateAsync()
		{
			const string diverId = "DIVER_1";

			// Configure diver service
			_ = DiverService.GetAsync(diverId).Returns(Task.FromResult<Account?>(null));

			var account = new Account
			{
				EmailAddress = "john.doe@foo.bar",
				Profile = new Profile
				{
					DateOfBirth = new DateTime(1990, 1, 1),
					FirstName = "John",
					LastName = "Doe",
					Gender = GenderType.Male,
				},
			};

			// Create diver account
			var diverGrain = GrainFactory.GetGrain<IDiverGrain>(diverId);
			await diverGrain.CreateAsync(account);

			// Assert single call to driver service
			await DiverService.Received(1).CreateOrUpdateAsync(diverId, account);
		}

		[Fact]
		public async Task DiverGrain_CreateAsync_AlreadyExists()
		{
			const string diverId = "DIVER_1";

			// Configure diver service
			_ = DiverService.GetAsync(diverId).Returns(Task.FromResult<Account?>(null));

			var account = new Account
			{
				EmailAddress = "john.doe@foo.bar",
				Profile = new Profile
				{
					DateOfBirth = new DateTime(1990, 1, 1),
					FirstName = "John",
					LastName = "Doe",
					Gender = GenderType.Male,
				},
			};

			// Create diver account
			var diverGrain = GrainFactory.GetGrain<IDiverGrain>(diverId);
			await diverGrain.CreateAsync(account);

			// Assert that an exception is thrown when an account already exists
			var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
			{
				await diverGrain.CreateAsync(account);
			});

			Assert.Equal($"Account '{diverId}' already exists.", exception.Message);

			// Assert single call to driver service (1 for initial creation)
			await DiverService.Received(1).CreateOrUpdateAsync(diverId, account);
		}

		#endregion

		#region UpdateProfileAsync

		[Fact]
		public async Task DiverGrain_UpdateProfileAsync()
		{
			const string diverId = "DIVER_1";

			var account = new Account
			{
				EmailAddress = "john.doe@foo.bar",
				Profile = new Profile
				{
					DateOfBirth = new DateTime(1990, 1, 1),
					FirstName = "John",
					LastName = "Doe",
					Gender = GenderType.Male,
				},
			};

			var updatedProfile = new Profile
			{
				DateOfBirth = new DateTime(1992, 1, 1),
				FirstName = "Jane",
				LastName = "Sparkles",
				Gender = GenderType.Female,
			};

			// Configure diver service
			_ = DiverService.GetAsync(diverId).Returns(Task.FromResult<Account?>(account));

			// Update diver profile
			var diverGrain = GrainFactory.GetGrain<IDiverGrain>(diverId);
			await diverGrain.UpdateProfileAsync(updatedProfile);

			// Assert single call to driver service
			await DiverService.Received(1).CreateOrUpdateAsync(diverId, account);

			// Assert that the profile was updated
			Assert.Equal(updatedProfile, account.Profile);
		}

		[Fact]
		public async Task DiverGrain_UpdateProfileAsync_NotExists()
		{
			const string diverId = "DIVER_1";

			var updatedProfile = new Profile
			{
				DateOfBirth = new DateTime(1992, 1, 1),
				FirstName = "Jane",
				LastName = "Sparkles",
				Gender = GenderType.Female,
			};

			// Configure diver service
			_ = DiverService.GetAsync(diverId).Returns(Task.FromResult<Account?>(null));

			// Update diver profile
			var diverGrain = GrainFactory.GetGrain<IDiverGrain>(diverId);

			// Assert that an exception is thrown when an account does not exist
			var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
			{
				await diverGrain.UpdateProfileAsync(updatedProfile);
			});

			Assert.Equal($"Account '{diverId}' does not exist.", exception.Message);

			// Assert no call to driver service
			await DiverService.DidNotReceive().CreateOrUpdateAsync(diverId, Arg.Any<Account>());
		}

		#endregion

		#region DeleteAsync

		[Fact]
		public async Task DiverGrain_DeleteAsync()
		{
			const string diverId = "DIVER_1";

			var account = new Account
			{
				EmailAddress = "john.doe@foo.bar",
				Profile = new Profile
				{
					DateOfBirth = new DateTime(1990, 1, 1),
					FirstName = "John",
					LastName = "Doe",
					Gender = GenderType.Male,
				},
			};

			// Configure diver service
			_ = DiverService.GetAsync(diverId).Returns(Task.FromResult<Account?>(account));

			// Update diver profile
			var diverGrain = GrainFactory.GetGrain<IDiverGrain>(diverId);
			await diverGrain.DeleteAsync();

			// Assert single call to driver service
			await DiverService.Received(1).DeleteAsync(diverId);
		}

		[Fact]
		public async Task DiverGrain_DeleteAsync_NotExists()
		{
			const string diverId = "DIVER_1";

			var account = new Account
			{
				EmailAddress = "john.doe@foo.bar",
				Profile = new Profile
				{
					DateOfBirth = new DateTime(1990, 1, 1),
					FirstName = "John",
					LastName = "Doe",
					Gender = GenderType.Male,
				},
			};

			// Configure diver service
			_ = DiverService.GetAsync(diverId).Returns(Task.FromResult<Account?>(null));

			// Update diver profile
			var diverGrain = GrainFactory.GetGrain<IDiverGrain>(diverId);

			// Assert that an exception is thrown when an account does not exist
			var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
			{
				await diverGrain.DeleteAsync();
			});

			Assert.Equal($"Account '{diverId}' does not exist.", exception.Message);

			// Assert no call to driver service
			await DiverService.DidNotReceive().DeleteAsync(diverId);
		}

		#endregion

		#region Lifetime

		public async Task InitializeAsync()
		{
			// Build cluster
			await CreateClusterAsync();
		}

		public async Task DisposeAsync()
		{
			// Stop silos and dispose of cluster
			await Cluster.StopAllSilosAsync();
			await Cluster.DisposeAsync();

			// Clear NSubstitute calls
			DiverService.ClearReceivedCalls();
		}

		#endregion

		#region Utilities

		private async Task CreateClusterAsync()
		{
			// Configure cluster builder
			var builder = new TestClusterBuilder();
			_ = builder.AddSiloBuilderConfigurator<DiverGrainTests_SiloBuilderConfigurator>();

			// Build cluster
			Cluster = builder.Build();

			// Deploy cluster
			await Cluster.DeployAsync();
		}

		#endregion
	}

	internal class DiverGrainTests_SiloBuilderConfigurator : ISiloConfigurator
	{
		public static IDiverService DiverService = Substitute.For<IDiverService>();

		public void Configure(ISiloBuilder builder)
		{
			_ = builder.ConfigureServices(services =>
			{
				_ = services.AddSingleton(_ => DiverService);
			});
		}
	}
}
