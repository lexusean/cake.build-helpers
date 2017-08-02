using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Cake.Common.IO;
using Cake.Common.Solution;
using Cake.Core;
using Cake.Core.IO;

namespace Cake.Helpers.DotNetCore
{
  public interface IProjectConfiguration : IHelperContext
  {
    string ProjectAlias { get; set; }
    FilePath SlnFilePath { get; set; }
    string Configuration { get; set; }
    string Framework { get; set; }
    string Platform { get; set; }

    DirectoryPath BuildTempDirectory { get; }

    IEnumerable<SolutionProject> AllProjects { get; }
    IEnumerable<SolutionProject> SrcProjects { get; }
    IEnumerable<SolutionProject> TestProjects { get; }

    IEnumerable<ITestConfiguration> TestConfigurations { get; }
    void AddTestConfig(ITestConfiguration testConfig);

    event Action<IProjectConfiguration, ITestConfiguration> TestConfigAdded;
  }

  public class ProjectConfiguration : IProjectConfiguration
  {
    #region Private Fields

    private List<SolutionProject> _allProjects;

    private string _projectAlias = string.Empty;

    private FilePath _slnFilePath;

    #endregion

    #region Ctor

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
        if (string.IsNullOrWhiteSpace(DotNetCoreSettingsCache.BuildTempFolder))
          return null;

        var path = this.Context.Directory(DotNetCoreSettingsCache.BuildTempFolder).Path.Combine(this.ProjectAlias);

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

    public IEnumerable<SolutionProject> TestProjects
    {
      get
      {
        var nameFilters = DotNetCoreSettingsCache.TestProjectNameFilters;
        return this.AllProjects
          .Where(t =>
          {
            if (!nameFilters.Any())
              return false;

            return nameFilters.Any(x => Regex.IsMatch(t.Name, x));
          });
      }
    }

    private List<ITestConfiguration> _testConfigurations = new List<ITestConfiguration>();
    public IEnumerable<ITestConfiguration> TestConfigurations
    {
      get { return this._testConfigurations; }
    }

    public void AddTestConfig(ITestConfiguration testConfig)
    {
      if(testConfig == null)
        throw new ArgumentNullException(nameof(testConfig));

      this._testConfigurations.Add(testConfig);
      if (this.TestConfigAdded != null)
        this.TestConfigAdded(this, testConfig);
    }

    public event Action<IProjectConfiguration, ITestConfiguration> TestConfigAdded;

    #endregion
  }

  public static class ProjectConfigurationExtensions
  {
    #region Static Members

    public static IEnumerable<string> GetAllProjectOutputDirectoryPaths(
      this IProjectConfiguration config)
    {
      if (config == null)
        throw new ArgumentNullException(nameof(config));

      var paths = config.AllProjects
        .SelectMany(t => t.GetProjectOutputDirectoryPaths(config))
        .Distinct();

      return paths;
    }

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

    public static IEnumerable<string> GetProjectOutputDirectoryPaths(
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

      return new[] { project.GetProjectBinOutputDirectoryPath(config), project.GetProjectObjOutputDirectoryPath(config) };
    }

    public static IEnumerable<string> GetProjectOutputFileNamePatterns(
      this SolutionProject project)
    {
      if (project == null)
        throw new ArgumentNullException(nameof(project));

      yield return $"{project.Name}.dll";
      yield return $"{project.Name}.xml";
      yield return $"{project.Name}.pdb";
    }

    public static IEnumerable<FilePath> GetSrcProjectArtifacts(
      this IProjectConfiguration config)
    {
      if(config == null)
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

    public static FilePath GetRelativeSlnFilePath(this IProjectConfiguration config)
    {
      if (config == null)
        throw new ArgumentNullException(nameof(config));

      return config.Context.MakeRelative(config.SlnFilePath);
    }

    public static ITestConfiguration AddTestConfig(this ProjectConfiguration projConfig, string testCategory)
    {
      if(projConfig == null)
        throw new ArgumentNullException(nameof(projConfig));

      var testConfig = new TestConfiguration(projConfig.Context, testCategory);

      projConfig.AddTestConfig(testConfig);

      return testConfig;
    }

    #endregion
  }
}