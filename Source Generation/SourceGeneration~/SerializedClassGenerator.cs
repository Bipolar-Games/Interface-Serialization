using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
            if (context.SyntaxReceiver is not InterfacesWithAttributesSyntaxReceiver receiver)
                return;

            foreach (var interfaceSyntax in receiver.CandidateInterfaces)
            {
                var model = context.Compilation.GetSemanticModel(interfaceSyntax.SyntaxTree);
                if (model.GetDeclaredSymbol(interfaceSyntax) is not INamedTypeSymbol symbol)
                    continue;

                bool hasAttribute = symbol.GetAttributes().Any(a => a.AttributeClass?.ToDisplayString() == AttributeFullName);
                if (hasAttribute == false)
                    continue;

                string interfaceName = symbol.Name;
                string? namespaceName = symbol.ContainingNamespace?.ToDisplayString();
                string className = GetClassName(interfaceName);

                var source = GenerateSource(namespaceName, className, interfaceName, symbol);
                context.AddSource($"{className}.g.cs", source);
            }
        }

        private static string GetClassName(string interfaceName)
        {
            return interfaceName.Length > 1 && interfaceName[0] == 'I' && char.IsUpper(interfaceName[1])
                ? interfaceName.Substring(1)
                : "Serialized" + interfaceName;
        }

        private static string GenerateSource(string? namespaceName, string className, string interfaceName, INamedTypeSymbol symbol)
        {
            var textWriter = new StringWriter();
            var codeWriter = new IndentedTextWriter(textWriter);
            bool hasNamespace = !string.IsNullOrWhiteSpace(namespaceName) && namespaceName != "<global namespace>";
            if (hasNamespace)
            {
                codeWriter.WriteLine($"namespace {namespaceName}");
                codeWriter.WriteLine("{");
                codeWriter.Indent++;
            }

            codeWriter.WriteLine("[System.Serializable]");
            codeWriter.WriteLine($"public class {className} : Bipolar.Serialized<{interfaceName}>, {interfaceName}");
            codeWriter.WriteLine("{");
            codeWriter.Indent++;
            WriteMemebers(codeWriter, symbol);
            codeWriter.Indent--;
            codeWriter.WriteLine("}");

            if (hasNamespace)
            {
                codeWriter.Indent--;
                codeWriter.WriteLine("}");
            }

            return textWriter.ToString();
        }

        private static void WriteMemebers(IndentedTextWriter writer, INamedTypeSymbol symbol)
        {
            var members = symbol.GetMembers();
            bool isFirstIteration = true;

            bool hasPropertyNamedValue = members.OfType<IPropertySymbol>().Any(p => p.Name == "Value");
            string valueText = hasPropertyNamedValue ? "base.Value" : "Value";
            foreach (var member in members)
            {
                if (member is IMethodSymbol method && method.MethodKind == MethodKind.Ordinary)
                {
                    WriteOptionalEmptyLine();
                    WriteMethod(method);
                }
                else if (member is IPropertySymbol property)
                {
                    WriteOptionalEmptyLine();
                    WriteProperty(property);
                }
                else if (member is IEventSymbol @event)
                {
                    WriteOptionalEmptyLine();
                    WriteEvent(@event);
                }
            }

            void WriteOptionalEmptyLine()
            {
                if (isFirstIteration == false)
                    writer.WriteLine();
                isFirstIteration = false;
            }

            void WriteMethod(IMethodSymbol method)
            {
                string returnTypeName = method.ReturnType.ToDisplayString();
                string methodName = method.Name;

                var parameters = string.Join(", ", method.Parameters
                    .Select(p => $"{p.Type.ToDisplayString()} {p.Name}"));

                var arguments = string.Join(", ", method.Parameters
                    .Select(p => p.Name));

                writer.WriteLine($"public {returnTypeName} {methodName}({parameters}) => {valueText}.{methodName}({arguments});");
            }

            void WriteProperty(IPropertySymbol property)
            {
                string returnTypeName = property.Type.ToDisplayString();
                string propertyName = property.Name;

                bool hasGet = property.GetMethod != null;
                bool hasSet = property.SetMethod != null;

                if (hasGet && hasSet)
                {
                    writer.Write($"public ");
                    if (propertyName == "Value")
                        writer.Write($"new ");
                    writer.WriteLine($"{returnTypeName} {propertyName}");
                    writer.WriteLine("{");
                    writer.Indent++;
                    writer.WriteLine($"get => {valueText}.{propertyName};");
                    writer.WriteLine($"set => {valueText}.{propertyName} = value;");
                    writer.Indent--;
                    writer.WriteLine("}");
                }
                else if (hasGet)
                {
                    writer.WriteLine($"public {returnTypeName} {propertyName} => {valueText}.{propertyName};");
                }
                else if (hasSet)
                {
                    writer.WriteLine($"public {returnTypeName} {propertyName} {{ set => {valueText}.{propertyName} = value; }}");
                }
            }

            void WriteEvent(IEventSymbol @event)
            {
                var eventTypeName = @event.Type.ToDisplayString();
                var eventName = @event.Name;

                writer.WriteLine($"public event {eventTypeName} {eventName}");
                writer.WriteLine("{");
                writer.Indent++;
                writer.WriteLine($"add => {valueText}.{eventName} += value;");
                writer.WriteLine($"remove => {valueText}.{eventName} -= value;");
                writer.Indent--;
                writer.WriteLine("}");
            }
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
