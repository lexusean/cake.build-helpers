using Cake.Core;

namespace Cake.Helpers
{
  /// <summary>
  ///   Helper Contract
  /// </summary>
  public interface IHelper
  { }

  /// <summary>
  ///   Helper With Context Contract
  /// </summary>
  /// <inheritdoc />
  public interface IHelperContext : IHelper
  {
    /// <summary>
    ///   Cake Context
    /// </summary>
    ICakeContext Context { get; set; }
  }
}