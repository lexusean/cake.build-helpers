using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cake.Common.Diagnostics;
using Cake.Common.IO;
using Cake.Common.Tools.DotNetCore;
using Cake.Common.Tools.DotNetCore.Build;
using Cake.Common.Tools.DotNetCore.Clean;
using Cake.Common.Tools.DotNetCore.Restore;
using Cake.Common.Tools.DotNetCore.Test;
using Cake.Core;
using Cake.Core.IO;
using Cake.Helpers.Build;
using Cake.Helpers.Test;

namespace Cake.Helpers.DotNetCore
{
  public interface IDotNetCoreHelper : IHelperContext
  {
    DirectoryPath BuildTempFolder { get; }
    DirectoryPath TestTempFolder { get; }
    IEnumerable<IProjectConfiguration> Projects { get; }
    IEnumerable<ITestConfiguration> Tests { get; }
    IProjectConfiguration GetProjectConfiguration(FilePath slnFilePath);
    IProjectConfiguration AddProjectConfiguration(IProjectConfiguration projConfig);
  }

  public class DotNetCoreHelper : IDotNetCoreHelper
  {
    #region Private Fields

    private readonly List<IProjectConfiguration> _projects = new List<IProjectConfiguration>();
    private readonly List<ITestConfiguration> _Tests = new List<ITestConfiguration>();

    #endregion

    #region Ctor

    internal DotNetCoreHelper()
    {
    }

    #endregion

    #region Private Methods

    private void RegisterBuildTasks(IProjectConfiguration projConfig)
    {
      this.Context.BuildTask(projConfig.ProjectAlias);

      this.Context.BuildTask("DotNetCoreBuild", false, projConfig.ProjectAlias)
        .Does(() =>
        {
          var buildSettings = new DotNetCoreBuildSettings
          {
            Configuration = projConfig.Configuration,
            Framework = projConfig.Framework
          };

          this.Context.Debug($"Building {projConfig.GetRelativeSlnFilePath().FullPath}...");
          this.Context.DotNetCoreBuild(projConfig.GetRelativeSlnFilePath().FullPath, buildSettings);
        });
    }

    private void RegisterCleanTasks(IProjectConfiguration projConfig)
    {
      this.Context.BuildCleanTask(projConfig.ProjectAlias);

      this.Context.BuildCleanTask("DotNetCoreClean", false, projConfig.ProjectAlias)
        .Does(() =>
        {
          var clnSettings = new DotNetCoreCleanSettings
          {
            Configuration = projConfig.Configuration,
            Framework = projConfig.Framework
          };

          this.Context.DotNetCoreClean(projConfig.GetRelativeSlnFilePath().FullPath, clnSettings);
        });

      this.Context.BuildCleanTask("BinAndObj", false, projConfig.ProjectAlias)
        .Does(() =>
        {
          var paths = projConfig.GetAllProjectOutputDirectoryPaths();
          foreach (var path in paths)
          {
            this.Context.Debug($"Cleaning Directory: {path}");
            this.Context.CleanDirectories(path);
          }
        });

      this.Context.BuildCleanTask("BuildTemp", false, projConfig.ProjectAlias)
        .Does(() =>
        {
          var buildTempDir = projConfig.BuildTempDirectory;
          if (buildTempDir != null && this.Context.DirectoryExists(buildTempDir))
          {
            this.Context.Debug($"Deleting BuildTemp directory: {buildTempDir.FullPath}");
            this.Context.DeleteDirectory(buildTempDir, true);
          }
        });
    }

    private void RegisterPostBuildTasks(IProjectConfiguration projConfig)
    {
      this.Context.PostBuildTask(projConfig.ProjectAlias);

      this.Context.PostBuildTask("MoveToBuildTemp", false, projConfig.ProjectAlias)
        .Does(() =>
        {
          if (projConfig.BuildTempDirectory == null)
            throw new ArgumentNullException(nameof(projConfig.BuildTempDirectory),
              "BuildTempDirectory was not set on ProjectConfig");

          var artifacts = projConfig.GetSrcProjectArtifacts();
          var filePaths = artifacts as FilePath[] ?? artifacts.ToArray();
          if (!filePaths.Any())
            throw new Exception($"No Build Artifacts found. Run \"build.(ps1|.sh) -r=Build-{projConfig.ProjectAlias}");

          this.Context.EnsureDirectoryExists(projConfig.BuildTempDirectory);

          foreach (var artifact in filePaths)
          {
            this.Context.CopyFileToDirectory(artifact, projConfig.BuildTempDirectory);
          }
        });
    }

