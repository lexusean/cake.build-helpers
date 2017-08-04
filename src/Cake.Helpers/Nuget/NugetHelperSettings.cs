using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Cake.Core;
using Cake.Helpers.Settings;

namespace Cake.Helpers.Nuget
{
  internal class NugetHelperSettings : INugetHelperSettings, ISubSetting
  {
    internal const string DefaultNugetFeedName = "NugetV3";
    internal const string DefaultNugetFeedUrl = "https://api.nuget.org/v3/index.json";

    #region Private Fields

    private readonly List<INugetSource> _NugetSources = new List<INugetSource>();

    #endregion

    #region Ctor

    internal NugetHelperSettings()
    {
      this.AddSource(DefaultNugetFeedName, DefaultNugetFeedUrl);
    }

    #endregion

    #region IHelperContext Members

    public ICakeContext Context { get; set; }

    #endregion

    #region INugetHelperSettings Members

    public IEnumerable<INugetSource> NugetSources
    {
      get { return this._NugetSources; }
    }

    public INugetSource AddSource(string feedUrl)
    {
      return this.AddSource(string.Empty, feedUrl);
    }

    public INugetSource AddSource(string feedName, string feedUrl, Action<INugetSource> sourceConfig = null)
    {
      if (string.IsNullOrWhiteSpace(feedUrl))
        throw new ArgumentNullException(nameof(feedUrl), "Nuget Source URI cannot be empty");

      var existingSource = this._NugetSources.FirstOrDefault(t => t.FeedName == feedName && t.FeedSource == feedUrl);
      if (existingSource == null)
      {
        existingSource = new NugetSource
        {
          FeedName = feedName,
          FeedSource = feedUrl
        };

        this._NugetSources.Add(existingSource);
      }

      sourceConfig?.Invoke(existingSource);

      return existingSource;
    }

    #endregion

    #region ISetting Members

    [ExcludeFromCodeCoverage]
    public void SetupSetting()
    { }

    #endregion

    #region ISubSetting Members

    public bool IsActive
    {
      [ExcludeFromCodeCoverage]
      get;
      [ExcludeFromCodeCoverage]
      internal set;
    }

    #endregion
  }
}