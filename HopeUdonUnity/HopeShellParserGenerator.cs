
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace HopeTools
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]

    public class HopeShellParserGenerator : UdonSharpBehaviour
    {
        // Token type constants for Udon compatibility
        public const int TOKEN_IDENTIFIER = 0;     // Variable names, command names
        public const int TOKEN_STRING = 1;         // Quoted strings
        public const int TOKEN_NUMBER = 2;         // Numeric values
        
        // Operators
        public const int TOKEN_ASSIGN = 10;         // =
        public const int TOKEN_PIPE = 11;           // |
        public const int TOKEN_REDIRECT_OUT = 12;   // >
        public const int TOKEN_REDIRECT_IN = 13;    // <
        public const int TOKEN_APPEND = 14;         // >>
        public const int TOKEN_SEMICOLON = 15;      // ;
        public const int TOKEN_AND = 16;            // &&
        public const int TOKEN_OR = 17;             // ||
        
        // Arithmetic operators
        public const int TOKEN_PLUS = 60;           // +
        public const int TOKEN_MINUS = 61;          // -
        public const int TOKEN_MULTIPLY = 62;       // *
        public const int TOKEN_DIVIDE = 63;         // /
        public const int TOKEN_MODULO = 64;          // %
        public const int TOKEN_POWER = 65;          // **
        
        // Comparison operators
        public const int TOKEN_EQUAL = 70;          // ==
        public const int TOKEN_NOT_EQUAL = 71;      // !=
        public const int TOKEN_LESS_EQUAL = 72;     // <=
        public const int TOKEN_GREATER_EQUAL = 73;  // >=
        public const int TOKEN_LESS = 74;           // < (alias)
        public const int TOKEN_GREATER = 75;        // > (alias)
        
        // Logical operators
        public const int TOKEN_NOT = 80;            // !
        public const int TOKEN_BITWISE_AND = 81;    // &
        public const int TOKEN_BITWISE_OR = 82;     // |
        public const int TOKEN_BITWISE_XOR = 83;    // ^
        public const int TOKEN_BITWISE_NOT = 84;    // ~
        
        // Increment/Decrement
        public const int TOKEN_INCREMENT = 90;      // ++
        public const int TOKEN_DECREMENT = 91;      // --
        
        // Compound assignment
        public const int TOKEN_PLUS_ASSIGN = 92;    // +=
        public const int TOKEN_MINUS_ASSIGN = 93;   // -=
        public const int TOKEN_MUL_ASSIGN = 94;     // *=
        public const int TOKEN_DIV_ASSIGN = 95;     // /=
        public const int TOKEN_MOD_ASSIGN = 96;     // %=
        
        // Special characters
        public const int TOKEN_DOLLAR = 20;         // $
        public const int TOKEN_DOT = 21;            // .
        public const int TOKEN_SLASH = 22;          // /
        public const int TOKEN_BACKSLASH = 23;      // \
        public const int TOKEN_QUOTE = 24;          // "
        public const int TOKEN_SINGLE_QUOTE = 25;   // '
        public const int TOKEN_BACKTICK = 26;       // `
        
        // Grouping
        public const int TOKEN_LPAREN = 30;         // (
        public const int TOKEN_RPAREN = 31;         // )
        public const int TOKEN_LBRACE = 32;         // {
        public const int TOKEN_RBRACE = 33;         // }
        public const int TOKEN_LBRACKET = 34;       // [
        public const int TOKEN_RBRACKET = 35;       // ]
        
        // Whitespace and control
        public const int TOKEN_WHITESPACE = 40;     // Space, tab
        public const int TOKEN_NEWLINE = 41;        // \n
        
        // Special tokens
        public const int TOKEN_VARIABLE = 50;       // $variable
        public const int TOKEN_MEMBER_ACCESS = 51;  // .member
        public const int TOKEN_COMMAND = 52;        // Built-in commands
        public const int TOKEN_COMMENT = 53;        // # comment
        
        // End of input
        public const int TOKEN_EOF = 100;
        public const int TOKEN_UNKNOWN = 999;

        // Token data storage using parallel arrays for Udon compatibility
        private int[] tokenTypes;
        private string[] tokenValues;
        private int[] tokenPositions;
        private int[] tokenLines;
        private int[] tokenColumns;
        
        // Helper method to get token type name for debugging
        private string GetTokenTypeName(int tokenType)
        {
            switch (tokenType)
            {
                // Basic tokens
                case TOKEN_IDENTIFIER: return "IDENTIFIER";
                case TOKEN_STRING: return "STRING";
                case TOKEN_NUMBER: return "NUMBER";
                
                // Arithmetic operators
                case TOKEN_PLUS: return "PLUS";
                case TOKEN_MINUS: return "MINUS";
                case TOKEN_MULTIPLY: return "MULTIPLY";
                case TOKEN_DIVIDE: return "DIVIDE";
                case TOKEN_MODULO: return "MODULO";
                case TOKEN_POWER: return "POWER";
                
                // Comparison operators
                case TOKEN_EQUAL: return "EQUAL";
                case TOKEN_NOT_EQUAL: return "NOT_EQUAL";
                case TOKEN_LESS_EQUAL: return "LESS_EQUAL";
                case TOKEN_GREATER_EQUAL: return "GREATER_EQUAL";
                case TOKEN_LESS: return "LESS";
                case TOKEN_GREATER: return "GREATER";
                
                // Logical operators
                case TOKEN_NOT: return "NOT";
                case TOKEN_AND: return "AND";
                case TOKEN_OR: return "OR";
                case TOKEN_BITWISE_AND: return "BITWISE_AND";
                case TOKEN_BITWISE_OR: return "BITWISE_OR";
                case TOKEN_BITWISE_XOR: return "BITWISE_XOR";
                case TOKEN_BITWISE_NOT: return "BITWISE_NOT";
                
                // Increment/Decrement
                case TOKEN_INCREMENT: return "INCREMENT";
                case TOKEN_DECREMENT: return "DECREMENT";
                
                // Compound assignment
                case TOKEN_PLUS_ASSIGN: return "PLUS_ASSIGN";
                case TOKEN_MINUS_ASSIGN: return "MINUS_ASSIGN";
                case TOKEN_MUL_ASSIGN: return "MUL_ASSIGN";
                case TOKEN_DIV_ASSIGN: return "DIV_ASSIGN";
                case TOKEN_MOD_ASSIGN: return "MOD_ASSIGN";
                
                // Shell operators
                case TOKEN_ASSIGN: return "ASSIGN";
                case TOKEN_PIPE: return "PIPE";
                case TOKEN_REDIRECT_OUT: return "REDIRECT_OUT";
                case TOKEN_REDIRECT_IN: return "REDIRECT_IN";
                case TOKEN_APPEND: return "APPEND";
                case TOKEN_SEMICOLON: return "SEMICOLON";
                
                // Special characters
                case TOKEN_DOLLAR: return "DOLLAR";
                case TOKEN_DOT: return "DOT";
                case TOKEN_SLASH: return "SLASH";
                case TOKEN_BACKSLASH: return "BACKSLASH";
                case TOKEN_QUOTE: return "QUOTE";
                case TOKEN_SINGLE_QUOTE: return "SINGLE_QUOTE";
                case TOKEN_BACKTICK: return "BACKTICK";
                
                // Grouping
                case TOKEN_LPAREN: return "LPAREN";
                case TOKEN_RPAREN: return "RPAREN";
                case TOKEN_LBRACE: return "LBRACE";
                case TOKEN_RBRACE: return "RBRACE";
                case TOKEN_LBRACKET: return "LBRACKET";
                case TOKEN_RBRACKET: return "RBRACKET";
                
                // Whitespace and control
                case TOKEN_WHITESPACE: return "WHITESPACE";
                case TOKEN_NEWLINE: return "NEWLINE";
                
                // Special tokens
                case TOKEN_VARIABLE: return "VARIABLE";
                case TOKEN_MEMBER_ACCESS: return "MEMBER_ACCESS";
                case TOKEN_COMMAND: return "COMMAND";
                case TOKEN_COMMENT: return "COMMENT";
                
                // End of input
                case TOKEN_EOF: return "EOF";
                default: return "UNKNOWN";
            }
        }
        
        // Token storage using parallel arrays
        private int tokenCount;
        private int currentTokenIndex;
        
        // Input processing
        private string input;
        private int position;
        private int line;
        private int column;
        
        // Configuration
        [SerializeField] private int maxTokens = 1000;
        
        // Keywords and commands
        private string[] keywords = new string[]
        {
            "help", "clear", "cls", "history", "time", "ls", "cd", 
            "addroot", "pwd", "exit", "quit", "echo", "set", "unset"
        };
        
        void Start()
        {
            tokenTypes = new int[maxTokens];
            tokenValues = new string[maxTokens];
            tokenPositions = new int[maxTokens];
            tokenLines = new int[maxTokens];
            tokenColumns = new int[maxTokens];
            tokenCount = 0;
            currentTokenIndex = 0;

            // Test comprehensive operator support
            string testInput = ".tf.sfa.tsadf + $sfda + x = 65";
            Lex(testInput);
            PrintTokens();
        }
        
        /// <summary>
        /// Main lexical analysis method - tokenizes the input string
        /// </summary>
        public int Lex(string input)
        {
            this.input = input;
            position = 0;
            line = 1;
            column = 1;
            tokenCount = 0;
            currentTokenIndex = 0;
            
            while (position < input.Length)
            {
                int tokenType = TOKEN_UNKNOWN;
                string tokenValue = "";
                int tokenPos = position;
                int tokenLine = line;
                int tokenCol = column;
                
                NextToken(ref tokenType, ref tokenValue, ref tokenPos, ref tokenLine, ref tokenCol);
                
                if (tokenType != TOKEN_WHITESPACE && tokenType != TOKEN_NEWLINE)
                {
                    if (tokenCount < maxTokens)
                    {
                        tokenTypes[tokenCount] = tokenType;
                        tokenValues[tokenCount] = tokenValue;
                        tokenPositions[tokenCount] = tokenPos;
                        tokenLines[tokenCount] = tokenLine;
                        tokenColumns[tokenCount] = tokenCol;
                        tokenCount++;
                    }
                }
                
                if (tokenType == TOKEN_EOF)
                    break;
            }
            
            // Add EOF token
            if (tokenCount < maxTokens)
            {
                tokenTypes[tokenCount] = TOKEN_EOF;
                tokenValues[tokenCount] = "";
                tokenPositions[tokenCount] = position;
                tokenLines[tokenCount] = line;
                tokenColumns[tokenCount] = column;
                tokenCount++;
            }
            
            return tokenCount;
        }
        
        /// <summary>
        /// Get the next token from input
        /// </summary>
        private void NextToken(ref int tokenType, ref string tokenValue, ref int tokenPos, ref int tokenLine, ref int tokenCol)
        {
            tokenPos = position;
            tokenLine = line;
            tokenCol = column;
            
            if (position >= input.Length)
            {
                tokenType = TOKEN_EOF;
                tokenValue = "";
                return;
            }
            
            char currentChar = input[position];
            
            // Handle different character types
            if (char.IsWhiteSpace(currentChar))
            {
                HandleWhitespace(ref tokenType, ref tokenValue);
            }
            else if (currentChar == '\n')
            {
                position++;
                line++;
                column = 1;
                tokenType = TOKEN_NEWLINE;
                tokenValue = "\\n";
            }
            else if (currentChar == '#')
            {
                HandleComment(ref tokenType, ref tokenValue);
            }
            else if (currentChar == '"')
            {
                HandleString('"', ref tokenType, ref tokenValue);
            }
            else if (currentChar == '\'')
            {
                HandleString('\'', ref tokenType, ref tokenValue);
            }
            else if (currentChar == '$')
            {
                HandleVariable(ref tokenType, ref tokenValue);
            }
            else if (char.IsDigit(currentChar))
            {
                HandleNumber(ref tokenType, ref tokenValue);
            }
            else if (char.IsLetter(currentChar) || currentChar == '_')
            {
                HandleIdentifier(ref tokenType, ref tokenValue);
            }
            else
            {
                HandleOperator(ref tokenType, ref tokenValue);
            }
        }
        
        /// <summary>
        /// Handle whitespace characters
        /// </summary>
        private void HandleWhitespace(ref int tokenType, ref string tokenValue)
        {
            int start = position;
            
            while (position < input.Length && char.IsWhiteSpace(input[position]) && input[position] != '\n')
            {
                position++;
                column++;
            }
            
            tokenType = TOKEN_WHITESPACE;
            tokenValue = input.Substring(start, position - start);
        }
        
        /// <summary>
        /// Handle comments (from # to end of line)
        /// </summary>
        private void HandleComment(ref int tokenType, ref string tokenValue)
        {
            int start = position;
            
            position++; // Skip #
            column++;
            
            while (position < input.Length && input[position] != '\n')
            {
                position++;
                column++;
            }
            
            tokenType = TOKEN_COMMENT;
            tokenValue = input.Substring(start, position - start);
        }
        
        /// <summary>
        /// Handle string literals (both single and double quoted)
        /// </summary>
        private void HandleString(char quoteType, ref int tokenType, ref string tokenValue)
        {
            int start = position;
            
            position++; // Skip opening quote
            column++;
            
            string value = "";
            while (position < input.Length && input[position] != quoteType)
            {
                if (input[position] == '\\' && position + 1 < input.Length)
                {
                    // Handle escape sequences
                    position++;
                    column++;
                    if (position < input.Length)
                    {
                        char escaped = input[position];
                        switch (escaped)
                        {
                            case 'n': value += '\n'; break;
                            case 't': value += '\t'; break;
                            case 'r': value += '\r'; break;
                            case '\\': value += '\\'; break;
                            case '"': value += '"'; break;
                            case '\'': value += '\''; break;
                            default: value += escaped; break;
                        }
                    }
                }
                else
                {
                    value += input[position];
                }
                position++;
                column++;
            }
            
            if (position < input.Length && input[position] == quoteType)
            {
                position++; // Skip closing quote
                column++;
            }
            
            tokenType = TOKEN_STRING;
            tokenValue = value;
        }
        
        /// <summary>
        /// Handle variable references ($variable)
        /// </summary>
        private void HandleVariable(ref int tokenType, ref string tokenValue)
        {
            int start = position;
            
            position++; // Skip $
            column++;
            
            if (position >= input.Length)
            {
                tokenType = TOKEN_DOLLAR;
                tokenValue = "$";
                return;
            }
            
            // Check for special variables like $1, $2, etc.
            if (char.IsDigit(input[position]))
            {
                tokenType = TOKEN_VARIABLE;
                tokenValue = "$" + input[position].ToString();
                position++;
                column++;
                return;
            }
            
            // Regular variable name
            string varName = "";
            while (position < input.Length && (char.IsLetterOrDigit(input[position]) || input[position] == '_'))
            {
                varName += input[position];
                position++;
                column++;
            }
            
            tokenType = TOKEN_VARIABLE;
            tokenValue = "$" + varName;
        }
        
        /// <summary>
        /// Handle numeric literals
        /// </summary>
        private void HandleNumber(ref int tokenType, ref string tokenValue)
        {
            int start = position;
            
            while (position < input.Length && char.IsDigit(input[position]))
            {
                position++;
                column++;
            }
            
            // Check for decimal point
            if (position < input.Length && input[position] == '.')
            {
                position++;
                column++;
                while (position < input.Length && char.IsDigit(input[position]))
                {
                    position++;
                    column++;
                }
            }
            
            tokenType = TOKEN_NUMBER;
            tokenValue = input.Substring(start, position - start);
        }
        
        /// <summary>
        /// Handle identifiers and keywords
        /// </summary>
        private void HandleIdentifier(ref int tokenType, ref string tokenValue)
        {
            int start = position;
            
            while (position < input.Length && (char.IsLetterOrDigit(input[position]) || input[position] == '_'))
            {
                position++;
                column++;
            }
            
            tokenValue = input.Substring(start, position - start);
            
            // Check if it's a keyword
            tokenType = TOKEN_IDENTIFIER;
            if (IsKeyword(tokenValue))
            {
                tokenType = TOKEN_COMMAND;
            }
        }
        
        /// <summary>
        /// Handle operators and special characters
        /// </summary>
        private void HandleOperator(ref int tokenType, ref string tokenValue)
        {
            char currentChar = input[position];
            
            position++;
            column++;
            
            tokenType = TOKEN_UNKNOWN;
            tokenValue = currentChar.ToString();
            
            switch (currentChar)
            {
                // Arithmetic operators
                case '+':
                    if (position < input.Length && input[position] == '+')
                    {
                        position++;
                        column++;
                        tokenValue = "++";
                        tokenType = TOKEN_INCREMENT;
                    }
                    else if (position < input.Length && input[position] == '=')
                    {
                        position++;
                        column++;
                        tokenValue = "+=";
                        tokenType = TOKEN_PLUS_ASSIGN;
                    }
                    else
                    {
                        tokenType = TOKEN_PLUS;
                    }
                    break;
                case '-':
                    if (position < input.Length && input[position] == '-')
                    {
                        position++;
                        column++;
                        tokenValue = "--";
                        tokenType = TOKEN_DECREMENT;
                    }
                    else if (position < input.Length && input[position] == '=')
                    {
                        position++;
                        column++;
                        tokenValue = "-=";
                        tokenType = TOKEN_MINUS_ASSIGN;
                    }
                    else
                    {
                        tokenType = TOKEN_MINUS;
                    }
                    break;
                case '*':
                    if (position < input.Length && input[position] == '*')
                    {
                        position++;
                        column++;
                        tokenValue = "**";
                        tokenType = TOKEN_POWER;
                    }
                    else if (position < input.Length && input[position] == '=')
                    {
                        position++;
                        column++;
                        tokenValue = "*=";
                        tokenType = TOKEN_MUL_ASSIGN;
                    }
                    else
                    {
                        tokenType = TOKEN_MULTIPLY;
                    }
                    break;
                case '%':
                    if (position < input.Length && input[position] == '=')
                    {
                        position++;
                        column++;
                        tokenValue = "%=";
                        tokenType = TOKEN_MOD_ASSIGN;
                    }
                    else
                    {
                        tokenType = TOKEN_MODULO;
                    }
                    break;
                
                // Comparison operators
                case '=':
                    if (position < input.Length && input[position] == '=')
                    {
                        position++;
                        column++;
                        tokenValue = "==";
                        tokenType = TOKEN_EQUAL;
                    }
                    else
                    {
                        tokenType = TOKEN_ASSIGN;
                    }
                    break;
                case '!':
                    if (position < input.Length && input[position] == '=')
                    {
                        position++;
                        column++;
                        tokenValue = "!=";
                        tokenType = TOKEN_NOT_EQUAL;
                    }
                    else
                    {
                        tokenType = TOKEN_NOT;
                    }
                    break;
                case '<':
                    if (position < input.Length && input[position] == '=')
                    {
                        position++;
                        column++;
                        tokenValue = "<=";
                        tokenType = TOKEN_LESS_EQUAL;
                    }
                    else
                    {
                        tokenType = TOKEN_LESS;
                    }
                    break;
                case '>':
                    if (position < input.Length && input[position] == '=')
                    {
                        position++;
                        column++;
                        tokenValue = ">=";
                        tokenType = TOKEN_GREATER_EQUAL;
                    }
                    else if (position < input.Length && input[position] == '>')
                    {
                        position++;
                        column++;
                        tokenValue = ">>";
                        tokenType = TOKEN_APPEND;
                    }
                    else
                    {
                        tokenType = TOKEN_GREATER;
                    }
                    break;
                
                // Logical operators
                case '&':
                    if (position < input.Length && input[position] == '&')
                    {
                        position++;
                        column++;
                        tokenValue = "&&";
                        tokenType = TOKEN_AND;
                    }
                    else
                    {
                        tokenType = TOKEN_BITWISE_AND;
                    }
                    break;
                case '|':
                    if (position < input.Length && input[position] == '|')
                    {
                        position++;
                        column++;
                        tokenValue = "||";
                        tokenType = TOKEN_OR;
                    }
                    else
                    {
                        tokenType = TOKEN_BITWISE_OR;
                    }
                    break;
                case '^':
                    tokenType = TOKEN_BITWISE_XOR;
                    break;
                case '~':
                    tokenType = TOKEN_BITWISE_NOT;
                    break;
                
                // Special characters
                case ';':
                    tokenType = TOKEN_SEMICOLON;
                    break;
                case '.':
                    tokenType = TOKEN_DOT;
                    break;
                case '/':
                    tokenType = TOKEN_DIVIDE;
                    break;
                case '\\':
                    tokenType = TOKEN_BACKSLASH;
                    break;
                case '"':
                    tokenType = TOKEN_QUOTE;
                    break;
                case '\'':
                    tokenType = TOKEN_SINGLE_QUOTE;
                    break;
                case '`':
                    tokenType = TOKEN_BACKTICK;
                    break;
                case '$':
                    tokenType = TOKEN_DOLLAR;
                    break;
                
                // Grouping
                case '(':
                    tokenType = TOKEN_LPAREN;
                    break;
                case ')':
                    tokenType = TOKEN_RPAREN;
                    break;
                case '{':
                    tokenType = TOKEN_LBRACE;
                    break;
                case '}':
                    tokenType = TOKEN_RBRACE;
                    break;
                case '[':
                    tokenType = TOKEN_LBRACKET;
                    break;
                case ']':
                    tokenType = TOKEN_RBRACKET;
                    break;
            }
        }
        
        /// <summary>
        /// Check if a string is a keyword
        /// </summary>
        private bool IsKeyword(string word)
        {
            for (int i = 0; i < keywords.Length; i++)
            {
                if (keywords[i] == word.ToLower())
                {
                    return true;
                }
            }
            return false;
        }
        
        /// <summary>
        /// Get token type at index
        /// </summary>
        public int GetTokenType(int index)
        {
            if (index >= 0 && index < tokenCount)
            {
                return tokenTypes[index];
            }
            return TOKEN_EOF;
        }
        
        /// <summary>
        /// Get token value at index
        /// </summary>
        public string GetTokenValue(int index)
        {
            if (index >= 0 && index < tokenCount)
            {
                return tokenValues[index];
            }
            return "";
        }
        
        /// <summary>
        /// Get token position at index
        /// </summary>
        public int GetTokenPosition(int index)
        {
            if (index >= 0 && index < tokenCount)
            {
                return tokenPositions[index];
            }
            return 0;
        }
        
        /// <summary>
        /// Get token line at index
        /// </summary>
        public int GetTokenLine(int index)
        {
            if (index >= 0 && index < tokenCount)
            {
                return tokenLines[index];
            }
            return 0;
        }
        
        /// <summary>
        /// Get token column at index
        /// </summary>
        public int GetTokenColumn(int index)
        {
            if (index >= 0 && index < tokenCount)
            {
                return tokenColumns[index];
            }
            return 0;
        }


        
        /// <summary>
        /// Get the total number of tokens
        /// </summary>
        public int GetTokenCount()
        {
            return tokenCount;
        }
        
        /// <summary>
        /// Reset token consumption index
        /// </summary>
        public void Reset()
        {
            currentTokenIndex = 0;
        }
        
        /// <summary>
        /// Debug method to print all tokens
        /// </summary>
        public void PrintTokens()
        {
            Debug.Log("=== Lexical Analysis Results ===");
            for (int i = 0; i < tokenCount; i++)
            {
                string tokenStr = $"{GetTokenTypeName(tokenTypes[i])}: '{tokenValues[i]}' (pos: {tokenPositions[i]}, line: {tokenLines[i]}, col: {tokenColumns[i]})";
                Debug.Log($"[{i}] {tokenStr}");
            }
            Debug.Log($"Total tokens: {tokenCount}");
        }
        
        /// <summary>
        /// Placeholder methods for token handling (can be extended)
        /// </summary>
        private void HandleKeywordToken(string token)
        {
            // Can be extended for specific keyword handling
        }
        
        private void HandleRegularToken(string token)
        {
            // Can be extended for regular token handling
        }
    }
}