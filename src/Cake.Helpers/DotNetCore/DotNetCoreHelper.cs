using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using Cake.Common.Diagnostics;
using Cake.Common.IO;
using Cake.Common.Tools.DotNetCore;
using Cake.Common.Tools.DotNetCore.Build;
using Cake.Common.Tools.DotNetCore.Clean;
using Cake.Common.Tools.DotNetCore.Restore;
using Cake.Common.Tools.DotNetCore.Test;
using Cake.Core;
using Cake.Core.IO;
using Cake.DocFx;
using Cake.DocFx.Build;
using Cake.DocFx.Metadata;
using Cake.Helpers.Build;
using Cake.Helpers.Documentation;
using Cake.Helpers.Settings;
using Cake.Helpers.Test;

namespace Cake.Helpers.DotNetCore
{
  /// <inheritdoc />
  /// <summary>
  /// DotNetCore Helper Contract
  /// </summary>
  public interface IDotNetCoreHelper : IHelperContext
  {
    /// <summary>
    /// Location for PostBuild artifacts
    /// </summary>
    DirectoryPath BuildTempFolder { get; }
    /// <summary>
    /// Location for test results
    /// </summary>
    DirectoryPath TestTempFolder { get; }
    /// <summary>
    /// Projects Defined
    /// </summary>
    IEnumerable<IProjectConfiguration> Projects { get; }
    /// <summary>
    /// Test configurations defined for all projects
    /// </summary>
    IEnumerable<ITestConfiguration> Tests { get; }
    /// <summary>
    /// Gets project config for sln path
    /// </summary>
    /// <param name="slnFilePath">relative path to sln file</param>
    /// <returns>ProjectConfiguration</returns>
    IProjectConfiguration GetProjectConfiguration(FilePath slnFilePath);
    /// <summary>
    /// Adds project configuration to DotNetCore Helper
    /// </summary>
    /// <param name="projConfig">Project Configuration</param>
    /// <returns>Added Project Configuration</returns>
    IProjectConfiguration AddProjectConfiguration(IProjectConfiguration projConfig);
  }

  internal class DotNetCoreHelper : IDotNetCoreHelper
  {
    #region Private Fields

    private readonly IHelperSettings _helperSettings = SingletonFactory.GetHelperSettings();

    private readonly List<IProjectConfiguration> _projects = new List<IProjectConfiguration>();

    #endregion

    #region Ctor

    internal DotNetCoreHelper()
    { }

    #endregion

    #region Private Methods

    [ExcludeFromCodeCoverage]
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

    [ExcludeFromCodeCoverage]
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

    [ExcludeFromCodeCoverage]
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

    [ExcludeFromCodeCoverage]
    private void RegisterPreBuildTasks(IProjectConfiguration projConfig)
    {
      this.Context.PreBuildTask(projConfig.ProjectAlias);

      this.Context.PreBuildTask("DotNetCoreRestore", false, projConfig.ProjectAlias)
        .Does(() =>
        {
          var restoreSettings = new DotNetCoreRestoreSettings
          {
            Sources = this._helperSettings.DotNetCoreSettings.NugetSettings.GetFeedUrls().ToList()
          };

          this.Context.DotNetCoreRestore(projConfig.GetRelativeSlnFilePath().FullPath, restoreSettings);
        });
    }

    [ExcludeFromCodeCoverage]
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
            NoBuild = !this._helperSettings.RunAllDependencies
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

    [ExcludeFromCodeCoverage]
    private void RegisterDocTasks(IProjectConfiguration projConfig)
    {
      if(this._helperSettings.DotNetCoreSettings.DocConfigs.Any())
        this.Context.DocBuildTask(projConfig.ProjectAlias);

      foreach (var docConfig in this._helperSettings.DotNetCoreSettings.DocFxConfigurations())
      {
        var docfxDirectory = docConfig.GetDocFxDirectoryPath();

        this.Context.DocExtractTask("BuildDocs", false, projConfig.ProjectAlias)
          .Does(() =>
          {
            this.Context.DocFxBuild(new DocFxBuildSettings()
            {
              WorkingDirectory = docfxDirectory
            });
          });
        this.Context.DocBuildTask("BuildDocs", false, projConfig.ProjectAlias)
          .Does(() =>
          {
            this.Context.DocFxMetadata(new DocFxMetadataSettings()
            {
              WorkingDirectory = docfxDirectory
            });
          });
      }
    }

    #endregion

    #region IDotNetCoreHelper Members

    public DirectoryPath BuildTempFolder
    {
      get { return this.Context.Directory(this._helperSettings.DotNetCoreSettings.BuildSettings.BuildTempFolder); }
    }

    public IEnumerable<IProjectConfiguration> Projects
    {
      get { return this._projects; }
    }

    public IEnumerable<ITestConfiguration> Tests
    {
      get { return this.Projects.SelectMany(t => t.TestConfigurations).Distinct(); }
    }

    public DirectoryPath TestTempFolder
    {
      get { return this.Context.Directory(this._helperSettings.DotNetCoreSettings.TestSettings.TestTempFolder); }
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
      this.RegisterDocTasks(projConfig);

      foreach (var testConfig in projConfig.TestConfigurations)
      {
        this.RegisterTestTasks(projConfig, testConfig);
      }

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

  /// <summary>
  /// DotNetCoreHelper Extensions
  /// </summary>
  public static class DotNetCoreHelperExtensions
  {
    #region Static Members

    /// <summary>
    /// Adds project configuration for sln file and allows config setup
    /// </summary>
    /// <param name="helper">DotNetCoreHelper</param>
    /// <param name="filePathStr">Relative Path to sln file</param>
    /// <param name="setupConfig">Project Configuration Setup</param>
    /// <returns>ProjectConfiguration</returns>
    /// <example>
    /// <code>
    /// var helper = SingletonFactory.GetDotNetCoreHelper();
    /// helper.AddProjectConfiguration("./my.sln", config =>
    ///   {
    ///     config.Framework = "net452";
    ///   });
    /// </code>
    /// </example>
    public static IProjectConfiguration AddProjectConfiguration(
      this IDotNetCoreHelper helper,
      string filePathStr,
      Action<IProjectConfiguration> setupConfig = null)
    {
      if (helper == null)
        throw new ArgumentNullException(nameof(helper));

      if (string.IsNullOrWhiteSpace(filePathStr))
        throw new ArgumentNullException(filePathStr);

      var filePath = helper.Context.File(filePathStr).Path;
      filePath = helper.Context.MakeRelative(filePath);

      if (!helper.Context.FileExists(filePath))
        throw new FileNotFoundException("Failed to find project file", filePath.FullPath);

      var projConfig = helper.GetProjectConfiguration(filePath);
      if (setupConfig != null)
        setupConfig(projConfig);

      return helper.AddProjectConfiguration(projConfig);
    }

    #endregion
  }
}