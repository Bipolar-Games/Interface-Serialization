using Microsoft.CodeAnalysis;
using System.Linq;

namespace Bipolar.InterfaceSerialization.SourceGeneration
{
    [Generator]
    public class SerializedInterfaceClassGenerator : ISourceGenerator
    {
        private const string AttributeFullName = "Bipolar.InterfaceSerialization.GenerateSerializedClassAttribute";
        private const string CustomClassNamePropertyName = "CustomClassName";

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new InterfacesWithAttributesSyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxReceiver is not InterfacesWithAttributesSyntaxReceiver receiver)
                return;

            foreach (var interfaceSyntax in receiver.CandidateInterfaces)
            {
                var model = context.Compilation.GetSemanticModel(interfaceSyntax.SyntaxTree);
                if (model.GetDeclaredSymbol(interfaceSyntax) is not INamedTypeSymbol symbol)
                    continue;

                var attribute = symbol.GetAttributes().FirstOrDefault(a => a.AttributeClass?.ToDisplayString() == AttributeFullName);
                if (attribute == null)
                    continue;

                string interfaceName = symbol.Name;

                var customNameProperty = attribute.NamedArguments.FirstOrDefault(kvp => kvp.Key == CustomClassNamePropertyName).Value;
                string className = customNameProperty.Value is string customName
                    ? customName
                    : GetClassName(interfaceName);

                string? namespaceName = symbol.ContainingNamespace?.ToDisplayString();
                var source = SerializedInterfaceClassSourceGenerator.GenerateSource(namespaceName, className, interfaceName, symbol);
                context.AddSource($"{className}.g.cs", source);
            }
        }

        private static string GetClassName(string interfaceName)
        {
            return interfaceName.Length > 1 && interfaceName[0] == 'I' && char.IsUpper(interfaceName[1])
                ? interfaceName.Substring(1)
                : "Serialized" + interfaceName;
        }
    }
}
