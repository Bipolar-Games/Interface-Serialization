using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace Bipolar.InterfaceSerialization.SourceGeneration
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class BindSerializedInterfaceAnalyzer : DiagnosticAnalyzer
    {
        #region Diagnostics

        public static readonly DiagnosticDescriptor MustBePartial = new(
            id: "BIS101",
            title: "Class must be partial",
            messageFormat: "Class '{0}' has [BindSerializedInterface] and must be declared as partial",
            category: "SourceGeneration",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public static readonly DiagnosticDescriptor ContainingTypeMustBePartial = new(
            id: "BIS102",
            title: "All containing types must be partial",
            messageFormat: "'{0}' must be declared as partial because it contains a class with [BindSerializedInterface]",
            category: "SourceGeneration",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);
        
        #endregion

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
            MustBePartial, 
            ContainingTypeMustBePartial);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
        }

        private void AnalyzeSymbol(SymbolAnalysisContext context)
        {
            var symbol = (INamedTypeSymbol)context.Symbol;
            if (HasAttribute(symbol) == false)
                return;

            var isPartial = symbol.DeclaringSyntaxReferences
                .Select(r => r.GetSyntax())
                .OfType<ClassDeclarationSyntax>()
                .All(c => c.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword)));

            if (isPartial == false)
            {
                var diagnostic = Diagnostic.Create(
                    MustBePartial,
                    symbol.Locations[0],
                    symbol.Name);
                context.ReportDiagnostic(diagnostic);
            }

            
            for (var containingType = symbol.ContainingType; containingType != null; containingType = containingType.ContainingType)
            {
                var containingIsPartial = containingType.DeclaringSyntaxReferences
                    .Select(r => r.GetSyntax())
                    .OfType<TypeDeclarationSyntax>()
                    .All(t => t.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword)));

                if (containingIsPartial == false)
                {
                    var diagnostic = Diagnostic.Create(
                        ContainingTypeMustBePartial,
                        containingType.Locations[0],
                        containingType.Name);
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }

        private static bool HasAttribute(INamedTypeSymbol symbol) => symbol
            .GetAttributes()
            .Any(attr => attr.AttributeClass?.ToDisplayString() == SerializedInterfaceBindingSettings.AttributeFullName);
    }
}
