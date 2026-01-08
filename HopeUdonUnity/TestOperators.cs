using UnityEngine;

namespace HopeTools.HopeUdonUnity
{
    public class TestOperators : MonoBehaviour
    {
        void Start()
        {
            // Create a simple test to verify operators work
            HopeShellParserGenerator lexer = gameObject.AddComponent<HopeShellParserGenerator>();
            
            // Test arithmetic operators
            string arithmeticTest = "2 + 3 * 4 - 5 / 6 % 7 ** 8";
            int tokenCount = lexer.Lex(arithmeticTest);
            Debug.Log("=== Arithmetic Operators Test ===");
            Debug.Log($"Input: {arithmeticTest}");
            Debug.Log($"Token count: {tokenCount}");
            for (int i = 0; i < tokenCount; i++)
            {
                string tokenName = GetTokenName(lexer.GetTokenType(i));
                string tokenValue = lexer.GetTokenValue(i);
                Debug.Log($"Token {i}: {tokenName} = '{tokenValue}'");
            }
            
            // Test comparison operators
            string comparisonTest = "a == b != c <= d >= e < f > g";
            tokenCount = lexer.Lex(comparisonTest);
            Debug.Log("=== Comparison Operators Test ===");
            Debug.Log($"Input: {comparisonTest}");
            Debug.Log($"Token count: {tokenCount}");
            for (int i = 0; i < tokenCount; i++)
            {
                string tokenName = GetTokenName(lexer.GetTokenType(i));
                string tokenValue = lexer.GetTokenValue(i);
                Debug.Log($"Token {i}: {tokenName} = '{tokenValue}'");
            }
            
            // Test logical operators
            string logicalTest = "!a && b || c & d | e ^ f ~ g";
            tokenCount = lexer.Lex(logicalTest);
            Debug.Log("=== Logical Operators Test ===");
            Debug.Log($"Input: {logicalTest}");
            Debug.Log($"Token count: {tokenCount}");
            for (int i = 0; i < tokenCount; i++)
            {
                string tokenName = GetTokenName(lexer.GetTokenType(i));
                string tokenValue = lexer.GetTokenValue(i);
                Debug.Log($"Token {i}: {tokenName} = '{tokenValue}'");
            }
            
            // Test increment/decrement and compound assignment
            string compoundTest = "x++ y-- z += 1 w -= 2 a *= 3 b /= 4 c %= 5";
            tokenCount = lexer.Lex(compoundTest);
            Debug.Log("=== Increment/Decrement & Compound Assignment Test ===");
            Debug.Log($"Input: {compoundTest}");
            Debug.Log($"Token count: {tokenCount}");
            for (int i = 0; i < tokenCount; i++)
            {
                string tokenName = GetTokenName(lexer.GetTokenType(i));
                string tokenValue = lexer.GetTokenValue(i);
                Debug.Log($"Token {i}: {tokenName} = '{tokenValue}'");
            }
        }
        
        private string GetTokenName(int tokenType)
        {
            switch (tokenType)
            {
                case 1: return "IDENTIFIER";
                case 2: return "STRING";
                case 3: return "NUMBER";
                case 60: return "PLUS";
                case 61: return "MINUS";
                case 62: return "MULTIPLY";
                case 63: return "DIVIDE";
                case 64: return "MODULO";
                case 65: return "POWER";
                case 70: return "EQUAL";
                case 71: return "NOT_EQUAL";
                case 72: return "LESS_EQUAL";
                case 73: return "GREATER_EQUAL";
                case 74: return "LESS";
                case 75: return "GREATER";
                case 80: return "NOT";
                case 81: return "BITWISE_AND";
                case 82: return "BITWISE_OR";
                case 83: return "BITWISE_XOR";
                case 84: return "BITWISE_NOT";
                case 90: return "INCREMENT";
                case 91: return "DECREMENT";
                case 92: return "PLUS_ASSIGN";
                case 93: return "MINUS_ASSIGN";
                case 94: return "MUL_ASSIGN";
                case 95: return "DIV_ASSIGN";
                case 96: return "MOD_ASSIGN";
                case 16: return "AND";
                case 17: return "OR";
                case 11: return "PIPE";
                case 12: return "REDIRECT_OUT";
                case 13: return "REDIRECT_IN";
                case 14: return "APPEND";
                case 15: return "SEMICOLON";
                case 4: return "ASSIGN";
                case 20: return "DOLLAR";
                case 21: return "DOT";
                case 22: return "SLASH";
                case 23: return "BACKSLASH";
                case 24: return "QUOTE";
                case 25: return "SINGLE_QUOTE";
                case 26: return "BACKTICK";
                case 30: return "LPAREN";
                case 31: return "RPAREN";
                case 32: return "LBRACE";
                case 33: return "RBRACE";
                case 34: return "LBRACKET";
                case 35: return "RBRACKET";
                case 40: return "WHITESPACE";
                case 41: return "NEWLINE";
                case 50: return "VARIABLE";
                case 51: return "MEMBER_ACCESS";
                case 52: return "COMMAND";
                case 53: return "COMMENT";
                case 100: return "EOF";
                case 999: return "UNKNOWN";
                default: return $"UNKNOWN_{tokenType}";
            }
        }
    }
}