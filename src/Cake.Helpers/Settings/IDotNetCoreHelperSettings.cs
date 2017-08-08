using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cake.Helpers.DotNetCore;

namespace Cake.Helpers.Settings
{
  /// <summary>
  /// DotNetCore Build Settings Contract
  /// </summary>
  /// <inheritdoc />
  public interface IDotNetCoreBuildHelperSettings : ISubSetting
  {
    /// <summary>
    /// Temporary PostBuild Temp Folder. Defaults to ./BuildTemp
    /// </summary>
    string BuildTempFolder { get; set; }
  }

  /// <summary>
  /// DotNetCore Test Settings Contract
  /// </summary>
  /// <inheritdoc />
  public interface IDotNetCoreTestHelperSettings : ISubSetting
  {
    /// <summary>
    /// Temporary Test Results folder. Defaults to ./TestTemp
    /// </summary>
    string TestTempFolder { get; set; }
    /// <summary>
    /// Project Name Filters that Defined Test Projects (Regex). Defaults to:
    ///   ".+\\.Test\\..*",
    ///   ".+\\.Tests\\..*"
    /// </summary>
    List<string> TestProjectNameFilters { get; }
  }

  /// <summary>
  /// DotNetCore Settings Contract
  /// </summary>
  /// <inheritdoc />
  public interface IDotNetCoreHelperSettings : ISubSetting
  {
    /// <summary>
    /// Nuget Settings
    /// </summary>
    INugetHelperSettings NugetSettings { get; }
    /// <summary>
    /// Build Settings
    /// </summary>
    IDotNetCoreBuildHelperSettings BuildSettings { get; }
    /// <summary>
    /// Test Settings
    /// </summary>
    IDotNetCoreTestHelperSettings TestSettings { get; }

    /// <summary>
    /// Projects Defined
    /// </summary>
    IEnumerable<IProjectConfiguration> Projects { get; }
    /// <summary>
    /// Add project for sln file
    /// </summary>
    /// <param name="slnFile">Relative path to sln file</param>
    /// <param name="projectConfig">ProjectConfiguration setup</param>
    /// <returns>ProjectConfiguration</returns>
    /// <example>
    /// <code>
    /// HelperSettings.DotNetCoreSettings.AddProject("./my.sln", config => 
    /// {
    ///   config.Framework = "net452";
    /// });
    /// </code>
    /// </example>
    IProjectConfiguration AddProject(string slnFile, Action<IProjectConfiguration> projectConfig = null);

    /// <summary>
    /// Documentation Configs Defined
    /// </summary>
    IEnumerable<IDocConfiguration> DocConfigs { get; }

    /// <summary>
    /// Adds Documentation Configuration
    /// </summary>
    /// <param name="docConfig">doc config to add</param>
    /// <returns>DocConfiguration</returns>
    IDocConfiguration AddDocConfiguration(IDocConfiguration docConfig);
  }
}
