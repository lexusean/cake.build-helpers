using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cake.Core;

namespace Cake.Helpers.Tasks
{
  public interface IHelperTask
  {
    string Category { get; set; }
    string TaskType { get; set; }
    bool IsTarget { get; set; }
    ActionTask Task { get; set; }
    string TaskName { get; }
  }

  public interface IHelperRunTarget
  {
    void SetRunTarget(Func<string, CakeReport> runTargetFunc);
    void RunTarget(IHelperTask task);
    void RunTarget(string targetName);
  }

  public interface IHelperTaskHandler : IHelper
  {
    IEnumerable<IHelperTask> Tasks { get; }
    void SetTaskTarget(Func<string, CakeTaskBuilder<ActionTask>> scriptHostTaskFunc);
    IHelperTask AddTask(string taskName);
    void RemoveTask(string taskName);
    bool BuildAllDependencies { get; set; }
  }
}
