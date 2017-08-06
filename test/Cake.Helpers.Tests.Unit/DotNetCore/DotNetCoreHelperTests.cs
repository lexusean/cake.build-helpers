using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using Cake.Common.IO;
using Cake.Core;
using Cake.Core.IO;
using Cake.Helpers.Clean;
using Cake.Helpers.DotNetCore;
using Cake.Helpers.Settings;
using Cake.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Cake.Helpers.Tests.Unit.DotNetCore
{
  [TestClass]
  public class DotNetCoreHelperTests
  {
    #region Test Setup and Teardown

    public TestContext TestContext { get; set; }

    [TestInitialize]
    public void TestInit()
    {
      SingletonFactory.ClearFactory();
    }

    #endregion

    #region Test Methods

    [TestMethod]
    [TestCategory(Global.TestType)]
    public void DotNetCoreHelper_TempFolders_Success()
    {
      var context = this.GetMoqContext(new Dictionary<string, bool>(), new Dictionary<string, string>());
      var defaultSln = SolutionFixture.GetDefaultSolution(context.FileSystem);
      SingletonFactory.Context = context;
      var helper = SingletonFactory.GetDotNetCoreHelper();

      Assert.IsNotNull(helper.BuildTempFolder);
      Assert.AreEqual("BuildTemp" , helper.BuildTempFolder.FullPath);
      Assert.IsNotNull(helper.TestTempFolder);
      Assert.AreEqual("TestTemp", helper.TestTempFolder.FullPath);
    }

    [TestMethod]
    [TestCategory(Global.TestType)]
    public void DotNetCoreHelper_Projects_Success()
    {
      var context = this.GetMoqContext(new Dictionary<string, bool>(), new Dictionary<string, string>());
      var settings = this.GetSettings(context);
      var defaultSln = SolutionFixture.GetDefaultSolution(context.FileSystem);
      SingletonFactory.Context = context;
      var dotNethelper = SingletonFactory.GetDotNetCoreHelper();
      var projConfig = context.AddDotNetCoreProject(defaultSln.SlnFile.Path.FullPath);
      projConfig.AddTestConfig("Unit");
      settings.SetupSetting();

      var anotherConfig = dotNethelper.AddProjectConfiguration(projConfig);

      Assert.AreEqual(1, dotNethelper.Projects.Count());
      Assert.AreEqual(1, dotNethelper.Tests.Count());
      Assert.AreEqual(projConfig, anotherConfig);

      projConfig.AddTestConfig("System");
      Assert.AreEqual(2, dotNethelper.Tests.Count());

      var newProjConfig = dotNethelper.GetProjectConfiguration(defaultSln.SlnFile.Path.FullPath);
      Assert.IsNotNull(newProjConfig);
      var fancyProjConfig = dotNethelper.AddProjectConfiguration(defaultSln.SlnFile.Path.FullPath, config =>
      {
        config.ProjectAlias = "TestProjects";
      });

      Assert.IsNotNull(fancyProjConfig);
      Assert.AreEqual(2, dotNethelper.Projects.Count());


      Assert.ThrowsException<ArgumentNullException>(() => ((IDotNetCoreHelper) null)
        .AddProjectConfiguration("slh.sln"));
      Assert.ThrowsException<ArgumentNullException>(() => dotNethelper.AddProjectConfiguration(string.Empty));
      Assert.ThrowsException<FileNotFoundException>(() => dotNethelper.AddProjectConfiguration("test.sln"));
    }

    [TestMethod]
    [TestCategory(Global.TestType)]
    public void DotNetCoreAlias_AddProject_NoContext()
    {
      ICakeContext context = null;
      Assert.ThrowsException<ArgumentNullException>(() => context.AddDotNetCoreProject("tst.sln"));
    }

    [TestMethod]
    [TestCategory(Global.TestType)]
    public void DotNetCoreAlias_AddProject_EmptySlnFile()
    {
      var context = this.GetMoqContext(new Dictionary<string, bool>(), new Dictionary<string, string>());
      Assert.ThrowsException<ArgumentNullException>(() => context.AddDotNetCoreProject(string.Empty));
    }

    [TestMethod]
    [TestCategory(Global.TestType)]
    public void DotNetCoreAlias_AddProject_FileNotFound()
    {
      var context = this.GetMoqContext(new Dictionary<string, bool>(), new Dictionary<string, string>());
      Assert.ThrowsException<FileNotFoundException>(() => context.AddDotNetCoreProject("test.sln"));
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