using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using Cake.Common.IO;
using Cake.Common.Solution;
using Cake.Core;
using Cake.Core.IO;
using Cake.Helpers.Settings;

namespace Cake.Helpers.DotNetCore
{
  /// <summary>
  ///   DotNetCore Project Configuration Contract
  /// </summary>
  public interface IProjectConfiguration : IHelperContext
  {
    /// <summary>
    ///   Project Name. This defaults to solution name with { " ", "." } trimmed
    /// </summary>
    string ProjectAlias { get; set; }

    /// <summary>
    ///   Solution path
    /// </summary>
    FilePath SlnFilePath { get; set; }

    /// <summary>
    ///   Configuration (Release, Debug, etc)
    /// </summary>
    string Configuration { get; set; }

    /// <summary>
    ///   Framework (net452, netstandard1.6)
    /// </summary>
    string Framework { get; set; }

    /// <summary>
    ///   Platform (x86, x64). Leave empty if AnyCpu
    /// </summary>
    string Platform { get; set; }

    /// <summary>
    ///   PostBuild temporary directory for artifacts from build
    /// </summary>
    DirectoryPath BuildTempDirectory { get; }

    /// <summary>
    ///   All .csproj defined in solution
    /// </summary>
    IEnumerable<SolutionProject> AllProjects { get; }

    /// <summary>
    ///   All .csproj defined in solution that not marked as test projects
    /// </summary>
    IEnumerable<SolutionProject> SrcProjects { get; }

    /// <summary>
    ///   All .csproj defined in solution that match test name filter DotNetCoreTestHelperSettings.TestProjectNameFilters
    ///   default is any project with ".Test." or ".Tests." in name
    /// </summary>
    IEnumerable<SolutionProject> TestProjects { get; }

    /// <summary>
    ///   Test configurations to run. This is what is set when you want to run different test categories
    /// </summary>
    IEnumerable<ITestConfiguration> TestConfigurations { get; }

    /// <summary>
    ///   Adds a test configuration to the project
    /// </summary>
    /// <param name="testConfig"></param>
    void AddTestConfig(ITestConfiguration testConfig);

    /// <summary>
    ///   Event to notify when test config is added
    /// </summary>
    event Action<IProjectConfiguration, ITestConfiguration> TestConfigAdded;
  }

  internal class ProjectConfiguration : IProjectConfiguration
  {
    #region Private Fields

    private List<SolutionProject> _allProjects;
    private readonly IHelperSettings _helperSettings = SingletonFactory.GetHelperSettings();
    private string _projectAlias = string.Empty;
    private FilePath _slnFilePath;
    private readonly List<ITestConfiguration> _testConfigurations = new List<ITestConfiguration>();

    #endregion

    #region Ctor

    [ExcludeFromCodeCoverage]
    internal ProjectConfiguration(ICakeContext context)
    {
      if (context == null)
        throw new ArgumentNullException(nameof(context));

      this.Context = context;
    }

    #endregion

    #region Private Methods

    private void ResetCache()
    {
      this._projectAlias = string.Empty;
      this._allProjects = null;
    }

    #endregion

    #region IHelperContext Members

    public ICakeContext Context { get; set; }

    #endregion

    #region IProjectConfiguration Members

    public IEnumerable<SolutionProject> AllProjects
    {
      get
      {
        if (this._allProjects == null || !this._allProjects.Any())
        {
          var slnParse = this.Context.ParseSolution(this.SlnFilePath);
          if (slnParse != null)
            this._allProjects = new List<SolutionProject>(slnParse.Projects);
        }

        return this._allProjects ?? Enumerable.Empty<SolutionProject>();
      }
    }

    public DirectoryPath BuildTempDirectory
    {
      get
      {
        var buildFolder = this._helperSettings.DotNetCoreSettings.BuildSettings.BuildTempFolder;

        if (string.IsNullOrWhiteSpace(buildFolder))
          return null;

        var path = this.Context.Directory(buildFolder).Path.Combine(this.ProjectAlias);

        if (!string.IsNullOrWhiteSpace(this.Configuration))
          path = path.Combine(this.Configuration);

        if (!string.IsNullOrWhiteSpace(this.Framework))
          path = path.Combine(this.Framework);

        if (!string.IsNullOrWhiteSpace(this.Platform))
          path = path.Combine(this.Platform);

        return path;
      }
    }

