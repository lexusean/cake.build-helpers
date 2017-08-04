using System;
using System.Collections.Generic;
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
  public class DotNetCoreAliasTests
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
    public void DotNetCoreAlias_HelperSettings_Success()
    {
      var context = this.GetMoqContext(new Dictionary<string, bool>(), new Dictionary<string, string>());
      Assert.IsFalse(SingletonFactory.ExistsInCache(typeof(HelperSettings)));

      var dotCoreSettings = context.DotNetCoreHelperSettings();
      Assert.IsNotNull(dotCoreSettings);
      Assert.IsTrue(SingletonFactory.ExistsInCache(typeof(HelperSettings)));

      var helperSettings = SingletonFactory.GetHelperSettings();
      Assert.IsNotNull(helperSettings);
      Assert.AreEqual(helperSettings.DotNetCoreSettings, dotCoreSettings);
    }

    [TestMethod]
    [TestCategory(Global.TestType)]
    public void DotNetCoreAlias_HelperSettings_NoContext()
    {
      ICakeContext context = null;
      Assert.IsFalse(SingletonFactory.ExistsInCache(typeof(HelperSettings)));

      Assert.ThrowsException<ArgumentNullException>(() => context.DotNetCoreHelperSettings());
    }

    [TestMethod]
    [TestCategory(Global.TestType)]
    public void DotNetCoreAlias_BuildHelperSettings_Success()
    {
      var context = this.GetMoqContext(new Dictionary<string, bool>(), new Dictionary<string, string>());
      Assert.IsFalse(SingletonFactory.ExistsInCache(typeof(HelperSettings)));

      var dotCoreSettings = context.DotNetCoreBuildHelperSettings();
      Assert.IsNotNull(dotCoreSettings);
      Assert.IsTrue(SingletonFactory.ExistsInCache(typeof(HelperSettings)));

      var helperSettings = SingletonFactory.GetHelperSettings();
      Assert.IsNotNull(helperSettings);
      Assert.AreEqual(helperSettings.DotNetCoreSettings.BuildSettings, dotCoreSettings);
    }

    [TestMethod]
    [TestCategory(Global.TestType)]
    public void DotNetCoreAlias_BuildHelperSettings_NoContext()
    {
      ICakeContext context = null;
      Assert.IsFalse(SingletonFactory.ExistsInCache(typeof(HelperSettings)));

      Assert.ThrowsException<ArgumentNullException>(() => context.DotNetCoreBuildHelperSettings());
    }

    [TestMethod]
    [TestCategory(Global.TestType)]
    public void DotNetCoreAlias_TestHelperSettings_Success()
    {
      var context = this.GetMoqContext(new Dictionary<string, bool>(), new Dictionary<string, string>());
      Assert.IsFalse(SingletonFactory.ExistsInCache(typeof(HelperSettings)));

      var dotCoreSettings = context.DotNetCoreTestHelperSettings();
      Assert.IsNotNull(dotCoreSettings);
      Assert.IsTrue(SingletonFactory.ExistsInCache(typeof(HelperSettings)));

      var helperSettings = SingletonFactory.GetHelperSettings();
      Assert.IsNotNull(helperSettings);
      Assert.AreEqual(helperSettings.DotNetCoreSettings.TestSettings, dotCoreSettings);
    }

    [TestMethod]
    [TestCategory(Global.TestType)]
    public void DotNetCoreAlias_TestHelperSettings_NoContext()
    {
      ICakeContext context = null;
      Assert.IsFalse(SingletonFactory.ExistsInCache(typeof(HelperSettings)));

      Assert.ThrowsException<ArgumentNullException>(() => context.DotNetCoreTestHelperSettings());
    }

    [TestMethod]
    [TestCategory(Global.TestType)]
    public void DotNetCoreAlias_AddNugetSource_Success()
    {
      var feedUrl1 = "testUrl";
      var feedUrl2 = "blahUrl";
      var feedName2 = "blah";

      var context = this.GetMoqContext(new Dictionary<string, bool>(), new Dictionary<string, string>());
      Assert.IsFalse(SingletonFactory.ExistsInCache(typeof(HelperSettings)));

      context.AddDotNetCoreHelperNugetSource(feedUrl1);

      var helperSetting = SingletonFactory.GetHelperSettings();
      Assert.IsNotNull(helperSetting);

      var nugetSources = helperSetting.DotNetCoreSettings.NugetSettings.NugetSources.ToArray();
      Assert.AreEqual(2, nugetSources.Length);
      Assert.IsTrue(nugetSources.Any(t => t.FeedSource == feedUrl1 && string.IsNullOrWhiteSpace(t.FeedName)));

      context.AddDotNetCoreHelperNugetSource(feedUrl2, config =>
      {
        config.FeedName = feedName2;
      });

      nugetSources = helperSetting.DotNetCoreSettings.NugetSettings.NugetSources.ToArray();
      Assert.AreEqual(3, nugetSources.Length);
      Assert.IsTrue(nugetSources.Any(t => t.FeedSource == feedUrl1 && string.IsNullOrWhiteSpace(t.FeedName)));
      Assert.IsTrue(nugetSources.Any(t => t.FeedSource == feedUrl2 && t.FeedName == feedName2));
    }

    [TestMethod]
    [TestCategory(Global.TestType)]
    public void DotNetCoreAlias_AddNugetSource_NoFeedUrl()
    {
      var context = this.GetMoqContext(new Dictionary<string, bool>(), new Dictionary<string, string>());

      Assert.ThrowsException<ArgumentNullException>(() => context.AddDotNetCoreHelperNugetSource(string.Empty));
    }

    [TestMethod]
    [TestCategory(Global.TestType)]
    public void DotNetCoreAlias_AddNugetSource_NoContext()
    {
      ICakeContext context = null;

      Assert.ThrowsException<ArgumentNullException>(() => context.AddDotNetCoreHelperNugetSource("test1"));
    }

    [TestMethod]
    [TestCategory(Global.TestType)]
    public void DotNetCoreAlias_AddProject_Default_Success()
    {
      var context = this.GetMoqContext(new Dictionary<string, bool>(), new Dictionary<string, string>());
      var defaultSln = SolutionFixture.GetDefaultSolution(context.FileSystem);
      context.AddDotNetCoreProject(defaultSln.SlnFile.Path.FullPath);

      var helperSetting = SingletonFactory.GetHelperSettings();
      Assert.IsNotNull(helperSetting);

      var projects = helperSetting.DotNetCoreSettings.Projects.ToArray();
      Assert.AreEqual(1, projects.Length);
      Assert.IsTrue(projects.Any(t => t.ProjectAlias == "CakeHelpers"));

      var slnProj = projects.FirstOrDefault();
      Assert.IsNotNull(slnProj);
      Assert.AreEqual("CakeHelpers", slnProj.ProjectAlias);
      slnProj.ProjectAlias = "Test";
      Assert.AreEqual("Test", slnProj.ProjectAlias);
      Assert.IsTrue(string.IsNullOrWhiteSpace(slnProj.Configuration));
      Assert.IsTrue(string.IsNullOrWhiteSpace(slnProj.Framework));
      Assert.IsTrue(string.IsNullOrWhiteSpace(slnProj.Platform));
      Assert.AreEqual("BuildTemp/Test", slnProj.BuildTempDirectory.FullPath);
      Assert.AreEqual(2, slnProj.AllProjects.Count());
      Assert.AreEqual(1, slnProj.SrcProjects.Count());
      Assert.AreEqual(1, slnProj.TestProjects.Count());
    }

    [TestMethod]
    [TestCategory(Global.TestType)]
    public void DotNetCoreAlias_AddProject_Default_NoTestFilters()
    {
      var context = this.GetMoqContext(new Dictionary<string, bool>(), new Dictionary<string, string>());
      var defaultSln = SolutionFixture.GetDefaultSolution(context.FileSystem);
      context.AddDotNetCoreProject(defaultSln.SlnFile.Path.FullPath);

      var helperSetting = SingletonFactory.GetHelperSettings();
      Assert.IsNotNull(helperSetting);

      helperSetting.DotNetCoreSettings.TestSettings.TestProjectNameFilters.Clear();
      var projects = helperSetting.DotNetCoreSettings.Projects.ToArray();
      Assert.AreEqual(1, projects.Length);
      Assert.IsTrue(projects.Any(t => t.ProjectAlias == "CakeHelpers"));

      var slnProj = projects.FirstOrDefault();
      Assert.IsNotNull(slnProj);
      Assert.AreEqual("CakeHelpers", slnProj.ProjectAlias);
      slnProj.ProjectAlias = "Test";
      Assert.AreEqual("Test", slnProj.ProjectAlias);
      Assert.IsTrue(string.IsNullOrWhiteSpace(slnProj.Configuration));
      Assert.IsTrue(string.IsNullOrWhiteSpace(slnProj.Framework));
      Assert.IsTrue(string.IsNullOrWhiteSpace(slnProj.Platform));
      Assert.AreEqual(2, slnProj.AllProjects.Count());
      Assert.AreEqual(2, slnProj.SrcProjects.Count());
      Assert.AreEqual(0, slnProj.TestProjects.Count());
    }

    [TestMethod]
    [TestCategory(Global.TestType)]
    public void DotNetCoreAlias_AddProject_Custom_Success()
    {
      var context = this.GetMoqContext(new Dictionary<string, bool>(), new Dictionary<string, string>());
      var defaultSln = SolutionFixture.GetDefaultSolution(context.FileSystem);
      context.AddDotNetCoreProject(defaultSln.SlnFile.Path.FullPath, proj =>
      {
        proj.Configuration = "Release";
      });

      var helperSetting = SingletonFactory.GetHelperSettings();
      Assert.IsNotNull(helperSetting);

      var projects = helperSetting.DotNetCoreSettings.Projects.ToArray();
      Assert.AreEqual(1, projects.Length);
      Assert.IsTrue(projects.Any(t => t.ProjectAlias == "CakeHelpers"));

      var slnProj = projects.FirstOrDefault();
      Assert.IsNotNull(slnProj);
      Assert.AreEqual("Release", slnProj.Configuration);
      Assert.AreEqual("BuildTemp/CakeHelpers/Release", slnProj.BuildTempDirectory.FullPath);

      slnProj.Framework = "net40";
      Assert.AreEqual("net40", slnProj.Framework);
      Assert.AreEqual("BuildTemp/CakeHelpers/Release/net40", slnProj.BuildTempDirectory.FullPath);

      slnProj.Platform = "x86";
      Assert.AreEqual("x86", slnProj.Platform);
      Assert.AreEqual("BuildTemp/CakeHelpers/Release/net40/x86", slnProj.BuildTempDirectory.FullPath);

      helperSetting.DotNetCoreSettings.BuildSettings.BuildTempFolder = string.Empty;
      Assert.IsNull(slnProj.BuildTempDirectory);

      Assert.AreEqual(2, slnProj.AllProjects.Count());
      Assert.AreEqual(1, slnProj.SrcProjects.Count());
      Assert.AreEqual(1, slnProj.TestProjects.Count());

      var unitConfig = slnProj.AddTestConfig("Unit");
      Assert.IsNotNull(unitConfig);
      Assert.IsTrue(string.IsNullOrWhiteSpace(unitConfig.Logger));
      unitConfig.Logger = "teamcity";
      Assert.IsFalse(string.IsNullOrWhiteSpace(unitConfig.Logger));

      Assert.AreEqual("Unit", unitConfig.TestCategory);

      Assert.AreEqual(TestTypeEnum.Default, unitConfig.TestType);
      unitConfig.TestType = TestTypeEnum.MsTest;
      Assert.AreEqual(TestTypeEnum.MsTest, unitConfig.TestType);

      Assert.IsNotNull(unitConfig.Context);

      var systemTest = "System";
      var testAdded = string.Empty;
      slnProj.TestConfigAdded += (configuration, testConfiguration) =>
      {
        testAdded = testConfiguration.TestCategory;
      };

      slnProj.AddTestConfig(systemTest);
      Assert.AreEqual(systemTest, testAdded);
      Assert.AreEqual(2, slnProj.TestConfigurations.Count());

      var sameProj = context.AddDotNetCoreProject(defaultSln.SlnFile.Path.FullPath);
      Assert.IsNotNull(sameProj);
      Assert.AreEqual(slnProj, sameProj);
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