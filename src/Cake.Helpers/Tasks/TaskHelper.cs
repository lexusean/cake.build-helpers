using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cake.Core;

namespace Cake.Helpers.Tasks
{
  public class TaskHelper : IHelperContext, IHelperRunTarget, IHelperTaskHandler
  {
    private TaskCache Cache { get; set; } = new TaskCache();
    private Func<string, CakeReport> RunTargetFunc { get; set; }
    private Func<string, CakeTaskBuilder<ActionTask>> ScriptHostTaskFunc { get; set; }

    /// <inheritdoc />
    public void SetRunTarget(Func<string, CakeReport> runTargetFunc)
    {
      this.RunTargetFunc = runTargetFunc;
    }

    /// <inheritdoc />
    public void RunTarget(IHelperTask task)
    {
      this.RunTarget(task.TaskName);
    }

    /// <inheritdoc />
    public void RunTarget(string targetName)
    {
      this.RunTargetFunc?.Invoke(targetName);
    }

    /// <inheritdoc />
    public IEnumerable<IHelperTask> Tasks
    {
      get { return this.Cache.Values; }
    }

    /// <inheritdoc />
    public void SetTaskTarget(Func<string, CakeTaskBuilder<ActionTask>> scriptHostTaskFunc)
    {
      this.ScriptHostTaskFunc = scriptHostTaskFunc;
    }

    /// <inheritdoc />
    public IHelperTask AddTask(string taskName)
    {
      if (this.Cache.ContainsKey(taskName))
        return this.Cache[taskName];

      if (this.ScriptHostTaskFunc == null)
        return null;

      var newTask = this.ScriptHostTaskFunc(taskName).Task;
      var newHelperTask = new HelperTask()
      {
        Task = newTask
      };

      this.Cache.Add(taskName, newHelperTask);

      return newHelperTask;
    }

    /// <inheritdoc />
    public void RemoveTask(string taskName)
    {
      this.Cache.Remove(taskName);
    }

    /// <inheritdoc />
    public bool BuildAllDependencies { get; set; }

    /// <inheritdoc />
    public ICakeContext Context { get; set; }
  }
}
