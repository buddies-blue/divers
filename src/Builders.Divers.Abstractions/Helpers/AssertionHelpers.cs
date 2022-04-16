namespace Builders.Divers.Abstractions.Helpers
{
	internal static class AssertionHelpers
	{
		/// <summary>
		/// Throws an <see cref="InvalidOperationException"/> when
		/// <paramref name="memberName"/> has a null value.
		/// </summary>
		/// <typeparam name="T">Type of underlying member</typeparam>
		/// <param name="value">Value of member</param>
		/// <param name="memberName">User-friendly name of member</param>
		/// <returns>Value of member when not null.</returns>
		/// <exception cref="InvalidOperationException"></exception>
		public static T NotNull<T>(T? value, string memberName)
		{
			return value ?? throw new InvalidOperationException($"'{memberName}' is required.");
		}

		/// <summary>
		/// Throws an <see cref="InvalidOperationException"/> when
		/// <paramref name="memberName"/> has a null value.
		/// </summary>
		/// <typeparam name="T">Type of underlying member</typeparam>
		/// <param name="value">Value of member</param>
		/// <param name="memberName">User-friendly name of member</param>
		/// <returns>Value of member when not null.</returns>
		/// <exception cref="InvalidOperationException"></exception>
		public static T NotNull<T>(T? value, string memberName)
			where T : struct
		{
			return value ?? throw new InvalidOperationException($"'{memberName}' is required.");
		}
	}
}
