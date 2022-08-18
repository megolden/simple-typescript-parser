namespace TypeScriptAST;

public enum TokenType
{
    Unknown,                            // illegal character
    End,                                // end of script
    Identifier,                         // x, y, z, ...
    OpenParen,                          // (
    CloseParen,                         // )
    OpenBracket,                        // [
    CloseBracket,                       // ]
    OpenBrace,                          // {
    CloseBrace,                         // }
    Plus,                               // +
    DoublePlus,                         // ++
    PlusEqual,                          // +=
    Minus,                              // -
    DoubleMinus,                        // --
    MinusEqual,                         // -=
    Asterisk,                           // *
    AsteriskEqual,                      // *=
    Slash,                              // /
    SlashEqual,                         // /=
    Percent,                            // %
    PercentEqual,                       // %=
    Dot,                                // .
    Question,                           // ?
    DoubleQuestion,                     // ??
    QuestionDot,                        // ?.
    NumericLiteral,                     // 1, 2, 3, ...
    StringLiteral,                      // "..."
    TemplateLiteral,                    // `......`
    TemplateHead,                       // `...${
    TemplateMiddle,                     // }...${
    TemplateTail,                       // }...`
    RegExpLiteral,                      // /.../d g i m s u y
    NullLiteral,                        // null
    BooleanLiteral,                     // true or false
    Semicolon,                          // ;
    Colon,                              // :
    Comment,                            // //... or /*...*/
    Exclamation,                        // !
    ExclamationEqual,                   // !=
    ExclamationDoubleEqual,             // !==
    Ampersand,                          // &
    DoubleAmpersand,                    // &&
    Comma,                              // ,
    Equal,                              // =
    DoubleEqual,                        // ==
    TripleEqual,                        // ===
    EqualGreaterThan,                   // =>
    LessThan,                           // <
    LessThanEqual,                      // <=
    GreaterThan,                        // >
    GreaterThanEqual,                   // >=
    TripleGreaterThan,                  // >>>
    Bar,                                // |
    DoubleBar,                          // ||
    DoubleGreaterThan,                  // >>
    DoubleLessThan,                     // <<
    Tilde,                              // ~
    Carat,                              // ^
    DoubleAsterisk,                     // **
    DoubleAsteriskEqual,                // **=
    DoubleLessThanEqual,                // <<=
    DoubleGreaterThanEqual,             // >>=
    TripleGreaterThanEqual,             // >>>=
    AmpersandEqual,                     // &=
    CaratEqual,                         // ^=
    BarEqual,                           // |=
    DoubleAmpersandEqual,               // &&=
    DoubleBarEqual,                     // ||=
    DoubleQuestionEqual,                // ??=
    TripleDot                           // ...
}
