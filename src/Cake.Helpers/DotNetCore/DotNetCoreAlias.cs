using System;
using System.Collections.Generic;
using Cake.Common.IO;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.IO;
using Cake.Helpers.Nuget;
using Cake.Helpers.Settings;

namespace Cake.Helpers.DotNetCore
{
  /// <summary>
  /// Cake DotNetCore Task Aliases
  /// </summary>
  [CakeAliasCategory("Helper")]
  [CakeAliasCategory("DotNetCore")]
  public static class DotNetCoreAlias
  {
    #region Static Members

    /// <summary>
    /// Gets DotNetCore Settings as CakeProperty. This is where you would setup your project information
    /// </summary>
    /// <param name="context">Cake Context</param>
    /// <returns>DotNetCoreHelperSettings</returns>
    /// <example>
    /// <code>
    /// var settings = DotNetCoreHelperSettings;
    /// var slnProj = settings.AddProject("./my.sln", config => 
    ///   {
    ///     config.Framework = "net452";
    ///   });
    /// </code>
    /// </example>
    [CakePropertyAlias]
    public static IDotNetCoreHelperSettings DotNetCoreHelperSettings(this ICakeContext context)
    {
      if (context == null)
        throw new ArgumentNullException(nameof(context));

      SingletonFactory.Context = context;
      return SingletonFactory.GetHelperSettings().DotNetCoreSettings;
    }

    /// <summary>
    /// Gets DotNetCore Build Settings as CakeProperty. This is where you would setup your project information for builds
    /// </summary>
    /// <param name="context">Cake Context</param>
    /// <returns>DotNetCoreBuildHelperSettings</returns>
    /// <example>
    /// <code>
    /// var settings = DotNetCoreBuildHelperSettings;
    /// settings.BuildTempFolder = "./temp/Build";
    /// </code>
    /// </example>
    [CakePropertyAlias]
    public static IDotNetCoreBuildHelperSettings DotNetCoreBuildHelperSettings(this ICakeContext context)
    {
      if (context == null)
        throw new ArgumentNullException(nameof(context));

      SingletonFactory.Context = context;
      return SingletonFactory.GetHelperSettings().DotNetCoreSettings.BuildSettings;
    }

    /// <summary>
    /// Gets DotNetCore Test Settings as CakeProperty. This is where you would setup your project information for tests
    /// </summary>
    /// <param name="context">Cake Context</param>
    /// <returns>DotNetCoreTestHelperSettings</returns>
    /// <example>
    /// <code>
    /// var settings = DotNetCoreTestHelperSettings;
    /// settings.TestTempFolder = "./temp/Test";
    /// settings.TestProjectNameFilters.Add(".+\\.MyTests\\..*");
    /// </code>
    /// </example>
    [CakePropertyAlias]
    public static IDotNetCoreTestHelperSettings DotNetCoreTestHelperSettings(this ICakeContext context)
    {
      if (context == null)
        throw new ArgumentNullException(nameof(context));

      SingletonFactory.Context = context;
      return SingletonFactory.GetHelperSettings().DotNetCoreSettings.TestSettings;
    }

    /// <summary>
    /// Add nuget source to DotNetCore for restore resolution
    /// </summary>
    /// <param name="context">Cake Context</param>
    /// <param name="feedSource">Feed Url</param>
    /// <param name="sourceConfig">Additional source config</param>
    /// <example>
    /// <code>
    /// AddDotNetCoreHelperNugetSource("http://myget.org", config => 
    ///   {
    ///     config.FeedName = "myget";
    ///   });
    /// </code>
    /// </example>
    [CakeMethodAlias]
    public static void AddDotNetCoreHelperNugetSource(this ICakeContext context, string feedSource, Action<INugetSource> sourceConfig = null)
    {
      if (context == null)
        throw new ArgumentNullException(nameof(context));

      if (string.IsNullOrWhiteSpace(feedSource))
        throw new ArgumentNullException(nameof(feedSource));

      var settings = context.DotNetCoreHelperSettings().NugetSettings;
      var source = settings.AddSource(feedSource);
      sourceConfig?.Invoke(source);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context">Cake Context</param>
    /// <param name="docfxFile">docfx.json file relative location</param>
    /// <param name="docConfig">Additional documentation config setup</param>
    /// <returns>DocFxConfiguration</returns>
    /// <example>
    /// <code>
    /// AddDotNetCoreDocConfiguration("./docs/docfx.json", config => 
    ///   {
    ///     config.Name = "DefaultDocs";
    ///   });
    /// </code>
    /// </example>
    [CakeMethodAlias]
    public static IDocFxConfiguration AddDotNetCoreDocConfiguration(
      this ICakeContext context,
      string docfxFile,
      Action<IDocFxConfiguration> docConfig = null)
    {
      if (context == null)
        throw new ArgumentNullException(nameof(context));

      var settings = context.DotNetCoreHelperSettings();
      var config = settings.GetDocFxConfiguration(docfxFile, docConfig);

      return config;
    }

    /// <summary>
    /// Add project for DotNetCore.
    /// </summary>
    /// <param name="context">Cake Context</param>
    /// <param name="slnFile">Relative Path to sln file</param>
    /// <param name="projConfig">Additional project configuration</param>
    /// <returns>Project Configuration</returns>
    /// <example>
    /// <code>
    /// AddDotNetCoreProject("./my.sln", config => 
    ///   {
    ///     config.Framework = "net452";
    ///   });
    /// </code>
    /// </example>
    [CakeMethodAlias]
    public static IProjectConfiguration AddDotNetCoreProject(this ICakeContext context, string slnFile, Action<IProjectConfiguration> projConfig = null)
    {
      if (context == null)
        throw new ArgumentNullException(nameof(context));

      var settings = context.DotNetCoreHelperSettings();
      var proj = settings.AddProject(slnFile, projConfig);

      return proj;
    }

    #endregion
  }
}