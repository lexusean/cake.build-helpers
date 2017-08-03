using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cake.Core;
using Cake.Helpers.DotNetCore;

namespace Cake.Helpers.Settings
{
  public interface ISetting
  {
    void SetupSetting();
  }

  public interface ISubSetting : ISetting
  {
    bool IsActive { get; }
  }

  public interface IHelperSettings : ISetting, IHelperContext
  {
    Func<string, CakeReport> RunTargetFunc { get; set; }
    Func<string, CakeTaskBuilder<ActionTask>> TaskTargetFunc { get; set; }

    IEnumerable<ISubSetting> SubSettings { get; }

    bool RunAllDependencies { get; set; }
    IDotNetCoreHelperSettings DotNetCoreSettings { get; }
  }
}
