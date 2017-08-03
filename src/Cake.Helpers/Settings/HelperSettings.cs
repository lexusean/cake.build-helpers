using System;
using System.Collections.Generic;
using Cake.Core;
using Cake.Core.Tooling;
using Cake.Helpers.DotNetCore;

namespace Cake.Helpers.Settings
{
  internal class HelperSettings : ToolSettings, IHelperSettings
  {
    #region Private Fields

    private ICakeContext _Context;

    #endregion

    #region IHelperContext Members

    public ICakeContext Context
    {
      get { return this._Context; }
      set
      {
        this._Context = value;
        ((DotNetCoreHelperSettings) this.DotNetCoreSettings).Context = this._Context;
      }
    }

    #endregion

    #region IHelperSettings Members

    public IDotNetCoreHelperSettings DotNetCoreSettings { get; } = new DotNetCoreHelperSettings();

    public IEnumerable<ISubSetting> SubSettings
    {
      get
      {
        yield return this.DotNetCoreSettings;
      }
    }

    public bool RunAllDependencies { get; set; }
    public Func<string, CakeReport> RunTargetFunc { get; set; }
    public Func<string, CakeTaskBuilder<ActionTask>> TaskTargetFunc { get; set; }

    #endregion

    public void SetupSetting()
    {
      foreach (var sub in this.SubSettings)
      {
        if(!sub.IsActive)
          continue;

        sub.SetupSetting();
      }
    }
  }
}