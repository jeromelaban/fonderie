using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml;
using Uno.RoslynHelpers;

namespace INPC.Generator
{
	[Generator]
    public class ReswConstantsGenerator : ISourceGenerator
    {
        public void Initialize(InitializationContext context)
        {
            // Debugger.Launch();
            // No initialization required for this one
        }

        public void Execute(SourceGeneratorContext context)
        {
            var resources = context.GetMSBuildItems("PRIResource");

            if (resources.Any())
            {
                var sb = new IndentedStringBuilder();

                using (sb.BlockInvariant($"namespace {context.GetMSBuildProperty("RootNamespace")}"))
                {
                    using (sb.BlockInvariant($"internal enum PriResources"))
                    {
                        foreach (var item in resources)
                        {
                            //load document
                            XmlDocument doc = new XmlDocument();
                            doc.Load(item);

                            //extract all localization keys from Win10 resource file
                            var nodes = doc.SelectNodes("//data")
                                .Cast<XmlElement>()
                                .Select(node => node.GetAttribute("name"))
                                .ToArray();

                            foreach (var node in nodes)
                            {
                                sb.AppendLineInvariant($"{node},");
                            }
                        }
                    }
                }

                context.AddSource("PriResources", SourceText.From(sb.ToString(), Encoding.UTF8));
            }
        }
    }
}
