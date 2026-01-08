using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using HopeTools;

namespace HopeTools
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class HopeLexerDemo : UdonSharpBehaviour
    {
        [SerializeField] private HopeShellParserGenerator lexer;
        [SerializeField] private string[] testCommands = new string[]
        {
            "help",
            "cd /home/user",
            "echo \"Hello World\"",
            "$var = 42",
            "ls -la | grep test",
            "# This is a comment",
            "if ($x > 5) { echo \"big\" }",
            "cat file.txt >> output.txt",
            // New operator tests
            "x = 2 + 3 * 4 - 5 / 6 % 7 ** 8",
            "if (a == b && c != d || e <= f && g >= h)",
            "counter++",
            "value--",
            "total += 10",
            "count -= 5",
            "result *= 2",
            "average /= count",
            "remainder %= 3",
            "flags = a & b | c ^ ~d"
        };
        
        void Start()
        {
            if (lexer == null)
            {
                Debug.LogError("Lexer not assigned!");
                return;
            }
            
            Debug.Log("=== Lexical Analyzer Demo ===");
            
            // Test each command
            for (int i = 0; i < testCommands.Length; i++)
            {
                if (!string.IsNullOrEmpty(testCommands[i]))
                {
                    TestLexicalAnalysis(testCommands[i]);
                }
            }
        }
        
        private void TestLexicalAnalysis(string command)
        {
            Debug.Log($"\n--- Analyzing: \"{command}\" ---");
            
            // Tokenize the input
            int tokenCount = lexer.Lex(command);
            
            // Display results
            Debug.Log($"Found {tokenCount} tokens:");
            for (int i = 0; i < tokenCount; i++)
            {
                int tokenType = lexer.GetTokenType(i);
                string tokenValue = lexer.GetTokenValue(i);
                int tokenLine = lexer.GetTokenLine(i);
                int tokenColumn = lexer.GetTokenColumn(i);
                Debug.Log($"  [{i}] {GetTokenDisplayString(tokenType, tokenValue)} (line: {tokenLine}, col: {tokenColumn})");
            }
        }
        
        private string GetTokenDisplayString(int tokenType, string tokenValue)
        {
            // Basic tokens
            if (tokenType == 1) return $"[IDENTIFIER] {tokenValue}";
            if (tokenType == 2) return $"[STRING] \"{tokenValue}\"";
            if (tokenType == 3) return $"[NUMBER] {tokenValue}";
            if (tokenType == 0) return $"[IDENTIFIER] {tokenValue}";
            if (tokenType == 50) return $"[VARIABLE] {tokenValue}";
            if (tokenType == 52) return $"[COMMAND] {tokenValue}";
            if (tokenType == 40) return $"[WHITESPACE] '{tokenValue.Replace(" ", "·")}'";
            if (tokenType == 41) return "[NEWLINE]";
            if (tokenType == 53) return $"[COMMENT] {tokenValue}";
            if (tokenType == 100) return "[EOF]";
            
            // Arithmetic operators
            if (tokenType == 60) return "[PLUS] +";
            if (tokenType == 61) return "[MINUS] -";
            if (tokenType == 62) return "[MULTIPLY] *";
            if (tokenType == 63) return "[DIVIDE] /";
            if (tokenType == 64) return "[MODULO] %";
            if (tokenType == 65) return "[POWER] **";
            
            // Comparison operators
            if (tokenType == 70) return "[EQUAL] ==";
            if (tokenType == 71) return "[NOT_EQUAL] !=";
            if (tokenType == 72) return "[LESS_EQUAL] <=";
            if (tokenType == 73) return "[GREATER_EQUAL] >=";
            if (tokenType == 74) return "[LESS] <";
            if (tokenType == 75) return "[GREATER] >";
            
            // Logical operators
            if (tokenType == 80) return "[NOT] !";
            if (tokenType == 81) return "[BITWISE_AND] &";
            if (tokenType == 82) return "[BITWISE_OR] |";
            if (tokenType == 83) return "[BITWISE_XOR] ^";
            if (tokenType == 84) return "[BITWISE_NOT] ~";
            
            // Increment/Decrement
            if (tokenType == 90) return "[INCREMENT] ++";
            if (tokenType == 91) return "[DECREMENT] --";
            
            // Compound assignment
            if (tokenType == 92) return "[PLUS_ASSIGN] +=";
            if (tokenType == 93) return "[MINUS_ASSIGN] -=";
            if (tokenType == 94) return "[MUL_ASSIGN] *=";
            if (tokenType == 95) return "[DIV_ASSIGN] /=";
            if (tokenType == 96) return "[MOD_ASSIGN] %=";
            
            // Shell operators
            if (tokenType == 4) return "[ASSIGN] =";
            if (tokenType == 11) return "[PIPE] |";
            if (tokenType == 12) return "[REDIRECT_OUT] >";
            if (tokenType == 13) return "[REDIRECT_IN] <";
            if (tokenType == 14) return "[APPEND] >>";
            if (tokenType == 15) return "[SEMICOLON] ;";
            if (tokenType == 16) return "[AND] &&";
            if (tokenType == 17) return "[OR] ||";
            
            // Special characters
            if (tokenType == 20) return "[DOLLAR] $";
            if (tokenType == 21) return "[DOT] .";
            if (tokenType == 22) return "[SLASH] /";
            if (tokenType == 23) return "[BACKSLASH] \\";
            if (tokenType == 24) return "[QUOTE] \"";
            if (tokenType == 25) return "[SINGLE_QUOTE] '"; 
            if (tokenType == 26) return "[BACKTICK] `";
            
            // Grouping
            if (tokenType == 30) return "[LPAREN] (";
            if (tokenType == 31) return "[RPAREN] )";
            if (tokenType == 32) return "[LBRACE] {";
            if (tokenType == 33) return "[RBRACE] }";
            if (tokenType == 34) return "[LBRACKET] [";
            if (tokenType == 35) return "[RBRACKET] ]";
            
            return $"[UNKNOWN_{tokenType}] {tokenValue}";
        }
        
        // Test method that can be called from Unity Inspector
        public void TestCustomCommand(string command)
        {
            if (lexer != null && !string.IsNullOrEmpty(command))
            {
                TestLexicalAnalysis(command);
                // Reset lexer for next use
                lexer.Reset();
            }
        }
    }
}