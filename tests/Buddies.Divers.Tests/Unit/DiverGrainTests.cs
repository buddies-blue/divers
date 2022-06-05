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

namespace Buddies.Divers.Tests.Unit
{
	public class DiverGrainTests : IClassFixture<DiverGrainFixture>, IAsyncLifetime
	{
		private readonly DiverGrainFixture _fixture;
		private readonly string _diverId = Guid.NewGuid().ToString("N");

		public DiverGrainTests(DiverGrainFixture fixture)
		{
			_fixture = fixture;
		}

		#region CreateAsync

		[Fact]
		public async Task DiverGrain_CreateAsync()
		{
			// Configure diver service
			_ = _fixture.DiverService.GetAsync(_diverId).Returns(Task.FromResult<Account?>(null));

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
			var diverGrain = _fixture.GrainFactory.GetGrain<IDiverGrain>(_diverId);
			await diverGrain.CreateAsync(account);

			// Assert single call to driver service
			await _fixture.DiverService.Received(1).CreateOrUpdateAsync(_diverId, account);
		}

		[Fact]
		public async Task DiverGrain_CreateAsync_AlreadyExists()
		{
			// Configure diver service
			_ = _fixture.DiverService.GetAsync(_diverId).Returns(Task.FromResult<Account?>(null));

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
			var diverGrain = _fixture.GrainFactory.GetGrain<IDiverGrain>(_diverId);
			await diverGrain.CreateAsync(account);

			// Assert that an exception is thrown when an account already exists
			var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
			{
				await diverGrain.CreateAsync(account);
			});

			Assert.Equal($"Account '{_diverId}' already exists.", exception.Message);

			// Assert single call to driver service (1 for initial creation)
			await _fixture.DiverService.Received(1).CreateOrUpdateAsync(_diverId, account);
		}

		#endregion

		#region UpdateProfileAsync

		[Fact]
		public async Task DiverGrain_UpdateProfileAsync()
		{
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
			_ = _fixture.DiverService.GetAsync(_diverId).Returns(Task.FromResult<Account?>(account));

			// Update diver profile
			var diverGrain = _fixture.GrainFactory.GetGrain<IDiverGrain>(_diverId);
			await diverGrain.UpdateProfileAsync(updatedProfile);

			// Assert single call to driver service
			await _fixture.DiverService.Received(1).CreateOrUpdateAsync(_diverId, account);

			// Assert that the profile was updated
			Assert.Equal(updatedProfile, account.Profile);
		}

		[Fact]
		public async Task DiverGrain_UpdateProfileAsync_NotExists()
		{
			var updatedProfile = new Profile
			{
				DateOfBirth = new DateTime(1992, 1, 1),
				FirstName = "Jane",
				LastName = "Sparkles",
				Gender = GenderType.Female,
			};

			// Configure diver service
			_ = _fixture.DiverService.GetAsync(_diverId).Returns(Task.FromResult<Account?>(null));

			// Update diver profile
			var diverGrain = _fixture.GrainFactory.GetGrain<IDiverGrain>(_diverId);

			// Assert that an exception is thrown when an account does not exist
			var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
			{
				await diverGrain.UpdateProfileAsync(updatedProfile);
			});

			Assert.Equal($"Account '{_diverId}' does not exist.", exception.Message);

			// Assert no call to driver service
			await _fixture.DiverService.DidNotReceive().CreateOrUpdateAsync(_diverId, Arg.Any<Account>());
		}

		#endregion

		#region DeleteAsync

		[Fact]
		public async Task DiverGrain_DeleteAsync()
		{
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
			_ = _fixture.DiverService.GetAsync(_diverId).Returns(Task.FromResult<Account?>(account));

			// Update diver profile
			var diverGrain = _fixture.GrainFactory.GetGrain<IDiverGrain>(_diverId);
			await diverGrain.DeleteAsync();

			// Assert single call to driver service
			await _fixture.DiverService.Received(1).DeleteAsync(_diverId);
		}

		[Fact]
		public async Task DiverGrain_DeleteAsync_NotExists()
		{
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
			_ = _fixture.DiverService.GetAsync(_diverId).Returns(Task.FromResult<Account?>(null));

			// Update diver profile
			var diverGrain = _fixture.GrainFactory.GetGrain<IDiverGrain>(_diverId);

			// Assert that an exception is thrown when an account does not exist
			var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
			{
				await diverGrain.DeleteAsync();
			});

			Assert.Equal($"Account '{_diverId}' does not exist.", exception.Message);

			// Assert no call to driver service
			await _fixture.DiverService.DidNotReceive().DeleteAsync(_diverId);
		}

		#endregion

		#region Lifetime

		public Task InitializeAsync()
		{
			return Task.CompletedTask;
		}

		public Task DisposeAsync()
		{
			// Clear NSubstitute calls
			_fixture.DiverService.ClearReceivedCalls();

			return Task.CompletedTask;
		}

		#endregion
	}

	public class DiverGrainFixture : IAsyncLifetime
	{
		public TestCluster Cluster { get; private set; } = null!;

		public IGrainFactory GrainFactory => Cluster.GrainFactory;

#pragma warning disable CA1822 // Mark members as static
		public IDiverService DiverService => DiverGrainTests_SiloBuilderConfigurator.DiverService;
#pragma warning restore CA1822 // Mark members as static

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

		private class DiverGrainTests_SiloBuilderConfigurator : ISiloConfigurator
		{
			public static readonly IDiverService DiverService = Substitute.For<IDiverService>();

			public void Configure(ISiloBuilder builder)
			{
				_ = builder.ConfigureServices(services =>
				{
					_ = services.AddSingleton(_ => DiverService);
				});
			}
		}
	}
}
