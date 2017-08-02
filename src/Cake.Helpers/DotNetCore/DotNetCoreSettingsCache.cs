using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cake.Helpers.DotNetCore
{
  public static class DotNetCoreSettingsCache
  {
    public static string BuildTempFolder { get; set; } = "./BuildTemp";
    public static string TestTempFolder { get; set; } = "./TestTemp";

    public static List<string> TestProjectNameFilters { get; } = new List<string>(new string[]
    {
      "*.Test.*",
      "*.Tests.*"
    });

    public static List<string> NugetSources { get; } = new List<string>(new string[]
    {
      "https://api.nuget.org/v3/index.json"
    });
  }
}
