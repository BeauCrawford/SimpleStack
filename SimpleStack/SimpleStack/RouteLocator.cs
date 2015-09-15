using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace SimpleStack
{
	public class RouteLocator
	{
		public RouteLocator()
		{
		}

		public IEnumerable<RouteInfo> FindAll(params Assembly[] assemblies)
		{
			var routes = new List<RouteInfo>();

			foreach (var assembly in assemblies)
			{
				var types = assembly.GetTypes();

				var allowedMethodNames = new[] { HttpMethods.Any, HttpMethods.Post, HttpMethods.Get, HttpMethods.Put, HttpMethods.Delete };

				foreach (var type in types)
				{
					foreach (var serviceMethod in type.GetMethods(BindingFlags.Public | BindingFlags.Instance).Where(m => m.ReturnType != typeof(void)))
					{
						if (allowedMethodNames.Any(m => string.Compare(m, serviceMethod.Name, true) == 0))
						{
							var parameters = serviceMethod.GetParameters();

							if (parameters.Length == 1)
							{
								var parameterType = parameters[0].ParameterType;

								var attributes = parameterType.GetCustomAttributes(typeof(RouteAttribute), true) as RouteAttribute[];

								foreach (var attribute in attributes)
								{
									routes.Add(new RouteInfo(attribute.Route, serviceMethod));
								}
							}
						}
					}
				}
			}

			return new ReadOnlyCollection<RouteInfo>(routes);
		}
	}
}