    public string Configuration { get; set; }
    public string Framework { get; set; }
    public string Platform { get; set; }

    public string ProjectAlias
    {
      get
      {
        if (string.IsNullOrWhiteSpace(this._projectAlias))
          this._projectAlias = this.SlnFilePath.GetFilenameWithoutExtension().FullPath
            .Replace(".", string.Empty)
            .Replace(" ", string.Empty);

        return this._projectAlias;
      }
      set { this._projectAlias = value; }
    }

    public FilePath SlnFilePath
    {
      get { return this._slnFilePath; }
      set
      {
        this._slnFilePath = value;
        this.ResetCache();
      }
    }

    public IEnumerable<SolutionProject> SrcProjects
    {
      get
      {
        return this.AllProjects
          .Where(t => this.TestProjects.All(x => x.Name != t.Name));
      }
    }

    public IEnumerable<ITestConfiguration> TestConfigurations
    {
      get { return this._testConfigurations; }
    }

    public IEnumerable<SolutionProject> TestProjects
    {
      get
      {
        var nameFilters = this._helperSettings.DotNetCoreSettings.TestSettings.TestProjectNameFilters;
        return this.AllProjects
          .Where(t =>
          {
            if (!nameFilters.Any())
              return false;

            return nameFilters.Any(x => Regex.IsMatch(t.Name, x));
          });
      }
    }

    public void AddTestConfig(ITestConfiguration testConfig)
    {
      if (testConfig == null)
        throw new ArgumentNullException(nameof(testConfig));

      this._testConfigurations.Add(testConfig);
      this.TestConfigAdded?.Invoke(this, testConfig);
    }

    public event Action<IProjectConfiguration, ITestConfiguration> TestConfigAdded;

    #endregion
  }

  /// <summary>
  ///   ProjectConfiguration Extensions
  /// </summary>
  public static class ProjectConfigurationExtensions
  {
    #region Static Members

    /// <summary>
    ///   Adds test config to project and returns that test config
    /// </summary>
    /// <param name="projConfig">ProjectConfiguration</param>
    /// <param name="testCategory">Test Category (Unit, System, etc)</param>
    /// <returns>TestConfiguration</returns>
    /// <example>
    ///   <code>
    /// // Creates task "Clean-Build-Sln"
    /// AddDotNetCoreProject("./my.sln", config => 
    ///   {
    ///     config.Framework = "net452";
    ///     config.AddTestConfig("Unit", testConfig => 
    ///     {
    ///       testConfig.Logger = "teamcity";
    ///     });
    ///   });
    /// </code>
    /// </example>
    public static ITestConfiguration AddTestConfig(this IProjectConfiguration projConfig, string testCategory)
    {
      if (projConfig == null)
        throw new ArgumentNullException(nameof(projConfig));

      var testConfig = new TestConfiguration(projConfig.Context, testCategory);

      projConfig.AddTestConfig(testConfig);

      return testConfig;
    }

    /// <summary>
    ///   Gets relative sln file path to current working directory
    /// </summary>
    /// <param name="config">ProjectConfiguration</param>
    /// <returns>Relative Sln FilePath</returns>
    public static FilePath GetRelativeSlnFilePath(this IProjectConfiguration config)
    {
      if (config == null)
        throw new ArgumentNullException(nameof(config));

      return config.Context.MakeRelative(config.SlnFilePath);
    }

    /// <summary>
    ///   Get FilePaths of all SourceProject artifacts. (Artifacts from bin directories)
    /// </summary>
    /// <param name="config">ProjectConfiguration</param>
    /// <returns>Artifact FilePaths</returns>
    [ExcludeFromCodeCoverage]
    public static IEnumerable<FilePath> GetSrcProjectArtifacts(
      this IProjectConfiguration config)
    {
      if (config == null)
        throw new ArgumentNullException(nameof(config));

      return config.SrcProjects
        .SelectMany(t =>
        {
          var outputDirString = t.GetProjectBinOutputDirectoryPath(config);
          var filePaths = t.GetProjectOutputFileNamePatterns()
            .Select(x => $"{outputDirString}/{x}")
            .SelectMany(x => config.Context.GetFiles(x));

          return filePaths;
        })
        .Where(t => config.Context.FileExists(t));
    }

