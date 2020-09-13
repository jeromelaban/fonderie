using Microsoft.CodeAnalysis;
using System.Linq;

namespace INPC.Generator
{
	internal static class SourceGeneratorContextExtensions
    {
        private const string SourceItemGroupMetadata = "build_metadata.AdditionalFiles.SourceItemGroup";

		public static string GetMSBuildProperty(
            this SourceGeneratorContext context,
			string name,
			string defaultValue = "")
		{
            context.AnalyzerConfigOptions.GlobalOptions.TryGetValue($"build_property.{name}", out var value);
            return value ?? defaultValue;
		}

		public static string[] GetMSBuildItems(this SourceGeneratorContext context, string name)
            => context
                .AdditionalFiles
                .Where(f => context.AnalyzerConfigOptions
                    .GetOptions(f)
                    .TryGetValue(SourceItemGroupMetadata, out var sourceItemGroup)
                    && sourceItemGroup == name)
                .Select(f => f.Path)
                .ToArray();
    }
}
