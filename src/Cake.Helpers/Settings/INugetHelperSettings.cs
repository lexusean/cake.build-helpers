using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cake.Helpers.Nuget;

namespace Cake.Helpers.Settings
{
  /// <summary>
  /// Nuget Settings Contract
  /// </summary>
  /// <inheritdoc />
  public interface INugetHelperSettings : IHelperContext
  {
    /// <summary>
    /// Nuget Sources. Defaults to { FeedName = "NugetV3", FeedSource = "https://api.nuget.org/v3/index.json" } 
    /// </summary>
    IEnumerable<INugetSource> NugetSources { get; }
    /// <summary>
    /// Adds Nuget Source by URI only
    /// </summary>
    /// <param name="feedUrl">Nuget API Uri</param>
    /// <returns>NugetSource</returns>
    INugetSource AddSource(string feedUrl);
    /// <summary>
    /// Add Nuget Source with name
    /// </summary>
    /// <param name="feedName">Nuget Source Name</param>
    /// <param name="feedUrl">Nuget API URI</param>
    /// <param name="sourceConfig">NugetSource Setup</param>
    /// <returns>NugetSource</returns>
    /// <example>
    /// <code>
    /// HelperSettings.DotNetCoreSettings.NugetSettings.AddSource("myget", "http://myget.org", config => 
    ///   {
    ///     config.IsSecure = true;
    ///   });
    /// </code>
    /// </example>
    INugetSource AddSource(string feedName, string feedUrl, Action<INugetSource> sourceConfig = null);
  }

  /// <summary>
  /// Nuget Settings Extensions
  /// </summary>
  public static class NugetHelperSettingsExtensions
  {
    /// <summary>
    /// Gets all Nuget Source URIs
    /// </summary>
    /// <param name="settings">NugetHelperSettings</param>
    /// <returns>All Nuget Source URIs</returns>
    public static IEnumerable<string> GetFeedUrls(this INugetHelperSettings settings)
    {
      if(settings == null)
        throw new ArgumentNullException(nameof(settings));

      return settings.NugetSources
        .Select(t => t.FeedSource)
        .Distinct();
    }
  }
}
