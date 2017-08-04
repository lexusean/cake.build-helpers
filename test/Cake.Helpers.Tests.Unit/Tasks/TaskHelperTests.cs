using System;
using System.Collections.Generic;
using System.Linq;
using Cake.Core;
using Cake.Core.IO;
using Cake.Helpers.Build;
using Cake.Helpers.Settings;
using Cake.Helpers.Tasks;
using Cake.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Cake.Helpers.Tests.Unit.Tasks
{
  [TestClass]
  public class TaskHelperTests
  {
    #region Test Setup and Teardown

    [TestInitialize]
    public void TestInit()
    {
      SingletonFactory.ClearFactory();
    }

    #endregion

    #region Test Methods

    [TestMethod]
    [TestCategory(Global.TestType)]
    public void TaskHelper_AddTask_ExistsInCache()
    {
      var target = "Test Target";

      var context = this.GetMoqContext(new Dictionary<string, bool>(), new Dictionary<string, string>());
      SingletonFactory.Context = context;
      var settings = SingletonFactory.GetHelperSettings();

      var targetRanSuccess = false;
      settings.TaskTargetFunc = taskName => { return new CakeTaskBuilder<ActionTask>(new ActionTask(taskName)); };

      settings.RunTargetFunc = targ =>
      {
        targetRanSuccess = targ == target;
        return new CakeReport();
      };

      var taskHelper = SingletonFactory.GetTaskHelper();
      Assert.IsNotNull(taskHelper);

      var firstTask = taskHelper.AddTask(target);
      Assert.IsNotNull(firstTask);

      var nextTask = taskHelper.AddTask(target);
      Assert.IsNotNull(nextTask);
      Assert.AreEqual(firstTask, nextTask);
    }

    [TestMethod]
    [TestCategory(Global.TestType)]
    public void TaskHelper_AddTaskDependency_DependentTaskExists()
    {
      var context = this.GetMoqContext(new Dictionary<string, bool>(), new Dictionary<string, string>());
      SingletonFactory.Context = context;

      var settings = SingletonFactory.GetHelperSettings();
      settings.TaskTargetFunc = taskName => { return new CakeTaskBuilder<ActionTask>(new ActionTask(taskName)); };

      var taskHelper = SingletonFactory.GetTaskHelper();
      Assert.IsNotNull(taskHelper);

      var firstTask = taskHelper.AddTask("test");
      var nextTask = taskHelper.AddTask("test2");

      Assert.AreEqual(0, firstTask.Task.Dependencies.Count);
      Assert.AreEqual(0, nextTask.Task.Dependencies.Count);

      taskHelper.AddTaskDependency(firstTask, nextTask);

      Assert.AreEqual(1, firstTask.Task.Dependencies.Count);
      Assert.IsTrue(firstTask.Task.Dependencies.Contains("test2"));
      Assert.AreEqual(0, nextTask.Task.Dependencies.Count);

      taskHelper.AddTaskDependency(firstTask, nextTask);

      Assert.AreEqual(1, firstTask.Task.Dependencies.Count);
      Assert.IsTrue(firstTask.Task.Dependencies.Contains("test2"));
      Assert.AreEqual(0, nextTask.Task.Dependencies.Count);
    }

    [TestMethod]
    [TestCategory(Global.TestType)]
    public void TaskHelper_AddTaskDependency_NoBuilderTask()
    {
      var context = this.GetMoqContext(new Dictionary<string, bool>(), new Dictionary<string, string>());
      SingletonFactory.Context = context;

      var settings = SingletonFactory.GetHelperSettings();
      settings.TaskTargetFunc = taskName => { return new CakeTaskBuilder<ActionTask>(new ActionTask(taskName)); };

      var taskHelper = SingletonFactory.GetTaskHelper();
      Assert.IsNotNull(taskHelper);

      CakeTaskBuilder<ActionTask> firstTask = null;

      Assert.ThrowsException<ArgumentNullException>(() => taskHelper.AddTaskDependency(firstTask, "test"));
    }

    [TestMethod]
    [TestCategory(Global.TestType)]
    public void TaskHelper_AddTaskDependency_NoDependentTask()
    {
      var context = this.GetMoqContext(new Dictionary<string, bool>(), new Dictionary<string, string>());
      SingletonFactory.Context = context;

      var settings = SingletonFactory.GetHelperSettings();
      settings.TaskTargetFunc = taskName => { return new CakeTaskBuilder<ActionTask>(new ActionTask(taskName)); };

      var taskHelper = SingletonFactory.GetTaskHelper();
      Assert.IsNotNull(taskHelper);

      var firstTask = taskHelper.AddTask("test");
      IHelperTask nextTask = null;

      Assert.AreEqual(0, firstTask.Task.Dependencies.Count);

      taskHelper.AddTaskDependency(firstTask, nextTask);

      Assert.AreEqual(0, firstTask.Task.Dependencies.Count);
    }

    [TestMethod]
    [TestCategory(Global.TestType)]
    public void TaskHelper_AddTaskDependency_NoDependentTaskName()
    {
      var context = this.GetMoqContext(new Dictionary<string, bool>(), new Dictionary<string, string>());
      SingletonFactory.Context = context;

      var settings = SingletonFactory.GetHelperSettings();
      settings.TaskTargetFunc = taskName => { return new CakeTaskBuilder<ActionTask>(new ActionTask(taskName)); };

      var taskHelper = SingletonFactory.GetTaskHelper();
      Assert.IsNotNull(taskHelper);

      var firstTask = taskHelper.AddTask("test");

      Assert.AreEqual(0, firstTask.Task.Dependencies.Count);

      Assert.ThrowsException<ArgumentNullException>(() => taskHelper.AddTaskDependency(firstTask, string.Empty));
    }

    [TestMethod]
    [TestCategory(Global.TestType)]
    public void TaskHelper_AddTaskDependency_NoTask()
    {
      var context = this.GetMoqContext(new Dictionary<string, bool>(), new Dictionary<string, string>());
      SingletonFactory.Context = context;

      var settings = SingletonFactory.GetHelperSettings();
      settings.TaskTargetFunc = taskName => { return new CakeTaskBuilder<ActionTask>(new ActionTask(taskName)); };

      var taskHelper = SingletonFactory.GetTaskHelper();
      Assert.IsNotNull(taskHelper);

      IHelperTask firstTask = null;

      Assert.ThrowsException<ArgumentNullException>(() => taskHelper.AddTaskDependency(firstTask, "test"));
    }

    [TestMethod]
    [TestCategory(Global.TestType)]
    public void TaskHelper_Change_BuildAllDependencies()
    {
      var context = this.GetMoqContext(new Dictionary<string, bool>(), new Dictionary<string, string>());
      SingletonFactory.Context = context;

      var settings = SingletonFactory.GetHelperSettings();
      Assert.IsNotNull(settings);
      Assert.AreEqual(false, settings.RunAllDependencies);

      var taskHelper = SingletonFactory.GetTaskHelper();
      Assert.IsNotNull(taskHelper);
      Assert.AreEqual(false, taskHelper.BuildAllDependencies);

      taskHelper.BuildAllDependencies = true;

      Assert.AreEqual(true, settings.RunAllDependencies);
      Assert.AreEqual(true, taskHelper.BuildAllDependencies);
    }

    [TestMethod]
    [TestCategory(Global.TestType)]
    public void TaskHelper_GetBuildTask_NoTask()
    {
      var context = this.GetMoqContext(new Dictionary<string, bool>(), new Dictionary<string, string>());
      SingletonFactory.Context = context;

      var settings = SingletonFactory.GetHelperSettings();
      settings.TaskTargetFunc = taskName => { return new CakeTaskBuilder<ActionTask>(new ActionTask(taskName)); };

      var taskHelper = SingletonFactory.GetTaskHelper();
      Assert.IsNotNull(taskHelper);

      IHelperTask firstTask = null;
      var buildTask = firstTask.GetTaskBuilder();

      Assert.IsNull(buildTask);
    }

    [TestMethod]
    [TestCategory(Global.TestType)]
    public void TaskHelper_NoScriptTaskFunc()
    {
      var target = "Test Target";

      var context = this.GetMoqContext(new Dictionary<string, bool>(), new Dictionary<string, string>());
      SingletonFactory.Context = context;
      var settings = SingletonFactory.GetHelperSettings();

      var targetRanSuccess = false;
      settings.RunTargetFunc = targ =>
      {
        targetRanSuccess = targ == target;
        return new CakeReport();
      };

      Assert.ThrowsException<ArgumentNullException>(() => context.HelperTask(target));
    }

    [TestMethod]
    [TestCategory(Global.TestType)]
    public void TaskHelper_RemoveTask_Success()
    {
      var target = "Test Target";

      var context = this.GetMoqContext(new Dictionary<string, bool>(), new Dictionary<string, string>());
      SingletonFactory.Context = context;

      var settings = SingletonFactory.GetHelperSettings();
      settings.TaskTargetFunc = taskName => { return new CakeTaskBuilder<ActionTask>(new ActionTask(taskName)); };

      var taskHelper = SingletonFactory.GetTaskHelper();
      Assert.IsNotNull(taskHelper);

      var firstTask = taskHelper.AddTask(target);
      Assert.IsNotNull(firstTask);
      Assert.AreEqual(1, taskHelper.Tasks.Count());

      taskHelper.RemoveTask(target);
      Assert.AreEqual(0, taskHelper.Tasks.Count());
    }

    [TestMethod]
    [TestCategory(Global.TestType)]
    public void TaskHelper_RunTarget_NoRunTargetFunc()
    {
      var target = "Test Target";

      var context = this.GetMoqContext(new Dictionary<string, bool>(), new Dictionary<string, string>());
      SingletonFactory.Context = context;
      var settings = SingletonFactory.GetHelperSettings();

      var taskHelper = SingletonFactory.GetTaskHelper();
      Assert.IsNotNull(taskHelper);

      Assert.ThrowsException<ArgumentNullException>(() => taskHelper.RunTarget(target));
    }

    [TestMethod]
    [TestCategory(Global.TestType)]
    public void TaskHelper_RunTarget_String_Success()
    {
      var target = "Test Target";

      var context = this.GetMoqContext(new Dictionary<string, bool>(), new Dictionary<string, string>());
      SingletonFactory.Context = context;
      var settings = SingletonFactory.GetHelperSettings();

      var targetRanSuccess = false;
      settings.RunTargetFunc = targ =>
      {
        targetRanSuccess = targ == target;
        return new CakeReport();
      };

      var taskHelper = SingletonFactory.GetTaskHelper();
      Assert.IsNotNull(taskHelper);

      var report = taskHelper.RunTarget(target);
      Assert.IsNotNull(report);
      Assert.IsTrue(targetRanSuccess);
    }

    [TestMethod]
    [TestCategory(Global.TestType)]
    public void TaskHelper_RunTarget_Task_Success()
    {
      var target = "Test Target";

      var context = this.GetMoqContext(new Dictionary<string, bool>(), new Dictionary<string, string>());
      SingletonFactory.Context = context;
      var settings = SingletonFactory.GetHelperSettings();

      var targetRanSuccess = false;
      settings.TaskTargetFunc = taskName => { return new CakeTaskBuilder<ActionTask>(new ActionTask(taskName)); };

      settings.RunTargetFunc = targ =>
      {
        targetRanSuccess = targ == target;
        return new CakeReport();
      };

      var targetTaskBuilder = context.HelperTask(target);

      var taskHelper = SingletonFactory.GetTaskHelper();
      Assert.IsNotNull(taskHelper);

      var helperTask = taskHelper.Tasks.FirstOrDefault(t => t.Task == targetTaskBuilder.Task);
      Assert.IsNotNull(helperTask);

      var report = taskHelper.RunTarget(helperTask);
      Assert.IsNotNull(report);
      Assert.IsTrue(targetRanSuccess);
    }

    [TestMethod]
    [TestCategory(Global.TestType)]
    public void TaskHelper_GetCategoriesForTasks()
    {
      var context = this.GetMoqContext(new Dictionary<string, bool>(), new Dictionary<string, string>());
      SingletonFactory.Context = context;
      var settings = SingletonFactory.GetHelperSettings();
      settings.TaskTargetFunc = taskName => { return new CakeTaskBuilder<ActionTask>(new ActionTask(taskName)); };
      
      var taskHelper = SingletonFactory.GetTaskHelper();
      Assert.IsNotNull(taskHelper);

      var firstTask = context.HelperTask("test1");
      var nextTask = context.HelperTask("test2", true, "Test");

      var categories = taskHelper.GetCategories().ToArray();
      Assert.AreEqual(2, categories.Length);
      Assert.IsTrue(categories.Contains("Generic"));
      Assert.IsTrue(categories.Contains("Test"));
    }

    [TestMethod]
    [TestCategory(Global.TestType)]
    public void TaskHelper_GetCategoriesForTasks_NoHelper()
    {
      var context = this.GetMoqContext(new Dictionary<string, bool>(), new Dictionary<string, string>());
      SingletonFactory.Context = context;
      var settings = SingletonFactory.GetHelperSettings();
      settings.TaskTargetFunc = taskName => { return new CakeTaskBuilder<ActionTask>(new ActionTask(taskName)); };

      var taskHelper = SingletonFactory.GetTaskHelper();
      Assert.IsNotNull(taskHelper);

      var firstTask = context.HelperTask("test1");
      var nextTask = context.HelperTask("test2", true, "Test");

      IHelperTaskHandler nullHelper = null;
      var categories = nullHelper.GetCategories().ToArray();
      Assert.AreEqual(0, categories.Length);
    }

    [TestMethod]
    [TestCategory(Global.TestType)]
    public void TaskHelper_GetTaskBuilders()
    {
      var context = this.GetMoqContext(new Dictionary<string, bool>(), new Dictionary<string, string>());
      SingletonFactory.Context = context;
      var settings = SingletonFactory.GetHelperSettings();
      settings.TaskTargetFunc = taskName => { return new CakeTaskBuilder<ActionTask>(new ActionTask(taskName)); };

      var taskHelper = SingletonFactory.GetTaskHelper();
      Assert.IsNotNull(taskHelper);

      var firstTask = context.HelperTask("test1");
      var nextTask = context.HelperTask("test2", true, "Test");

      var taskBuilders = taskHelper.Tasks.GetTaskBuilders().ToArray();

      Assert.AreEqual(2, taskBuilders.Length);
      Assert.IsTrue(taskBuilders.Any(t => t.Task.Name == "test1"));
      Assert.IsTrue(taskBuilders.Any(t => t.Task.Name == "test2"));
    }

    [TestMethod]
    [TestCategory(Global.TestType)]
    public void TaskHelper_GetTargetTasks()
    {
      var context = this.GetMoqContext(new Dictionary<string, bool>(), new Dictionary<string, string>());
      SingletonFactory.Context = context;
      var settings = SingletonFactory.GetHelperSettings();
      settings.TaskTargetFunc = taskName => { return new CakeTaskBuilder<ActionTask>(new ActionTask(taskName)); };

      var taskHelper = SingletonFactory.GetTaskHelper();
      Assert.IsNotNull(taskHelper);

      var firstTask = context.HelperTask("test1");
      var nextTask = context.HelperTask("test2", false, "Generic", "test1");

      var targetTasks = taskHelper.Tasks.GetTargetTasks().ToArray();

      Assert.AreEqual(1, targetTasks.Length);
      Assert.IsTrue(targetTasks.Any(t => t.Task.Name == "test1"));
    }

    [TestMethod]
    [TestCategory(Global.TestType)]
    public void TaskHelper_GetTargetTask_Success()
    {
      var context = this.GetMoqContext(new Dictionary<string, bool>(), new Dictionary<string, string>());
      SingletonFactory.Context = context;
      var settings = SingletonFactory.GetHelperSettings();
      settings.TaskTargetFunc = taskName => { return new CakeTaskBuilder<ActionTask>(new ActionTask(taskName)); };

      var taskHelper = SingletonFactory.GetTaskHelper();
      Assert.IsNotNull(taskHelper);

      Assert.IsFalse(taskHelper.Tasks.Any());

      var firstTask = taskHelper.GetTargetTask("test1");
      Assert.IsNotNull(firstTask);

      var targetTasks = taskHelper.Tasks.ToArray();

      Assert.AreEqual(1, targetTasks.Length);
      Assert.IsTrue(targetTasks.Any(t => t.Task.Name == "test1" && t.IsTarget && t.Category == "Generic" && t.TaskType == "Unknown"));
    }

    #endregion

    #region Test Helpers

    private ICakeArguments GetMoqArguments(
      IDictionary<string, bool> hasArgs,
      IDictionary<string, string> argValues)
    {
      var argsMock = new Mock<ICakeArguments>();
      argsMock.Setup(t => t.HasArgument(It.IsAny<string>()))
        .Returns((string arg) =>
        {
          if (!hasArgs.ContainsKey(arg))
            return false;

          return hasArgs[arg];
        });

      argsMock.Setup(t => t.GetArgument(It.IsAny<string>()))
        .Returns((string arg) =>
        {
          if (!argValues.ContainsKey(arg))
            return string.Empty;

          return argValues[arg];
        });

      return argsMock.Object;
    }

    private ICakeContext GetMoqContext(
      IDictionary<string, bool> hasArgs,
      IDictionary<string, string> argValues)
    {
      var fixture = HelperFixture.CreateFixture();
      var args = this.GetMoqArguments(hasArgs, argValues);
      var globber = this.GetMoqGlobber(fixture.FileSystem, fixture.Environment);
      var reg = this.GetMoqRegistry();

      return this.GetMoqContext(fixture, globber, reg, args);
    }

    private ICakeContext GetMoqContext(
      HelperFixture fixture,
      IGlobber globber,
      IRegistry registry,
      ICakeArguments args)
    {
      var log = new FakeLog();

      var contextMock = new Mock<ICakeContext>();
      contextMock.SetupGet(t => t.FileSystem).Returns(fixture.FileSystem);
      contextMock.SetupGet(t => t.Environment).Returns(fixture.Environment);
      contextMock.SetupGet(t => t.Globber).Returns(globber);
      contextMock.SetupGet(t => t.Log).Returns(log);
      contextMock.SetupGet(t => t.Arguments).Returns(args);
      contextMock.SetupGet(t => t.ProcessRunner).Returns(fixture.ProcessRunner);
      contextMock.SetupGet(t => t.Registry).Returns(registry);
      contextMock.SetupGet(t => t.Tools).Returns(fixture.Tools);

      return contextMock.Object;
    }

    private IGlobber GetMoqGlobber(
      IFileSystem fs,
      ICakeEnvironment env)
    {
      return new Globber(fs, env);
    }

    private IRegistry GetMoqRegistry()
    {
      var regMock = new Mock<IRegistry>();
      regMock.SetupGet(t => t.LocalMachine).Returns((IRegistryKey) null);

      return regMock.Object;
    }

    private IHelperSettings GetSettings(ICakeContext context)
    {
      SingletonFactory.Context = context;
      var settings = SingletonFactory.GetHelperSettings();
      settings.TaskTargetFunc = taskName =>
      {
        var task = new ActionTask(taskName);
        return new CakeTaskBuilder<ActionTask>(task);
      };

      settings.RunTargetFunc = targetName =>
      {
        var report = new CakeReport {{targetName, TimeSpan.Zero, CakeTaskExecutionStatus.Executed}};

        return report;
      };

      return settings;
    }

    #endregion
  }
}