using System;
using System.Collections.Generic;
using System.Linq;
using Cake.Core;

namespace Cake.Helpers.Tasks
{
  public static class TaskHelperExtensions
  {
    #region Static Members

    public static TaskHelper AddRunTarget(this TaskHelper helper, Func<string, CakeReport> runTargetFunc)
    {
      if (helper == null)
        throw new ArgumentNullException(nameof(helper));

      helper.SetRunTarget(runTargetFunc);

      return helper;
    }

    public static void AddTaskDependency(
      this IHelperTaskHandler helper,
      CakeTaskBuilder<ActionTask> task,
      string dependentTaskName)
    {
      if (task == null)
        return;

      if (task.Task.Dependencies.Any(t => t == dependentTaskName))
        return;

      task.IsDependentOn(dependentTaskName);
    }

    public static void AddTaskDependency(
      this IHelperTaskHandler helper,
      IHelperTask task,
      string dependentTaskName)
    {
      if (task == null)
        return;

      helper.AddTaskDependency(new CakeTaskBuilder<ActionTask>(task.Task), dependentTaskName);
    }

    public static void AddTaskDependency(
      this IHelperTaskHandler helper,
      IHelperTask task,
      IHelperTask dependentTask)
    {
      if (dependentTask == null)
        return;

      helper.AddTaskDependency(task, dependentTask.TaskName);
    }

    public static TaskHelper AddTaskTarget(this TaskHelper helper,
      Func<string, CakeTaskBuilder<ActionTask>> taskTargetFunc)
    {
      if (helper == null)
        throw new ArgumentNullException(nameof(helper));

      helper.SetTaskTarget(taskTargetFunc);

      return helper;
    }

    public static CakeTaskBuilder<ActionTask> GetBuildTask(this IHelperTask task)
    {
      if (task == null)
        return null;

      return new CakeTaskBuilder<ActionTask>(task.Task);
    }

    public static IHelperTask GetBuildTask(
      this IHelperTaskHandler helper,
      string taskName,
      string category = "",
      string taskType = "")
    {
      return helper.GetTask(taskName, true, category, taskType);
    }

    public static IEnumerable<string> GetCategories(this IEnumerable<IHelperTask> tasks)
    {
      return tasks.Select(t => t.Category).Distinct();
    }

    public static IEnumerable<string> GetCategories(this IHelperTaskHandler helper)
    {
      if (helper == null)
        return Enumerable.Empty<string>();

      return helper.Tasks.GetCategories();
    }

    public static string GetCategoryForTask(this IHelperTaskHandler helper, string taskName)
    {
      if (helper == null)
        return string.Empty;

      return helper.Tasks
        .Where(t => t.Category == taskName)
        .Select(t => t.Category)
        .FirstOrDefault();
    }

    public static IEnumerable<IHelperTask> GetTargetTasks(this IEnumerable<IHelperTask> tasks)
    {
      return tasks.Where(t => t.IsTarget);
    }

    public static IHelperTask GetTask(
      this IHelperTaskHandler helper,
      string taskName,
      bool isTarget = false,
      string category = "",
      string taskType = "")
    {
      var task = helper.AddTask(taskName);
      task.IsTarget = isTarget;
      task.Category = category;
      task.TaskType = taskType;

      return task;
    }

    public static IEnumerable<CakeTaskBuilder<ActionTask>> GetTaskBuilders(this IEnumerable<IHelperTask> tasks)
    {
      return tasks.Select(task => task.GetBuildTask());
    }

    #endregion
  }
}