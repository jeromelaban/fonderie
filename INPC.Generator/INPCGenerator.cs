using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Linq;
using System.Text;
using Uno.RoslynHelpers;

namespace INPC.Generator
{
    [Generator]
    public class MySourceGenerator : ISourceGenerator
    {
        public void Initialize(InitializationContext context)
        {
            // No initialization required for this one
        }

        public void Execute(SourceGeneratorContext context)
        {
            // Search for the GeneratedPropertyAttribute symbol
            var _generatedPropertyAttributeSymbol =
                context.Compilation.GetTypeByMetadataName("INPC.GeneratedPropertyAttribute");

            if (_generatedPropertyAttributeSymbol != null)
            {
                // Search in all types defined in the current compilation (not in the dependents)
                var query = from typeSymbol in context.Compilation.SourceModule.GlobalNamespace.GetNamespaceTypes()
                            from property in typeSymbol.GetFields()

                                // Find the attribute on the field
                            let info = property.FindAttributeFlattened(_generatedPropertyAttributeSymbol)
                            where info != null

                            // Group properties by type
                            group property by typeSymbol into g
                            select g;

                foreach (var type in query)
                {
                    // Let's generate the needed class
                    var builder = new IndentedStringBuilder();

                    builder.AppendLineInvariant("using System;");
                    builder.AppendLineInvariant("using System.ComponentModel;");

                    using (builder.BlockInvariant($"namespace {type.Key.ContainingNamespace}"))
                    {
                        using (builder.BlockInvariant($"partial class {type.Key.Name} : INotifyPropertyChanged"))
                        {
                            builder.AppendLineInvariant($"public event PropertyChangedEventHandler PropertyChanged;");

                            foreach (var fieldInfo in type)
                            {
                                var propertyName = fieldInfo.Name.TrimStart('_');

                                // Uppercase name for camel case
                                propertyName = propertyName[0].ToString().ToUpperInvariant() + propertyName.Substring(1);

                                using (builder.BlockInvariant($"public {fieldInfo.Type} {propertyName}"))
                                {
                                    builder.AppendLineInvariant($"get => {fieldInfo.Name};");

                                    using (builder.BlockInvariant($"set"))
                                    {
                                        builder.AppendLineInvariant($"var previous = {fieldInfo.Name};");
                                        builder.AppendLineInvariant($"{fieldInfo.Name} = value;");
                                        builder.AppendLineInvariant($"PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof({propertyName})));");
                                        builder.AppendLineInvariant($"On{propertyName}Changed(previous, value);");
                                    }
                                }

                                builder.AppendLineInvariant($"partial void On{propertyName}Changed({fieldInfo.Type} previous, {fieldInfo.Type} value);");
                            }
                        }
                    }

                    var sanitizedName = type.Key.ToDisplayString().Replace(".", "_").Replace("+", "_");
                    context.AddSource(sanitizedName, SourceText.From(builder.ToString(), Encoding.UTF8));
                }
            }
        }
    }
}
