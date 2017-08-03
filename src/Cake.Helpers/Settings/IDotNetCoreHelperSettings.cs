using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cake.Helpers.DotNetCore;

namespace Cake.Helpers.Settings
{
  public interface IDotNetCoreBuildHelperSettings : ISubSetting
  {
    string BuildTempFolder { get; set; }
  }

  public interface IDotNetCoreTestHelperSettings : ISubSetting
  {
    string TestTempFolder { get; set; }
    List<string> TestProjectNameFilters { get; }
  }

  public interface IDotNetCoreHelperSettings : ISubSetting
  {
    INugetHelperSettings NugetSettings { get; }
    IDotNetCoreBuildHelperSettings BuildSettings { get; }
    IDotNetCoreTestHelperSettings TestSettings { get; }

    IEnumerable<IProjectConfiguration> Projects { get; }
    IProjectConfiguration AddProject(string slnFile, Action<IProjectConfiguration> projectConfig = null);
  }
}
