using FOOD.Core.Diagnostics;
using FOOD.Core.Scoping;
using FOOD.Core.Syntax.Binding;
using FOOD.Core.Syntax.Lex;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOOD.Core.Syntax;
/// <summary>
/// The syntax parser. Split across multiple files for readiblity.
/// </summary>
public partial class Parser : CompilationPart
{
    /// <summary>
    /// The list of parsed tokens.
    /// </summary>
    private readonly Token[] _tokens;

    /// <summary>
    /// The root scope.
    /// </summary>
    public readonly Scope Root;

    /// <summary>
    /// The current head scope.
    /// </summary>
    private Scope _head;

    /// <summary>
    /// The current head scope.
    /// </summary>
    public Scope Head => _head;

    /// <summary>
    /// Creates a subnode of that scope, a fork.
    /// </summary>
    private void StartScope()
    {
        _head.Scopes.Add(new Scope(_head));
        _head = _head.Scopes.Last();
    }

    /// <summary>
    /// Walks up to the parent scope. If <see cref="_head"/> stays the same, then you are at the root.
    /// </summary>
    private void EndScope() => _head = -_head ?? _head;

    /// <summary>
    /// The index.
    /// </summary>
    private long _index = 0;

    private Lexer _lexer;

    /// <summary>
    /// Whether the end of the file has been reached or not.
    /// </summary>
    public bool EndReached => _index >= _tokens.Length || Current.Type == TokenType.EndOfFile;

    /// <summary>
    /// The current token.
    /// </summary>
    private Token Current
    {
        get
        {
            if (_index >= _tokens.Length)
                return new Token(_index, TokenType.EndOfFile, null);
            return _tokens[_index];
        }
    }

    /// <summary>
    /// The token before this one.
    /// </summary>
    private Token Previous
    {
        get
        {
            if (_index - 1 < 0 || _index >= _tokens.Length)
                return new Token(_index, TokenType.EndOfFile, null);
            return _tokens[_index - 1];
        }
    }

    private Binder _binder;

    /// <summary>
    /// Builds a parser.
    /// </summary>
    /// <param name="source">The source code.</param>
    public Parser(CompilationUnit unit, string source) : base(unit)
    {
        _lexer = new Lexer(CompilationUnit, source);
        _binder = new Binder(CompilationUnit, _lexer);
        Root = new Scope(null);
        _head = Root;
        var list = new List<Token>();

        Token token;
        while (true)
        {
            token = _lexer.Fetch();
            if (token.Type == TokenType.EndOfFile)
            {
                list.Add(token);
                break;
            }
            if (token.Type == TokenType.Error)
            {
                CompilationUnit.Report(
                    new ReportedDiagnostic(DiagnosticContext.Diagnostics["_lexUnexpectedChar"],
                    _lexer.GetPosition(token),
                    token.Value ?? '\0'));
                continue;
            }
            list.Add(token);
        }
        _tokens = list.ToArray();
    }
}
