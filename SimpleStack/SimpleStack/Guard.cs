using System;

namespace SimpleStack
{
	public static class Guard
	{
		public static void NotNull(object value, string argument)
		{
			if (value == null)
			{
				throw new ArgumentNullException(argument);
			}
		}
	}
}