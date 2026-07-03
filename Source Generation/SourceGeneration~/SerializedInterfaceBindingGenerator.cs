using Microsoft.CodeAnalysis;

namespace Bipolar.InterfaceSerialization.SourceGeneration
{
    public static class SerializedInterfaceBindingSettings
    {
        public const string AttributeFullName = "Bipolar.InterfaceSerialization.BindSerializedInterfaceAttribute";
    }

    [Generator]
    public class SerializedInterfaceBindingGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new BindSyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxReceiver is not BindSyntaxReceiver receiver)
                return;

            foreach (var classSyntax in receiver.CandidateClasses)
            {
                var model = context.Compilation.GetSemanticModel(classSyntax.SyntaxTree);
                if (model.GetDeclaredSymbol(classSyntax) is not INamedTypeSymbol classSymbol)
                    continue;

                if (TryGetAttribute(classSymbol, out var attribute) == false)
                    continue;

                if (attribute.ConstructorArguments[0].Value is not INamedTypeSymbol interfaceSymbol)
                    continue;

                var className = classSymbol.Name;
                var interfaceName = interfaceSymbol.Name;
                var namespaceName = classSymbol.ContainingNamespace?.ToDisplayString();

                var source = SerializedInterfaceClassSourceGenerator.GenerateSource(namespaceName, className, interfaceName, interfaceSymbol, isPartial: true);
                context.AddSource($"{className}.SerializedInterfaceBinding.g.cs", source);
            }
        }

        private static bool TryGetAttribute(INamedTypeSymbol symbol, out AttributeData attribute)
        {
            var attributes = symbol.GetAttributes();
            for (var i = 0; i < attributes.Length; i++)
            {
                if (attributes[i]?.AttributeClass?.ToDisplayString() == SerializedInterfaceBindingSettings.AttributeFullName)
                {
                    attribute = attributes[i];
                    return true;
                }
            }

            attribute = null!;
            return false;
        }
    }
}
