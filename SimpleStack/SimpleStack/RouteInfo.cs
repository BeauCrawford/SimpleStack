using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Autofac;

namespace SimpleStack
{
	public class RouteInfo
	{
		private static Regex routePropertyPattern = new Regex(@"\{([a-zA-Z0-9]+)\}", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		public static class Patterns
		{
			public const string Anything = "[a-zA-Z0-9]+";
			public const string GroupedInteger = "([0-9]+)";
			public const string GroupedString = "([a-zA-Z0-9]+)";
		}

		public static class Messages
		{
			public const string InvalidParameterCount = "Invalid Parameter Count";
		}

		public static RouteParseResult Parse(string route, Type requestType)
		{
			Guard.NotNull(route, "route");
			Guard.NotNull(requestType, "requestType");

			var groups = new Dictionary<int, PropertyInfo>();

			var routePattern = new StringBuilder();

			routePattern.Append("/");

			var routeParts = route.Split('/').Where(p => !string.IsNullOrWhiteSpace(p));

			int groupCounter = 1;

			foreach (var part in routeParts)
			{
				var match = routePropertyPattern.Match(part);

				if (match.Success)
				{
					var property = requestType.GetProperty(match.Groups[1].Value, BindingFlags.Public | BindingFlags.Instance);

					if (property == null)
					{
						routePattern.Append(Patterns.Anything);
					}
					else
					{
						if (property.PropertyType == typeof(int))
						{
							routePattern.Append(Patterns.GroupedInteger);
						}
						else if (property.PropertyType == typeof(string))
						{
							routePattern.Append(Patterns.GroupedString);
						}
						else
						{
							throw new NotSupportedException("Property type not supported: " + property.PropertyType);
						}

						groups.Add(groupCounter, property);

						groupCounter++;
					}
				}
				else
				{
					routePattern.Append(part);
				}

				routePattern.Append("/");
			}

			var pattern = new Regex("^" + routePattern.ToString().TrimEnd('/') + "$", RegexOptions.IgnoreCase);

			return new RouteParseResult(pattern, groups);
		}

		public RouteInfo(string route, MethodInfo method)
		{
			Guard.NotNull(route, "route");
			Guard.NotNull(method, "method");

			var parameters = method.GetParameters();

			if (parameters.Length != 1)
			{
				throw new InvalidOperationException(Messages.InvalidParameterCount);
			}

			Route = route;
			Method = method;
			RequestType = parameters[0].ParameterType;
			Result = Parse(route, RequestType);
		}

		public RouteParseResult Result { get; private set; }
		public string Route { get; private set; }
		public MethodInfo Method { get; private set; }
		public Type RequestType { get; private set; }

		public bool IsMatch(HttpContextBase context)
		{
			return context != null
				&& context.Request != null
				&& !string.IsNullOrWhiteSpace(context.Request.Path)
				&& string.Compare(Method.Name, context.Request.HttpMethod, true) == 0
				&& Result.Pattern.IsMatch(context.Request.Path);
		}

		public object CreateRequest(IContainer container, HttpContextBase context)
		{
			var request = container.Resolve(RequestType);

			var match = Result.Pattern.Match(context.Request.Path);

			if (!match.Success)
			{
				throw new InvalidOperationException();
			}

			for (int i = 1; i < match.Groups.Count; i++)
			{
				var groupValue = match.Groups[i].Value;

				var property = Result.RouteGroups[i];

				if (property.PropertyType == typeof(int))
				{
					property.SetValue(request, int.Parse(groupValue));
				}
				else if (property.PropertyType == typeof(string))
				{
					property.SetValue(request, groupValue);
				}
				else
				{
					throw new InvalidOperationException();
				}
			}

			return request;
		}

		public override string ToString()
		{
			return Method.Name + ": " + Route;
		}
	}
}