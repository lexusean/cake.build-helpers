using System;
using System.Collections.Generic;
using System.Linq;
using Cake.Core;

namespace Cake.Helpers.Tasks
{
  public static class TaskHelperExtensions
  {
    #region Static Members

    public static void AddTaskDependency(
      this IHelperTaskHandler helper,
      CakeTaskBuilder<ActionTask> task,
      string dependentTaskName)
    {
      if (task == null)
        throw new ArgumentNullException(nameof(task));

      if(string.IsNullOrWhiteSpace(dependentTaskName))
        throw new ArgumentNullException(nameof(dependentTaskName));

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
        throw new ArgumentNullException(nameof(task));

      helper.AddTaskDependency(task.GetTaskBuilder(), dependentTaskName);
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

    public static CakeTaskBuilder<ActionTask> GetTaskBuilder(this IHelperTask task)
    {
      if (task == null)
        return null;

      return new CakeTaskBuilder<ActionTask>(task.Task);
    }

    public static IHelperTask GetTargetTask(
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
      return tasks.Select(task => task.GetTaskBuilder());
    }

    #endregion
  }
}