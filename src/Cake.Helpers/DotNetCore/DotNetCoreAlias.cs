using System;
using System.Collections.Generic;
using Cake.Common.IO;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.IO;

namespace Cake.Helpers.DotNetCore
{
  [CakeAliasCategory("Helper")]
  [CakeAliasCategory("DotNetCore")]
  public static class DotNetCoreAlias
  {
    #region Static Members

    [CakeMethodAlias]
    public static void AddDotNetCoreHelperNugetSource(this ICakeContext context, string src)
    {
      if (context == null)
        throw new ArgumentNullException(nameof(context));

      if (string.IsNullOrWhiteSpace(src))
        return;

      DotNetCoreSettingsCache.NugetSources.Add(src);
    }

    [CakeMethodAlias]
    public static IProjectConfiguration AddDotNetCoreHelperProject(
      this ICakeContext context,
      string slnFilePath,
      Action<IProjectConfiguration> setupProjConfig = null)
    {
      if (context == null)
        throw new ArgumentNullException(nameof(context));

      var helper = SingletonFactory.GetDotNetCoreHelper();
      return helper.AddProjectConfiguration(slnFilePath, setupProjConfig);
    }

    [CakeMethodAlias]
    public static void AddDotNetCoreHelperTestProjectFilters(this ICakeContext context, string filter)
    {
      if (context == null)
        throw new ArgumentNullException(nameof(context));

      if (string.IsNullOrWhiteSpace(filter))
        return;

      DotNetCoreSettingsCache.TestProjectNameFilters.Add(filter);
    }

    [CakePropertyAlias]
    public static DotNetCoreHelper DotNetCoreHelper(this ICakeContext context)
    {
      if (context == null)
        throw new ArgumentNullException(nameof(context));

      SingletonFactory.Context = context;
      return SingletonFactory.GetDotNetCoreHelper();
    }

    [CakePropertyAlias]
    public static DirectoryPath DotNetCoreHelperBuildOutputFolder(this ICakeContext context)
    {
      if (context == null)
        throw new ArgumentNullException(nameof(context));

      return context.Directory(DotNetCoreSettingsCache.BuildTempFolder);
    }

    [CakePropertyAlias]
    public static List<string> DotNetCoreHelperNugetSources(this ICakeContext context)
    {
      if (context == null)
        throw new ArgumentNullException(nameof(context));

      return DotNetCoreSettingsCache.NugetSources;
    }

    [CakePropertyAlias]
    public static DirectoryPath DotNetCoreHelperTestOutputFolder(this ICakeContext context)
    {
      if (context == null)
        throw new ArgumentNullException(nameof(context));

      return context.Directory(DotNetCoreSettingsCache.TestTempFolder);
    }

    [CakePropertyAlias]
    public static List<string> DotNetCoreHelperTestProjectFilters(this ICakeContext context)
    {
      if (context == null)
        throw new ArgumentNullException(nameof(context));

      return DotNetCoreSettingsCache.TestProjectNameFilters;
    }

    [CakeMethodAlias]
    public static void SetDotNetCoreHelperBuildOutputFolder(this ICakeContext context, string outputFolder)
    {
      if (context == null)
        throw new ArgumentNullException(nameof(context));

      DotNetCoreSettingsCache.BuildTempFolder = outputFolder;
    }

    [CakeMethodAlias]
    public static void SetDotNetCoreHelperTestOutputFolder(this ICakeContext context, string outputFolder)
    {
      if (context == null)
        throw new ArgumentNullException(nameof(context));

      DotNetCoreSettingsCache.TestTempFolder = outputFolder;
    }

    #endregion
  }
}