using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Bipolar.InterfaceSerialization.SourceGeneration
{
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
