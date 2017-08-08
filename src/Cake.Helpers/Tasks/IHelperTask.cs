using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cake.Core;

namespace Cake.Helpers.Tasks
{
  /// <summary>
  /// Helper Task Contract
  /// </summary>
  public interface IHelperTask
  {
    /// <summary>
    /// Task Category (Build, Clean, PostBuild, etc.)
    /// </summary>
    string Category { get; set; }
    /// <summary>
    /// Task Type (Build, Clean, Test, etc)
    /// </summary>
    string TaskType { get; set; }
    /// <summary>
    /// Is Public Task (listed in help) or not
    /// </summary>
    bool IsTarget { get; set; }
    /// <summary>
    /// Task Action Delegate. From Cake ScriptHost Task(string) method
    /// </summary>
    ActionTask Task { get; set; }
    /// <summary>
    /// Task Name or Target name
    /// </summary>
    string TaskName { get; }
  }

  /// <summary>
  /// RunTarget Contract
  /// </summary>
  public interface IHelperRunTarget
  {
    /// <summary>
    /// Executes ActionTask for task
    /// </summary>
    /// <param name="task">HelperTask</param>
    /// <returns>CakeReport from task run</returns>
    CakeReport RunTarget(IHelperTask task);
    /// <summary>
    /// Executes ActionTask for task name
    /// </summary>
    /// <param name="targetName">Task/Target name</param>
    /// <returns>CakeReport from task run</returns>
    CakeReport RunTarget(string targetName);
  }

  /// <summary>
  /// Task Handler Contract
  /// </summary>
  /// <inheritdoc />
  public interface IHelperTaskHandler : IHelper
  {
    /// <summary>
    /// All Tasks defined
    /// </summary>
    IEnumerable<IHelperTask> Tasks { get; }
    /// <summary>
    /// Adds Task to TaskHelper
    /// </summary>
    /// <param name="taskName">Task Name</param>
    /// <returns>Helper Task</returns>
    IHelperTask AddTask(string taskName);
    /// <summary>
    /// Removes a task from TaskHelper
    /// </summary>
    /// <param name="taskName">Task Name</param>
    void RemoveTask(string taskName);
    /// <summary>
    /// Gets or sets flag to build entire dependency chain always
    /// </summary>
    bool BuildAllDependencies { get; set; }
  }

  /// <summary>
  /// TaskHelper Contract
  /// </summary>
  /// <inheritdoc cref="IHelperContext" />
  /// <inheritdoc cref="IHelperRunTarget" />
  /// <inheritdoc cref="IHelperTaskHandler" />
  public interface ITaskHelper : IHelperTaskHandler, IHelperRunTarget, IHelperContext
  { }
}
