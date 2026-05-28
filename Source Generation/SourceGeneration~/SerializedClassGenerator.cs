using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bipolar.InterfaceSerialization.SourceGeneration
{
    [Generator]
    public class SerializedClassGenerator : ISourceGenerator
    {
        private const string AttributeFullName = "Bipolar.InterfaceSerialization.GenerateSerializedClassAttribute";

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new InterfacesWithAttributesSyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if ((context.SyntaxReceiver is InterfacesWithAttributesSyntaxReceiver receiver) == false)
                return;

            foreach (var interfaceSyntax in receiver.CandidateInterfaces)
            {
                var model = context.Compilation.GetSemanticModel(interfaceSyntax.SyntaxTree);
                if (model.GetDeclaredSymbol(interfaceSyntax) is not INamedTypeSymbol symbol)
                    continue;

                var hasAttribute = symbol.GetAttributes().Any(a => a.AttributeClass?.ToDisplayString() == AttributeFullName);
                if (hasAttribute == false)
                    continue;

                var interfaceName = symbol.Name;
                var namespaceName = symbol.ContainingNamespace?.ToDisplayString();
                var className = GetClassName(interfaceName);

                var source = GenerateSource(namespaceName, className, interfaceName);
                context.AddSource($"{className}.g.cs", source);
            }
        }

        private static string GetClassName(string interfaceName)
        {
            if (interfaceName.Length > 1
                && interfaceName[0] == 'I'
                && char.IsUpper(interfaceName[1]))
            {
                return interfaceName.Substring(1);
            }

            return "Serialized" + interfaceName;
        }

        private static string GenerateSource(string? namespaceName, string className, string interfaceName)
        {
            var builder = new StringBuilder();
            var hasNamespace = !string.IsNullOrWhiteSpace(namespaceName) && namespaceName != "<global namespace>";
            if (hasNamespace)
            {
                builder.AppendLine($"namespace {namespaceName}");
                builder.AppendLine("{");
            }

            builder.AppendLine("[System.Serializable]");
            builder.AppendLine($"    public class {className} : Bipolar.Serialized<{interfaceName}>, {interfaceName} {{ }}");

            if (hasNamespace)
            {
                builder.AppendLine("}");
            }

            return builder.ToString();
        }
    }

    internal class InterfacesWithAttributesSyntaxReceiver : ISyntaxReceiver
    {
        public List<InterfaceDeclarationSyntax> CandidateInterfaces { get; } = new List<InterfaceDeclarationSyntax>();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is InterfaceDeclarationSyntax interfaceDecl && interfaceDecl.AttributeLists.Count > 0)
            {
                CandidateInterfaces.Add(interfaceDecl);
            }
        }
    }
}
