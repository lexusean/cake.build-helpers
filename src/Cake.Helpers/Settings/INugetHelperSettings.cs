using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cake.Helpers.Nuget;

namespace Cake.Helpers.Settings
{
  public interface INugetHelperSettings : IHelperContext
  {
    IEnumerable<INugetSource> NugetSources { get; }
    INugetSource AddSource(string feedUrl);
    INugetSource AddSource(string feedName, string feedUrl, Action<INugetSource> sourceConfig = null);
  }

  public static class NugetHelperSettingsExtensions
  {
    public static IEnumerable<string> GetFeedUrls(this INugetHelperSettings settings)
    {
      if(settings == null)
        throw new ArgumentNullException(nameof(settings));

      return settings.NugetSources
        .Select(t => t.FeedSource)
        .Distinct();
    }
  }
}
