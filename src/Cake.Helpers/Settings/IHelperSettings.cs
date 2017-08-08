using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cake.Core;
using Cake.Helpers.DotNetCore;

namespace Cake.Helpers.Settings
{
  /// <summary>
  /// Contract for all Helper Settings
  /// </summary>
  public interface ISetting
  {
    /// <summary>
    /// Sets up setting when called
    /// </summary>
    void SetupSetting();
  }

  /// <summary>
  /// Nested Settings Contract
  /// </summary>
  /// <inheritdoc />
  public interface ISubSetting : ISetting
  {
    /// <summary>
    /// Active flag
    /// </summary>
    bool IsActive { get; }
  }

  /// <summary>
  /// Top level helper settings contract
  /// </summary>
  /// <inheritdoc />
  public interface IHelperSettings : ISetting, IHelperContext
  {
    /// <summary>
    /// Delegate to set to Cake RunTarget()
    /// </summary>
    /// <example>
    /// <code>
    /// HelperSettings.RunTargetFunc = RunTarget;
    /// </code>
    /// </example>
    Func<string, CakeReport> RunTargetFunc { get; set; }
    /// <summary>
    /// Delegate to set to Cake ScriptHost Task()
    /// </summary>
    /// <example>
    /// <code>
    /// HelperSettings.TaskTargetFunc = Task;
    /// </code>
    /// </example>
    Func<string, CakeTaskBuilder<ActionTask>> TaskTargetFunc { get; set; }

    /// <summary>
    /// All Sub settings
    /// </summary>
    IEnumerable<ISubSetting> SubSettings { get; }

    /// <summary>
    /// Flag to indicate whether to build all dependency trees. Default is False
    /// </summary>
    bool RunAllDependencies { get; set; }
    /// <summary>
    /// DotNetCore Settings
    /// </summary>
    IDotNetCoreHelperSettings DotNetCoreSettings { get; }
  }
}
