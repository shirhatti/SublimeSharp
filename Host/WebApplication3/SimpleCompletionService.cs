using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Recommendations;
using System.Text;

namespace WebApplication3
{
    /// <summary>
    /// Summary description for SimpleCompletionService
    /// </summary>
    public class SimpleCompletionService
    {
        public SimpleCompletionService()
        {
            //
            // TODO: Add constructor logic here
            //
        }


        public static List<List<string>> GetCompletions(int position, string documentText)
        {
            int returns = 20;
            documentText = documentText.Replace("\\\\", "\\");
            var workspace = new CustomWorkspace();
            ProjectId proj = workspace.AddProject("Program", LanguageNames.CSharp);
            Console.WriteLine(documentText);
            DocumentId doc = workspace.AddDocument(proj, "Program.cs", documentText);

            
            SyntaxTree tree = CSharpSyntaxTree.ParseText(documentText);

            var root = (CompilationUnitSyntax)tree.GetRoot();

            CSharpCompilation compilation = CSharpCompilation.Create("HelloWorld")
                .AddReferences(
                    new MetadataFileReference(
                        typeof(object).Assembly.Location))
                .AddSyntaxTrees(tree);

            SemanticModel model = compilation.GetSemanticModel(tree);


            var rec = Recommender
                .GetRecommendedSymbolsAtPosition(model, position, workspace)
                .Take(returns)
                .Select(s =>
                {
                    var str = s.ToDisplayString();
                    var methodSymbol = s as IMethodSymbol;
                    var snippet=s.Name;
                    if (methodSymbol != null)
                    {
                        var snippetBuilder = new StringBuilder(s.Name);
                        snippetBuilder.Append("(");
                        var paramArity = methodSymbol.Parameters.Length;
                        for(int i = 0; i<paramArity; i++){
                            var paramName = methodSymbol.Parameters[i].Name;
                            snippetBuilder.AppendFormat("${{{0}:{1}}}",i+1,paramName);
                            if (i + 1 < paramArity)
                            {
                                snippetBuilder.Append(",");
                            }
                        }
                        snippetBuilder.Append(")$0");
                        snippet = snippetBuilder.ToString();
                    }
                    return new List<string> { str, snippet };
                });

            return rec.ToList(); 
        }
    }
}