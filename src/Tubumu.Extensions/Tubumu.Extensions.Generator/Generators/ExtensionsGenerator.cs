using System.Diagnostics;
using System.Reflection;
using Microsoft.CodeAnalysis;

namespace Tubumu.Extensions.Generator.Generators;

[Generator(LanguageNames.CSharp)]
public class ExtensionsGenerator : IIncrementalGenerator
{
    private static void WriteSource(SourceProductionContext context,Accessibility accessibility)
    {
        foreach(var type in typeof(ExtensionsGenerator)
                    .Assembly
                    .DefinedTypes
                    .Where(x => x.Namespace?.Contains(nameof(Templates)) is true))
        {
            var text = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static).FirstOrDefault(x => x.Name.Contains("Text"))?.GetValue(null) as string;
            switch(accessibility)
            {
                case Accessibility.Public:
                    ReplaceTo("public");
                    break;
                case Accessibility.Internal:
                    ReplaceTo("internal");
                    break;
            }
            context.AddSource($"{type.FullName}.g.cs", text ?? string.Empty);
            continue;


            void ReplaceTo(string to)
            {
                var from = to is "public" ? "internal" : "public";
                text = text?
                        .Replace(from + " static class", to + " static class")
                        .Replace(from + " class", to        + " class")
                    ;
            }
        }
    }

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var provider = context.SyntaxProvider.ForAttributeWithMetadataName(typeof(TubumuExtensionAttribute).FullName!,
            (s, c) => true,
            (s, c) => s);

        context.RegisterSourceOutput(context.CompilationProvider.Combine(provider.Collect()), (ctx, tuple) =>
        {
            if(tuple.Right.Length != 0)
            {
                var attr = tuple
                    .Right
                    .First()
                    .Attributes
                    .FirstOrDefault(x => x
                        .AttributeClass
                        .HasFullyQualifiedMetadataName(typeof(TubumuExtensionAttribute).FullName))
                    .ToAttribute<TubumuExtensionAttribute>();
                if(attr is not null)
                {
                    if(!attr.Generate) return;
                    WriteSource(ctx, attr.Accessibility);
                    return;
                }
            }
            WriteSource(ctx, Accessibility.Public);
        });
    }
}