    internal static IEnumerable<string> GetAllProjectOutputDirectoryPaths(
      this IProjectConfiguration config)
    {
      if (config == null)
        throw new ArgumentNullException(nameof(config));

      var paths = config.AllProjects
        .SelectMany(t => t.GetProjectOutputDirectoryPaths(config))
        .Distinct();

      return paths;
    }

    [ExcludeFromCodeCoverage]
    internal static IEnumerable<string> GetProjectOutputDirectoryPaths(
      this SolutionProject project,
      IProjectConfiguration config)
    {
      if (project == null)
        throw new ArgumentNullException(nameof(project));

      if (config == null)
        throw new ArgumentNullException(nameof(config));

      var relativeBase = config.GetRelativeSlnFilePath().GetDirectory();
      var baseOutputPath = $"{relativeBase.FullPath}/**/{project.Name}";
      var baseBinOutputPath = $"{baseOutputPath}/bin";
      var baseObjOutputPath = $"{baseOutputPath}/bin";

      var configPath = string.IsNullOrWhiteSpace(config.Configuration) ? "/**" : $"/{config.Configuration}";
      var frameworkPath = string.IsNullOrWhiteSpace(config.Framework) ? string.Empty : $"/{config.Framework}";

      var binPath = $"{baseBinOutputPath}{configPath}{frameworkPath}";
      var objPath = $"{baseObjOutputPath}{configPath}{frameworkPath}";

      return new[] {project.GetProjectBinOutputDirectoryPath(config), project.GetProjectObjOutputDirectoryPath(config)};
    }

    internal static IEnumerable<string> GetProjectOutputFileNamePatterns(
      this SolutionProject project)
    {
      if (project == null)
        throw new ArgumentNullException(nameof(project));

      yield return $"{project.Name}.dll";
      yield return $"{project.Name}.xml";
      yield return $"{project.Name}.pdb";
    }

    [ExcludeFromCodeCoverage]
    private static string GetProjectBinOutputDirectoryPath(
      this SolutionProject project,
      IProjectConfiguration config)
    {
      if (project == null)
        throw new ArgumentNullException(nameof(project));

      if (config == null)
        throw new ArgumentNullException(nameof(config));

      var relativeBase = config.GetRelativeSlnFilePath().GetDirectory();
      var baseOutputPath = $"{relativeBase.FullPath}/**/{project.Name}";
      var baseBinOutputPath = $"{baseOutputPath}/bin";

      var configPath = string.IsNullOrWhiteSpace(config.Configuration) ? "/**" : $"/{config.Configuration}";
      var frameworkPath = string.IsNullOrWhiteSpace(config.Framework) ? string.Empty : $"/{config.Framework}";

      var binPath = $"{baseBinOutputPath}{configPath}{frameworkPath}";

      return binPath;
    }

    [ExcludeFromCodeCoverage]
    private static string GetProjectObjOutputDirectoryPath(
      this SolutionProject project,
      IProjectConfiguration config)
    {
      if (project == null)
        throw new ArgumentNullException(nameof(project));

      if (config == null)
        throw new ArgumentNullException(nameof(config));

      var relativeBase = config.GetRelativeSlnFilePath().GetDirectory();
      var baseOutputPath = $"{relativeBase.FullPath}/**/{project.Name}";
      var baseBinOutputPath = $"{baseOutputPath}/obj";

      var configPath = string.IsNullOrWhiteSpace(config.Configuration) ? "/**" : $"/{config.Configuration}";
      var frameworkPath = string.IsNullOrWhiteSpace(config.Framework) ? string.Empty : $"/{config.Framework}";

      var binPath = $"{baseBinOutputPath}{configPath}{frameworkPath}";

      return binPath;
    }

    #endregion
  }
}