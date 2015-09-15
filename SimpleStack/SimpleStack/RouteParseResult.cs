using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace SimpleStack
{
	public class RouteParseResult
	{
		public RouteParseResult(Regex pattern, Dictionary<int, PropertyInfo> routeGroups)
		{
			Guard.NotNull(pattern, "pattern");
			Guard.NotNull(routeGroups, "routeGroups");

			Pattern = pattern;
			RouteGroups = routeGroups;
		}

		public Regex Pattern { get; private set; }

		public IDictionary<int, PropertyInfo> RouteGroups { get; private set; }
	}
}