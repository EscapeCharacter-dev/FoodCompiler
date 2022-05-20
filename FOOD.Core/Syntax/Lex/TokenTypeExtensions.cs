namespace FOOD.Core.Syntax.Lex;

public static class TokenTypeExtensions
{
    public static bool IsPartOf(this TokenType concernedType, params TokenType[] types)
        => types.Contains(concernedType);
}
