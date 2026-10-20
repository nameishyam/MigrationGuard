
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MigrationGuard.Cli;

if (args.Length != 1)
{
    Console.WriteLine("Usage: MigrationGuard.Cli <Migration.cs>");
    return;
}

var migrationPath = Path.GetFullPath(args[0]);

if (!File.Exists(migrationPath))
{
    Console.WriteLine($"Migration not found: {migrationPath}");
    Environment.ExitCode = 2;
    return;
}

var root = CSharpSyntaxTree
    .ParseText(File.ReadAllText(migrationPath))
    .GetRoot();

var migrationDirectory = Path.GetDirectoryName(migrationPath)!;

foreach (var methodName in new[] { "Up", "Down" })
{
    var method = root.DescendantNodes()
        .OfType<MethodDeclarationSyntax>()
        .FirstOrDefault(m => m.Identifier.Text == methodName);

    if (method is null)
    {
        Console.WriteLine($"Method not found: {methodName}");
        continue;
    }

    Console.WriteLine($"\n{methodName} migration:");

    var scripts = method.DescendantNodes()
        .OfType<InvocationExpressionSyntax>()
        .Where(invocation =>
            invocation.Expression is MemberAccessExpressionSyntax member &&
            member.Expression.ToString() == "Execute" &&
            member.Name.Identifier.Text == "Script");

    foreach (var script in scripts)
    {
        var argument = script.ArgumentList.Arguments.FirstOrDefault();

        if (argument?.Expression is not LiteralExpressionSyntax literal ||
            !literal.IsKind(SyntaxKind.StringLiteralExpression))
        {
            Console.WriteLine("  Dynamic SQL path: manual review required.");
            continue;
        }

        var sqlPath = Path.GetFullPath(
            Path.Combine(migrationDirectory, literal.Token.ValueText));

        if (!File.Exists(sqlPath))
        {
            Console.WriteLine($"  SQL file not found: {sqlPath}");
            Environment.ExitCode = 2;
            continue;
        }

        Console.WriteLine($"  SQL file: {sqlPath}");

        MigrationAnalyzer.Analyze(sqlPath);
    }
}
