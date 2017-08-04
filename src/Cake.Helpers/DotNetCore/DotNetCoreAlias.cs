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
  [CakeAliasCategory("Helper")]
  [CakeAliasCategory("DotNetCore")]
  public static class DotNetCoreAlias
  {
    #region Static Members

    [CakePropertyAlias]
    public static IDotNetCoreHelperSettings DotNetCoreHelperSettings(this ICakeContext context)
    {
      if (context == null)
        throw new ArgumentNullException(nameof(context));

      SingletonFactory.Context = context;
      return SingletonFactory.GetHelperSettings().DotNetCoreSettings;
    }

    [CakePropertyAlias]
    public static IDotNetCoreBuildHelperSettings DotNetCoreBuildHelperSettings(this ICakeContext context)
    {
      if (context == null)
        throw new ArgumentNullException(nameof(context));

      SingletonFactory.Context = context;
      return SingletonFactory.GetHelperSettings().DotNetCoreSettings.BuildSettings;
    }

    [CakePropertyAlias]
    public static IDotNetCoreTestHelperSettings DotNetCoreTestHelperSettings(this ICakeContext context)
    {
      if (context == null)
        throw new ArgumentNullException(nameof(context));

      SingletonFactory.Context = context;
      return SingletonFactory.GetHelperSettings().DotNetCoreSettings.TestSettings;
    }

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