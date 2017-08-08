using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cake.Common.IO;
using Cake.Common.Tools.DotNetCore.NuGet.Delete;
using Cake.Core.IO;
using Cake.Helpers.Settings;

namespace Cake.Helpers.DotNetCore
{
  /// <summary>
  /// Configuration for DocFx build
  /// </summary>
  /// <inheritdoc />
  public interface IDocFxConfiguration : IDocConfiguration
  {
    /// <summary>
    /// Relative File Location to docfx.json file
    /// </summary>
    string DocFxFile { get; set; }
  }

  internal class DocFxConfiguration : DocConfiguration, IDocFxConfiguration
  {
    public string DocFxFile { get; set; }
  }

  /// <summary>
  /// DocFx Extensions
  /// </summary>
  public static class DocFxConfigurationExtensions
  {
    /// <summary>
    /// Gets DirectoryPath of DocFx file docfx.json
    /// </summary>
    /// <param name="config">DocFxConfig</param>
    /// <returns>DirectoryPath</returns>
    public static DirectoryPath GetDocFxDirectoryPath(this IDocFxConfiguration config)
    {
      if (config == null)
        throw new ArgumentNullException(nameof(config));

      if(config.Context == null)
        throw new ArgumentNullException(nameof(config.Context));

      var file = config.Context.File(config.DocFxFile);
      var relFile = config.Context.MakeRelative(file);
      var wd = relFile.GetDirectory();
      if(wd == null)
        throw new DirectoryNotFoundException("missing directory for DocFxFile");

      if(!config.Context.DirectoryExists(wd))
        throw new DirectoryNotFoundException("missing directory for DocFxFile");

      return config.Context.MakeAbsolute(wd);
    }

    /// <summary>
    /// Gets or Adds DocFx configuration to DotNetCore helper
    /// </summary>
    /// <param name="settings">DotNetCoreHelper</param>
    /// <param name="docFxFile">DocFxFile</param>
    /// <param name="config">DocFxConfiguration Setup</param>
    /// <returns>DocFxConfiguration</returns>
    public static IDocFxConfiguration GetDocFxConfiguration(
      this IDotNetCoreHelperSettings settings,
      string docFxFile,
      Action<IDocFxConfiguration> config = null)
    {
      if(settings == null)
        throw new ArgumentNullException(nameof(settings));

      var docFxConfig = new DocFxConfiguration()
      {
        DocFxFile = docFxFile,
      };
      
      config?.Invoke(docFxConfig);

      return settings.AddDocConfiguration(docFxConfig) as IDocFxConfiguration;
    }

    internal static IEnumerable<IDocFxConfiguration> DocFxConfigurations(this IDotNetCoreHelperSettings settings)
    {
      if(settings == null)
        throw new ArgumentNullException(nameof(settings));

      return settings.DocConfigs
        .OfType<IDocFxConfiguration>();
    }
  }
}
