using System;
using System.Collections.Generic;
using Cake.Core;
using Cake.Helpers.Settings;

namespace Cake.Helpers.Tasks
{
  internal class TaskHelper : IHelperContext, ITaskHelper
  {
    #region Private Properties

    private TaskCache Cache { get; } = new TaskCache();

    #endregion

    #region Private Fields

    private readonly IHelperSettings _HelperSettings = SingletonFactory.GetHelperSettings();

    #endregion

    #region Private Methods

    private CakeTaskBuilder<ActionTask> AddScriptTask(string taskName)
    {
      if (this._HelperSettings.TaskTargetFunc == null)
        throw new ArgumentNullException(nameof(this._HelperSettings.TaskTargetFunc));

      return this._HelperSettings.TaskTargetFunc?.Invoke(taskName);
    }

    #endregion

    #region IHelperContext Members

    /// <inheritdoc />
    public ICakeContext Context { get; set; }

    #endregion

    #region IHelperRunTarget Members

    /// <inheritdoc />
    public CakeReport RunTarget(IHelperTask task)
    {
      return this.RunTarget(task.TaskName);
    }

    /// <inheritdoc />
    public CakeReport RunTarget(string targetName)
    {
      if (this._HelperSettings.RunTargetFunc == null)
        throw new ArgumentNullException(nameof(this._HelperSettings.RunTargetFunc));

      return this._HelperSettings.RunTargetFunc?.Invoke(targetName);
    }

    #endregion

    #region IHelperTaskHandler Members

    /// <inheritdoc />
    public bool BuildAllDependencies
    {
      get { return this._HelperSettings.RunAllDependencies; }
      set { this._HelperSettings.RunAllDependencies = value; }
    }

    /// <inheritdoc />
    public IEnumerable<IHelperTask> Tasks
    {
      get { return this.Cache.Values; }
    }

    /// <inheritdoc />
    public IHelperTask AddTask(string taskName)
    {
      if (this.Cache.ContainsKey(taskName))
        return this.Cache[taskName];

      var newTask = this.AddScriptTask(taskName).Task;
      var newHelperTask = new HelperTask
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

    #endregion
  }
}