    private void RegisterPreBuildTasks(IProjectConfiguration projConfig)
    {
      this.Context.PreBuildTask(projConfig.ProjectAlias);

      this.Context.PreBuildTask("DotNetCoreRestore", false, projConfig.ProjectAlias)
        .Does(() =>
        {
          var restoreSettings = new DotNetCoreRestoreSettings
          {
            Sources = DotNetCoreSettingsCache.NugetSources
          };

          this.Context.DotNetCoreRestore(projConfig.GetRelativeSlnFilePath().FullPath, restoreSettings);
        });
    }

    private void RegisterTestTasks(IProjectConfiguration projConfig, ITestConfiguration testConfig)
    {
      this.Context.TestCleanTask(projConfig.ProjectAlias, testConfig.TestCategory)
        .Does(() =>
        {
          var targetDir = this.Context.MakeAbsolute(this.TestTempFolder);
          targetDir = targetDir.Combine(projConfig.ProjectAlias);
          targetDir = targetDir.Combine(testConfig.TestCategory);

          if (this.Context.DirectoryExists(targetDir))
            this.Context.DeleteDirectory(targetDir, true);
        });

      this.Context.TestTask(projConfig.ProjectAlias, testConfig.TestCategory)
        .Does(() =>
        {
          var targetDir = this.Context.MakeAbsolute(this.TestTempFolder);
          targetDir = targetDir.Combine(projConfig.ProjectAlias);
          targetDir = targetDir.Combine(testConfig.TestCategory);

          var settings = new DotNetCoreTestSettings
          {
            Filter = testConfig.GetDotNetCoreCategoryString(),
            Configuration = projConfig.Configuration,
            Framework = projConfig.Framework,
            NoBuild = !HelperSettingsCache.BuildAllDependencies
          };

          foreach (var testProject in projConfig.TestProjects)
          {
            var logger = testConfig.Logger;

            if (string.IsNullOrWhiteSpace(testConfig.Logger))
            {
              var testResultsFile = $"{testProject.Name}.{testConfig.TestCategory}.trx";
              var testResultsTarget = targetDir.CombineWithFilePath(testResultsFile);

              logger = $"trx;LogFileName={testResultsTarget.FullPath}";
            }

            settings.Logger = logger;
            this.Context.DotNetCoreTest(this.Context.MakeAbsolute(testProject.Path).FullPath, settings);
          }
        });
    }

    #endregion

    #region IDotNetCoreHelper Members

    public DirectoryPath BuildTempFolder
    {
      get { return this.Context.Directory(DotNetCoreSettingsCache.BuildTempFolder); }
    }

    public IEnumerable<IProjectConfiguration> Projects
    {
      get { return this._projects; }
    }

    public IEnumerable<ITestConfiguration> Tests
    {
      get { return this._Tests; }
    }

    public DirectoryPath TestTempFolder
    {
      get { return this.Context.Directory(DotNetCoreSettingsCache.TestTempFolder); }
    }

    public IProjectConfiguration AddProjectConfiguration(IProjectConfiguration projConfig)
    {
      if (this._projects.Any(t => t.ProjectAlias == projConfig.ProjectAlias))
        return projConfig;

      this._projects.Add(projConfig);

      this.RegisterCleanTasks(projConfig);
      this.RegisterPreBuildTasks(projConfig);
      this.RegisterBuildTasks(projConfig);
      this.RegisterPostBuildTasks(projConfig);

      projConfig.TestConfigAdded += this.RegisterTestTasks;

      return projConfig;
    }

    public IProjectConfiguration GetProjectConfiguration(FilePath slnFilePath)
    {
      return new ProjectConfiguration(this.Context)
      {
        SlnFilePath = slnFilePath
      };
    }

    #endregion

    #region IHelperContext Members

    public ICakeContext Context { get; set; }

    #endregion
  }

  public static class DotNetCoreHelperExtensions
  {
    public static IProjectConfiguration AddProjectConfiguration(
      this IDotNetCoreHelper helper, 
      string filePathStr,
      Action<IProjectConfiguration> setupConfig = null)
    {
      if(helper == null)
        throw new ArgumentNullException(nameof(helper));

      if(string.IsNullOrWhiteSpace(filePathStr))
        throw new ArgumentNullException(filePathStr);

      var filePath = helper.Context.File(filePathStr).Path;
      filePath = helper.Context.MakeRelative(filePath);

      if(!helper.Context.FileExists(filePath))
        throw new FileNotFoundException("Failed to find project file", filePath.FullPath);

      var projConfig = helper.GetProjectConfiguration(filePath);
      if (setupConfig != null)
        setupConfig(projConfig);

      return helper.AddProjectConfiguration(projConfig);
    }
  }
}