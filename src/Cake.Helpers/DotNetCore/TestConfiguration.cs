using System;
using Cake.Core;

namespace Cake.Helpers.DotNetCore
{
  public enum TestTypeEnum
  {
    Default,
    MsTest,
    VsTest,
    NUnit,
    XUnit
  }

  public interface ITestConfiguration : IHelperContext
  {
    string TestCategory { get; set; }
    TestTypeEnum TestType { get; set; }
    string Logger { get; set; }
  }

  public class TestConfiguration : ITestConfiguration
  {
    #region Private Fields

    private string _TestCategory = string.Empty;

    #endregion

    #region Ctor

    internal TestConfiguration(ICakeContext context, string testCategory)
    {
      if (context == null)
        throw new ArgumentNullException(nameof(context));

      this.Context = context;
      this.TestCategory = testCategory;
    }

    #endregion

    #region IHelperContext Members

    public ICakeContext Context { get; set; }

    #endregion

    #region ITestConfiguration Members

    public string Logger { get; set; }

    public string TestCategory
    {
      get { return this._TestCategory; }
      set
      {
        if (string.IsNullOrWhiteSpace(value))
          throw new ArgumentNullException(nameof(this.TestCategory));

        this._TestCategory = value;
      }
    }

    public TestTypeEnum TestType { get; set; }

    #endregion
  }

  public static class TestConfigurationExtensions
  {
    #region Static Members

    public static string GetDotNetCoreCategoryString(this ITestConfiguration config)
    {
      if (config == null)
        throw new ArgumentNullException(nameof(config));

      switch (config.TestType)
      {
        case TestTypeEnum.XUnit:
          return $"Category={config.TestCategory}";
        default:
          return $"TestCategory={config.TestCategory}";
      }
    }

    #endregion
  }
}