using System;
using System.Collections.Generic;
using System.Linq;
using Cake.Core;
using Cake.Core.IO;
using Cake.Helpers.Clean;
using Cake.Helpers.Settings;
using Cake.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Cake.Helpers.Tests.Unit.Clean
{
  [TestClass]
  public class CleanHelperAliasTests
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
    public void CleanTask_Default_Success()
    {
      var cleanCategory = "Generic";
      var category = "Clean";
      var taskType = $"{category}-{cleanCategory}";
      var parentTaskName = $"{taskType}-All";
      var taskName = "TestClean";

      var context = this.GetMoqContext(new Dictionary<string, bool>(), new Dictionary<string, string>());
      var settings = this.GetSettings(context);
      var cleanTask = context.CleanTask(taskName);

      var taskHelper = SingletonFactory.GetTaskHelper();

      Assert.IsNotNull(cleanTask);
      Assert.AreEqual($"{taskType}-{taskName}", cleanTask.Task.Name);

      var defaultCleanAllTask = taskHelper.Tasks.FirstOrDefault(t => t.TaskName == $"{category}-All");

      Assert.IsNotNull(defaultCleanAllTask);
      Assert.IsTrue(defaultCleanAllTask.IsTarget);
      Assert.AreEqual($"{category}-All", defaultCleanAllTask.TaskName);
      Assert.AreEqual(category, defaultCleanAllTask.Category);
      Assert.IsNotNull(defaultCleanAllTask.Task);
      Assert.IsTrue(defaultCleanAllTask.Task.Dependencies.Contains(parentTaskName));

      var cleanAllTask = taskHelper.Tasks.FirstOrDefault(t => t.TaskName == parentTaskName);

      Assert.IsNotNull(cleanAllTask);
      Assert.IsTrue(cleanAllTask.IsTarget);
      Assert.AreEqual(parentTaskName, cleanAllTask.TaskName);
      Assert.AreEqual(category, cleanAllTask.Category);
      Assert.AreEqual(taskType, cleanAllTask.TaskType);
      Assert.IsNotNull(cleanAllTask.Task);
      Assert.IsTrue(cleanAllTask.Task.Dependencies.Contains(cleanTask.Task.Name));

      var cleanHelperTask = taskHelper.Tasks.FirstOrDefault(t => t.Task == cleanTask.Task);

      Assert.IsNotNull(cleanHelperTask);
      Assert.IsTrue(cleanHelperTask.IsTarget);
      Assert.AreEqual($"{taskType}-{taskName}", cleanHelperTask.TaskName);
      Assert.AreEqual(category, cleanHelperTask.Category);
      Assert.AreEqual(taskType, cleanHelperTask.TaskType);
      Assert.IsNotNull(cleanHelperTask.Task);
      Assert.IsFalse(cleanHelperTask.Task.Dependencies.Any());
    }

    [TestMethod]
    [TestCategory(Global.TestType)]
    public void CleanTask_Category_Success()
    {
      var cleanCategory = "UnitTest";
      var category = "Clean";
      var taskType = $"{category}-{cleanCategory}";
      var parentTaskName = $"{taskType}-All";
      var taskName = "TestClean";

      var context = this.GetMoqContext(new Dictionary<string, bool>(), new Dictionary<string, string>());
      var settings = this.GetSettings(context);
      var cleanTask = context.CleanTask(taskName, cleanCategory);

      var taskHelper = SingletonFactory.GetTaskHelper();

      Assert.IsNotNull(cleanTask);
      Assert.AreEqual($"{taskType}-{taskName}", cleanTask.Task.Name);

      var cleanAllTask = taskHelper.Tasks.FirstOrDefault(t => t.TaskName == parentTaskName);

      Assert.IsNotNull(cleanAllTask);
      Assert.IsTrue(cleanAllTask.IsTarget);
      Assert.AreEqual(parentTaskName, cleanAllTask.TaskName);
      Assert.AreEqual(category, cleanAllTask.Category);
      Assert.AreEqual(taskType, cleanAllTask.TaskType);
      Assert.IsNotNull(cleanAllTask.Task);
      Assert.IsTrue(cleanAllTask.Task.Dependencies.Contains(cleanTask.Task.Name));

      var cleanHelperTask = taskHelper.Tasks.FirstOrDefault(t => t.Task == cleanTask.Task);

      Assert.IsNotNull(cleanHelperTask);
      Assert.IsTrue(cleanHelperTask.IsTarget);
      Assert.AreEqual($"{taskType}-{taskName}", cleanHelperTask.TaskName);
      Assert.AreEqual(category, cleanHelperTask.Category);
      Assert.AreEqual(taskType, cleanHelperTask.TaskType);
      Assert.IsNotNull(cleanHelperTask.Task);
      Assert.IsFalse(cleanHelperTask.Task.Dependencies.Any());
    }

    [TestMethod]
    [TestCategory(Global.TestType)]
    public void CleanTask_SubTask_Success()
    {
      var cleanCategory = "UnitTest";
      var category = "Clean";
      var taskType = $"{category}-{cleanCategory}";
      var parentTaskName = $"{taskType}-All";
      var taskName = "TestClean";
      var subtaskName = "Cleaning";

      var context = this.GetMoqContext(new Dictionary<string, bool>(), new Dictionary<string, string>());
      var settings = this.GetSettings(context);
      var cleanTask = context.CleanTask(subtaskName, cleanCategory, false, taskName);

      var taskHelper = SingletonFactory.GetTaskHelper();

      Assert.IsNotNull(cleanTask);
      Assert.AreEqual($"{taskType}-{taskName}-{subtaskName}", cleanTask.Task.Name);

      var parentTask = taskHelper.Tasks.FirstOrDefault(t => t.TaskName == $"{taskType}-{taskName}");

      Assert.IsNotNull(parentTask);
      Assert.IsTrue(parentTask.IsTarget);
      Assert.AreEqual($"{taskType}-{taskName}", parentTask.TaskName);
      Assert.AreEqual(category, parentTask.Category);
      Assert.AreEqual(taskType, parentTask.TaskType);
      Assert.IsNotNull(parentTask.Task);
      Assert.IsTrue(parentTask.Task.Dependencies.Contains(cleanTask.Task.Name));

      var cleanAllTask = taskHelper.Tasks.FirstOrDefault(t => t.TaskName == parentTaskName);

      Assert.IsNotNull(cleanAllTask);
      Assert.IsTrue(cleanAllTask.IsTarget);
      Assert.AreEqual(parentTaskName, cleanAllTask.TaskName);
      Assert.AreEqual(category, cleanAllTask.Category);
      Assert.AreEqual(taskType, cleanAllTask.TaskType);
      Assert.IsNotNull(cleanAllTask.Task);
      Assert.IsTrue(cleanAllTask.Task.Dependencies.Contains(parentTask.TaskName));

      var cleanHelperTask = taskHelper.Tasks.FirstOrDefault(t => t.Task == cleanTask.Task);

      Assert.IsNotNull(cleanHelperTask);
      Assert.IsFalse(cleanHelperTask.IsTarget);
      Assert.AreEqual($"{taskType}-{taskName}-{subtaskName}", cleanHelperTask.TaskName);
      Assert.AreEqual(category, cleanHelperTask.Category);
      Assert.AreEqual(taskType, cleanHelperTask.TaskType);
      Assert.IsNotNull(cleanHelperTask.Task);
      Assert.IsFalse(cleanHelperTask.Task.Dependencies.Any());
    }

    [TestMethod]
    [TestCategory(Global.TestType)]
    public void CleanTask_NoContext()
    {
      var cleanCategory = "Generic";
      var category = "Clean";
      var taskType = $"{category}-{cleanCategory}";
      var parentTaskName = $"{taskType}-All";
      var taskName = "TestClean";

      ICakeContext context = null;
      Assert.ThrowsException<ArgumentNullException>(() => context.CleanTask(taskName));
    }

    #endregion

    #region Test Helpers

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

    #endregion
  }
}