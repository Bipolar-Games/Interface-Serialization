using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Bipolar.InterfaceSerialization.SourceGeneration
{
    internal class BindSyntaxReceiver : ISyntaxReceiver
    {
        public List<ClassDeclarationSyntax> CandidateClasses { get; } = new();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is ClassDeclarationSyntax classDecl
                && classDecl.AttributeLists.Count > 0)
            {
                CandidateClasses.Add(classDecl);
            }
        }
    }
}
