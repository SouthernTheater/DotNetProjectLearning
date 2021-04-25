using Abp.Configuration.Startup;
using Abp.Localization.Dictionaries;
using Abp.Localization.Dictionaries.Xml;
using Abp.Reflection.Extensions;

namespace AspNetCoreRateLimitAbpDemo.Localization
{
    public static class AspNetCoreRateLimitAbpDemoLocalizationConfigurer
    {
        public static void Configure(ILocalizationConfiguration localizationConfiguration)
        {
            localizationConfiguration.Sources.Add(
                new DictionaryBasedLocalizationSource(AspNetCoreRateLimitAbpDemoConsts.LocalizationSourceName,
                    new XmlEmbeddedFileLocalizationDictionaryProvider(
                        typeof(AspNetCoreRateLimitAbpDemoLocalizationConfigurer).GetAssembly(),
                        "AspNetCoreRateLimitAbpDemo.Localization.SourceFiles"
                    )
                )
            );
        }
    }
}
