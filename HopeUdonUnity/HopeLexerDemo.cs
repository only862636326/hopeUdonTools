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
            "cat file.txt >> output.txt"
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
            switch (tokenType)
            {
                case 40: // TOKEN_WHITESPACE
                    return $"[WHITESPACE] '{tokenValue.Replace(" ", "·")}'";
                case 41: // TOKEN_NEWLINE
                    return "[NEWLINE]";
                case 1: // TOKEN_STRING
                    return $"[STRING] \"{tokenValue}\"";
                case 2: // TOKEN_NUMBER
                    return $"[NUMBER] {tokenValue}";
                case 0: // TOKEN_IDENTIFIER
                    return $"[IDENTIFIER] {tokenValue}";
                case 50: // TOKEN_VARIABLE
                    return $"[VARIABLE] {tokenValue}";
                case 52: // TOKEN_COMMAND
                    return $"[COMMAND] {tokenValue}";
                case 10: // TOKEN_ASSIGN
                    return "[ASSIGN] =";
                case 11: // TOKEN_PIPE
                    return "[PIPE] |";
                case 12: // TOKEN_REDIRECT_OUT
                    return "[REDIRECT_OUT] >";
                case 13: // TOKEN_REDIRECT_IN
                    return "[REDIRECT_IN] <";
                case 14: // TOKEN_APPEND
                    return "[APPEND] >>";
                case 15: // TOKEN_SEMICOLON
                    return "[SEMICOLON] ;";
                case 16: // TOKEN_AND
                    return "[AND] &&";
                case 17: // TOKEN_OR
                    return "[OR] ||";
                case 100: // TOKEN_EOF
                    return "[EOF]";
                default:
                    return $"[{tokenType}] {tokenValue}";
            }
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