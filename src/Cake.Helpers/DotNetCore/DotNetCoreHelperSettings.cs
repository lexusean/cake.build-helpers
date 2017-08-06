using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Cake.Common.IO;
using Cake.Core;
using Cake.Helpers.Nuget;
using Cake.Helpers.Settings;

namespace Cake.Helpers.DotNetCore
{
  internal class DotNetCoreBuildHelperSettings : IDotNetCoreBuildHelperSettings, IHelperContext
  {
    #region IDotNetCoreBuildHelperSettings Members

    public string BuildTempFolder { get; set; } = "./BuildTemp";

    #endregion

    #region IHelperContext Members

    public ICakeContext Context { [ExcludeFromCodeCoverage]get; set; }

    #endregion

    #region ISetting Members

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public void SetupSetting()
    { }

    #endregion

    #region ISubSetting Members

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public bool IsActive { get; internal set; }

    #endregion
  }

  internal class DotNetCoreTestHelperSettings : IDotNetCoreTestHelperSettings, IHelperContext
  {
    #region IDotNetCoreTestHelperSettings Members

    public List<string> TestProjectNameFilters { get; } = new List<string>(new[]
    {
      ".+\\.Test\\..*",
      ".+\\.Tests\\..*"
    });

    public string TestTempFolder { get; set; } = "./TestTemp";

    #endregion

    #region IHelperContext Members

    public ICakeContext Context { [ExcludeFromCodeCoverage]get; set; }

    #endregion

    #region ISetting Members

    [ExcludeFromCodeCoverage]
    public void SetupSetting()
    { }

    #endregion

    #region ISubSetting Members

    [ExcludeFromCodeCoverage]
    public bool IsActive { get; internal set; }

    #endregion
  }

  internal class DotNetCoreHelperSettings : IDotNetCoreHelperSettings, IHelperContext
  {
    #region Private Fields

    private ICakeContext _Context;

    private bool _IsActive;

    private readonly List<IProjectConfiguration> _Projects = new List<IProjectConfiguration>();

    #endregion

    #region IDotNetCoreHelperSettings Members

    public IDotNetCoreBuildHelperSettings BuildSettings { get; } = new DotNetCoreBuildHelperSettings();
    public INugetHelperSettings NugetSettings { get; } = new NugetHelperSettings();

    public IEnumerable<IProjectConfiguration> Projects
    {
      get { return this._Projects; }
    }

    public IDotNetCoreTestHelperSettings TestSettings { get; } = new DotNetCoreTestHelperSettings();

    public IProjectConfiguration AddProject(string slnFile, Action<IProjectConfiguration> projectConfig = null)
    {
      if (string.IsNullOrWhiteSpace(slnFile))
        throw new ArgumentNullException(nameof(slnFile));

      var filePath = this.Context.File(slnFile).Path;
      filePath = this.Context.MakeRelative(filePath);

      if (!this.Context.FileExists(filePath))
        throw new FileNotFoundException("Failed to find Sln file", filePath.FullPath);

      var existingProjectConfig = this._Projects.FirstOrDefault(t => t.SlnFilePath.FullPath == filePath.FullPath);
      if (existingProjectConfig == null)
      {
        existingProjectConfig = new ProjectConfiguration(this.Context)
        {
          SlnFilePath = filePath
        };

        this._Projects.Add(existingProjectConfig);
        this.IsActive = true;
      }

      projectConfig?.Invoke(existingProjectConfig);

      return existingProjectConfig;
    }

    #endregion

    #region IHelperContext Members

    public ICakeContext Context
    {
      get { return this._Context; }
      set
      {
        this._Context = value;
        ((NugetHelperSettings) this.NugetSettings).Context = this._Context;
        ((DotNetCoreBuildHelperSettings) this.BuildSettings).Context = this._Context;
        ((DotNetCoreTestHelperSettings) this.TestSettings).Context = this._Context;
      }
    }

    #endregion

    #region ISetting Members

    public void SetupSetting()
    {
      var coreHelper = SingletonFactory.GetDotNetCoreHelper();
      foreach (var projectConfig in this.Projects)
      {
        coreHelper.AddProjectConfiguration(projectConfig);
      }
    }

    #endregion

    #region ISubSetting Members

    public bool IsActive
    {
      get { return this._IsActive; }
      set
      {
        this._IsActive = value;
        ((NugetHelperSettings) this.NugetSettings).IsActive = value;
        ((DotNetCoreBuildHelperSettings) this.BuildSettings).IsActive = value;
        ((DotNetCoreTestHelperSettings) this.TestSettings).IsActive = value;
      }
    }

    #endregion
  }
}