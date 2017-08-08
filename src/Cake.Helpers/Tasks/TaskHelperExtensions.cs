using System;
using System.Collections.Generic;
using System.Linq;
using Cake.Core;

namespace Cake.Helpers.Tasks
{
  /// <summary>
  /// Task Helper Extensions
  /// </summary>
  public static class TaskHelperExtensions
  {
    #region Static Members

    /// <summary>
    /// Adds dependency between tasks
    /// </summary>
    /// <param name="helper">TaskHelper</param>
    /// <param name="task">Task</param>
    /// <param name="dependentTaskName">Task name</param>
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

    /// <summary>
    /// Adds dependency between tasks
    /// </summary>
    /// <param name="helper">TaskHelper</param>
    /// <param name="task">Helper Task</param>
    /// <param name="dependentTaskName">Task name</param>
    public static void AddTaskDependency(
      this IHelperTaskHandler helper,
      IHelperTask task,
      string dependentTaskName)
    {
      if (task == null)
        throw new ArgumentNullException(nameof(task));

      helper.AddTaskDependency(task.GetTaskBuilder(), dependentTaskName);
    }

    /// <summary>
    /// Adds dependency between tasks
    /// </summary>
    /// <param name="helper">TaskHelper</param>
    /// <param name="task">Helper Task</param>
    /// <param name="dependentTask">Helper Task</param>
    /// <example>
    /// <code>
    /// var bCleanTask = BuildCleanTask("Sln", true)
    ///   .Does(() => { "Clean Solution" });
    /// var bTask = BuildTask("Sln", true)
    ///   .Does(() => { "Build Solution" });
    /// TaskHelper.AddTaskDependency(bTask, bCleanTask);
    /// </code>
    /// </example>
    public static void AddTaskDependency(
      this IHelperTaskHandler helper,
      IHelperTask task,
      IHelperTask dependentTask)
    {
      if (dependentTask == null)
        return;

      helper.AddTaskDependency(task, dependentTask.TaskName);
    }

    /// <summary>
    /// Gets a Task Builder (with Does, IsDependentOn, etc) for Helper Task
    /// </summary>
    /// <param name="task">Helper Task</param>
    /// <returns>Task Builder</returns>
    /// <example>
    /// <code>
    /// var bCleanTask = TaskHelper.AddTask("TaskTest");
    /// bCleanTask.GetTaskBuilder()
    ///   .Does(() => { "Clean Solution" });
    /// </code>
    /// </example>
    public static CakeTaskBuilder<ActionTask> GetTaskBuilder(this IHelperTask task)
    {
      if (task == null)
        return null;

      return new CakeTaskBuilder<ActionTask>(task.Task);
    }

    /// <summary>
    /// Adds or Gets task marked as public target
    /// </summary>
    /// <param name="helper">TaskHelper</param>
    /// <param name="taskName">Task/Target Name</param>
    /// <param name="category">Task Category</param>
    /// <param name="taskType">Task Type</param>
    /// <returns>Helper Task</returns>
    /// <example>
    /// <code>
    /// var bCleanTask = TaskHelper.GetTargetTask("TaskTest");
    /// </code>
    /// </example>
    public static IHelperTask GetTargetTask(
      this IHelperTaskHandler helper,
      string taskName,
      string category = "",
      string taskType = "")
    {
      return helper.GetTask(taskName, true, category, taskType);
    }

    internal static IEnumerable<string> GetCategories(this IEnumerable<IHelperTask> tasks)
    {
      return tasks.Select(t => t.Category).Distinct();
    }

    internal static IEnumerable<string> GetCategories(this IHelperTaskHandler helper)
    {
      if (helper == null)
        return Enumerable.Empty<string>();

      return helper.Tasks.GetCategories();
    }

    /// <summary>
    /// Gets all Tasks marked as Targets
    /// </summary>
    /// <param name="tasks">All Tasks</param>
    /// <returns>Target Tasks</returns>
    public static IEnumerable<IHelperTask> GetTargetTasks(this IEnumerable<IHelperTask> tasks)
    {
      return tasks.Where(t => t.IsTarget);
    }

    /// <summary>
    /// Adds or Gets task marked as public target
    /// </summary>
    /// <param name="helper">TaskHelper</param>
    /// <param name="taskName">Task/Target Name</param>
    /// <param name="isTarget">True=Public Target(Listed),False=Private Target(Not Listed)</param>
    /// <param name="category">Task Category</param>
    /// <param name="taskType">Task Type</param>
    /// <returns>Helper Task</returns>
    /// <example>
    /// <code>
    /// var bCleanTask = TaskHelper.GetTask("TaskTest", false);
    /// </code>
    /// </example>
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

    internal static IEnumerable<CakeTaskBuilder<ActionTask>> GetTaskBuilders(this IEnumerable<IHelperTask> tasks)
    {
      return tasks.Select(task => task.GetTaskBuilder());
    }

    #endregion
  }
}