using System;
using System.Collections.Generic;
using System.IO;
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
		#region Static

		private static Regex routePropertyPattern = new Regex(@"\{([a-zA-Z0-9]+)\}", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		public static object Convert(Type targetType, string value)
		{
			Guard.NotNull(targetType, "targetType");

			var converter = System.ComponentModel.TypeDescriptor.GetConverter(targetType);
			return converter.ConvertFrom(value);
		}

		public static string GetRequestBody(HttpContextBase http, Type requestType, ContentType requestContentType)
		{
			Guard.NotNull(http, "http");
			Guard.NotNull(requestType, "requestType");
			Guard.NotNull(requestContentType, "requestContentType");

			string body = null;

			if (http.Request != null && http.Request.InputStream != null)
			{
				using (var stream = http.Request.InputStream)
				{
					using (var reader = new StreamReader(stream, true))
					{
						body = reader.ReadToEnd();
					}
				}
			}

			if (string.IsNullOrWhiteSpace(body))
			{
				return requestContentType.GetDefaultBody(requestType);
			}
			else
			{
				return body;
			}
		}

		public static class Patterns
		{
			public const string Anything = "[^/]+";
			public const string GroupedAnything = "([^/]+)";
			public const string GroupedInteger = "([0-9]+)";
			public const string GroupedString = "([^/]+)";
		}

		public static class Messages
		{
			public const string InvalidParameterCount = "Invalid Parameter Count";
			public const string RequestNotMatched = "Request is not a match for this Route";
		}

		private static readonly Dictionary<Type, string> propertyPatterns = new Dictionary<Type, string>();

		static RouteInfo()
		{
			propertyPatterns.Add(typeof(int), Patterns.GroupedInteger);
			propertyPatterns.Add(typeof(string), Patterns.GroupedString);
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
						var propertyType = property.PropertyType;

						if (propertyPatterns.ContainsKey(propertyType))
						{
							routePattern.Append(propertyPatterns[propertyType]);
						}
						else
						{
							routePattern.Append(Patterns.GroupedAnything);
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

		#endregion

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

		public bool IsMatch(HttpContextBase http)
		{
			return http != null
				&& http.Request != null
				&& !string.IsNullOrWhiteSpace(http.Request.Path)
				&& (string.Compare(HttpMethods.Any, Method.Name, true) == 0 || string.Compare(Method.Name, http.Request.HttpMethod, true) == 0)
				&& Result.Pattern.IsMatch(http.Request.Path);
		}

		public object CreateRequest(IContainer container, HttpContextBase http, ContentType requestContentType)
		{
			Guard.NotNull(container, "container");
			Guard.NotNull(http, "http");
			Guard.NotNull(requestContentType, "requestContentType");

			var match = Result.Pattern.Match(http.Request.Path);

			if (!match.Success)
			{
				throw new InvalidOperationException(Messages.RequestNotMatched);
			}

			var body = GetRequestBody(http, RequestType, requestContentType);
			var serializer = requestContentType.CreateSerializer();
			var request = serializer.Deserialize(RequestType, body);

			for (int i = 1; i < match.Groups.Count; i++)
			{
				var groupValue = match.Groups[i].Value;
				var property = Result.RouteGroups[i];
				var convertedValue = Convert(property.PropertyType, groupValue);
				property.SetValue(request, convertedValue);
			}

			return request;
		}

		public override string ToString()
		{
			return Method.Name + ": " + Route;
		}
	}
}