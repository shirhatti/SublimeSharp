using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Recommendations;
using System.Text;
using SublimeSharp.Host.Model;

namespace SublimeSharp.Host.CodeAnalysis
{
    public class SimpleCompletionService
    {
        public static List<Suggestion> GetCompletions(SimpleFileCompletionMessage message)
        {
            int returns = 20;
            var documentText = message.DocumentText.Replace("\\\\", "\\");
            var workspace = new CustomWorkspace();

            var proj = workspace.AddProject("Program", LanguageNames.CSharp);
            var doc = workspace.AddDocument(proj, "Program.cs", documentText);
            
            var tree = CSharpSyntaxTree.ParseText(documentText);

            var root = (CompilationUnitSyntax)tree.GetRoot();

            CSharpCompilation compilation = CSharpCompilation.Create("HelloWorld")
                .AddReferences(
                    new MetadataFileReference(
                        typeof(object).Assembly.Location))
                .AddSyntaxTrees(tree);

            SemanticModel model = compilation.GetSemanticModel(tree);


            var rec = Recommender
                .GetRecommendedSymbolsAtPosition(model, message.SuggestionPosition, workspace)
                .Take(returns)
                .Select(s =>
                {
                    var methodSymbol = s as IMethodSymbol;
                    var snippet=s.Name;
                    if (methodSymbol != null)
                    {
                        var snippetBuilder = new StringBuilder(s.Name);
                        snippetBuilder.Append("(");
                        var paramArity = methodSymbol.Parameters.Length;
                        for(var i = 0; i<  paramArity; i++){
                            var paramName = methodSymbol.Parameters[i].Name;
                            snippetBuilder.AppendFormat("${{{0}:{1}}}", i+1, paramName); // add named tab field
                            if (i + 1 < paramArity)
                            {
                                snippetBuilder.Append(",");
                            }
                        }
                        snippetBuilder.Append(")$0"); // add end tab escape
                        snippet = snippetBuilder.ToString();
                    }
                    return new Suggestion
                    {
                        DisplayName = s.ToDisplayString(),
                        Snippet = snippet,
                    };
                });

            return rec.ToList(); 
        }
    }
}