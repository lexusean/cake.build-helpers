using System;
using System.Diagnostics.CodeAnalysis;
using Cake.Core;

namespace Cake.Helpers.DotNetCore
{
  /// <summary>
  /// Testing Framework Types for DotNetCore
  /// </summary>
  public enum TestTypeEnum
  {
    /// <summary>
    /// Uses VsTest
    /// </summary>
    Default,
    /// <summary>
    /// Uses VsTest
    /// </summary>
    MsTest,
    /// <summary>
    /// Uses VsTest
    /// </summary>
    VsTest,
    /// <summary>
    /// Uses NUnit v3
    /// </summary>
    NUnit,
    /// <summary>
    /// Uses Xunit
    /// </summary>
    XUnit
  }

  /// <summary>
  /// Test Configuration Contract
  /// </summary>
  public interface ITestConfiguration : IHelperContext
  {
    /// <summary>
    /// Test Category to run
    /// </summary>
    string TestCategory { get; set; }
    /// <summary>
    /// Testing Framework
    /// </summary>
    TestTypeEnum TestType { get; set; }
    /// <summary>
    /// Logger name to set on /logger: commandline for testing framework. Defaults to trx
    /// </summary>
    string Logger { get; set; }
  }

  internal class TestConfiguration : ITestConfiguration
  {
    #region Private Fields

    private string _TestCategory = string.Empty;

    #endregion

    #region Ctor

    [ExcludeFromCodeCoverage]
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
      [ExcludeFromCodeCoverage]
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

  /// <summary>
  /// TestConfiguration Extensions
  /// </summary>
  public static class TestConfigurationExtensions
  {
    #region Static Members

    internal static string GetDotNetCoreCategoryString(this ITestConfiguration config)
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