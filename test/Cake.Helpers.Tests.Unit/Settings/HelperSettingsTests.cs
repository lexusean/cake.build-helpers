using System;
using System.Collections.Generic;
using System.Linq;
using Cake.Common.IO;
using Cake.Core;
using Cake.Core.IO;
using Cake.Helpers.DotNetCore;
using Cake.Helpers.Nuget;
using Cake.Helpers.Settings;
using Cake.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Cake.Helpers.Tests.Unit.Settings
{
  [TestClass]
  public class HelperSettingsTests
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
    public void HelperSettings_Setup_Success()
    { 
      var context = this.GetMoqContext(new Dictionary<string, bool>(), new Dictionary<string, string>());
      SingletonFactory.Context = context;

      var dotNetCoreHelper = SingletonFactory.GetDotNetCoreHelper();
      Assert.IsNotNull(dotNetCoreHelper);
      var coreHelperType = dotNetCoreHelper.GetType();
      Assert.IsTrue(SingletonFactory.ExistsInCache(coreHelperType));

      SingletonFactory.ClearFactory();
      SingletonFactory.Context = context;
      Assert.IsFalse(SingletonFactory.ExistsInCache(coreHelperType));

      var helperSetting = new HelperSettings();
      ((DotNetCoreHelperSettings)helperSetting.DotNetCoreSettings).IsActive = true;

      helperSetting.SetupSetting();
      Assert.IsTrue(SingletonFactory.ExistsInCache(coreHelperType));

      var newDotNetCoreHelper = SingletonFactory.GetDotNetCoreHelper();

      Assert.IsNotNull(newDotNetCoreHelper);
      Assert.AreNotEqual(dotNetCoreHelper, newDotNetCoreHelper);
    }

    [TestMethod]
    [TestCategory(Global.TestType)]
    public void HelperSettings_Setup_NoActive()
    {
      var context = this.GetMoqContext(new Dictionary<string, bool>(), new Dictionary<string, string>());
      SingletonFactory.Context = context;

      var dotNetCoreHelper = SingletonFactory.GetDotNetCoreHelper();
      Assert.IsNotNull(dotNetCoreHelper);
      var coreHelperType = dotNetCoreHelper.GetType();
      Assert.IsTrue(SingletonFactory.ExistsInCache(coreHelperType));

      SingletonFactory.ClearFactory();
      SingletonFactory.Context = context;
      Assert.IsFalse(SingletonFactory.ExistsInCache(coreHelperType));

      var helperSetting = new HelperSettings();
      ((DotNetCoreHelperSettings)helperSetting.DotNetCoreSettings).IsActive = false;

      helperSetting.SetupSetting();
      Assert.IsFalse(SingletonFactory.ExistsInCache(coreHelperType));
    }

    [TestMethod]
    [TestCategory(Global.TestType)]
    public void HelperSettings_SetRunTarget()
    {
      var context = this.GetMoqContext(new Dictionary<string, bool>(), new Dictionary<string, string>());
      SingletonFactory.Context = context;

      bool runTargetRan = false;
      var helperSetting = new HelperSettings();
      helperSetting.RunTargetFunc = target =>
      {
        runTargetRan = true;
        return new CakeReport();
      };

      Assert.IsNotNull(helperSetting.RunTargetFunc);
      Assert.IsFalse(runTargetRan);

      helperSetting.RunTargetFunc(string.Empty);
      Assert.IsTrue(runTargetRan);
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

    #endregion
  }
}