using System;
using System.Collections.Generic;
using Autofac;

namespace SimpleStack
{
	internal static class InternalExtensions
	{
		public static T CreateInstance<T>(this IContainer container, params object[] args)
			where T : class
		{
			var parameters = new List<TypedParameter>();

			foreach (var arg in args)
			{
				parameters.Add(new TypedParameter(arg.GetType(), arg));
			}

			return container.Resolve<T>(parameters.ToArray());
		}

		public static Type GetUnderlyingType(this Type type)
		{

			bool nullableType = type.IsGenericType
				&& type.GetGenericTypeDefinition().Equals(typeof(Nullable<>));

			if (nullableType)
			{
				type = new System.ComponentModel.NullableConverter(type).UnderlyingType;
			}

			return type;
		}
	}
}
