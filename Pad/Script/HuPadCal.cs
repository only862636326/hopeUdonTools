using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace HUPad
{
    /// <summary>
    /// Simple four-operation calculator.
    /// Exposes public methods for UI button binding via Unity Inspector.
    /// </summary>
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class HuPadCal : UdonSharpBehaviour
    {
        [Header("=== Display ===")]
        [SerializeField] private Text resultText;
        [SerializeField] private Text expressionText;

        // Internal state
        private float previousValue;
        private float currentValue;
        private int currentOperator; // 0=none, 1=add, 2=sub, 3=mul, 4=div
        private bool isNewEntry;
        private bool hasError;
        private string display = "0";
        private string expressionDisplay = "";

        // Operator constants
        private const int OP_NONE = 0;
        private const int OP_ADD = 1;
        private const int OP_SUB = 2;
        private const int OP_MUL = 3;
        private const int OP_DIV = 4;

        [Header("=== Settings ===")]
        [SerializeField] private bool enableKeyboardInput = true;

        void Start()
        {
            ClearAll();
        }

        // ==============================
        // Keyboard input
        // ==============================

        void Update()
        {
            if (enableKeyboardInput)
            {
                PollKeyboardInput();
            }
        }

        /// <summary>
        /// Poll keyboard input and route to calculator functions.
        /// </summary>
        private void PollKeyboardInput()
        {
            // --- Shift-requiring keys (no clean char mapping, keep direct) ---
            // Multiply: Shift + 8  →  '*'
            if (Input.GetKeyDown(KeyCode.Alpha8) && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
            {
                UiInput('*');
                return;
            }
            // Percent: Shift + 5 → '%'
            if (Input.GetKeyDown(KeyCode.Alpha5) && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
            {
                UiInput('%');
                return;
            }
            // Add: Shift + = → '+'
            if (Input.GetKeyDown(KeyCode.Equals) && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
            {
                UiInput('+');
                return;
            }

            // --- Plain single-key inputs via UiInput ---

            // Digits (main keyboard)
            if (Input.GetKeyDown(KeyCode.Alpha0)) { UiInput('0'); return; }
            if (Input.GetKeyDown(KeyCode.Alpha1)) { UiInput('1'); return; }
            if (Input.GetKeyDown(KeyCode.Alpha2)) { UiInput('2'); return; }
            if (Input.GetKeyDown(KeyCode.Alpha3)) { UiInput('3'); return; }
            if (Input.GetKeyDown(KeyCode.Alpha4)) { UiInput('4'); return; }
            if (Input.GetKeyDown(KeyCode.Alpha5)) { UiInput('5'); return; }
            if (Input.GetKeyDown(KeyCode.Alpha6)) { UiInput('6'); return; }
            if (Input.GetKeyDown(KeyCode.Alpha7)) { UiInput('7'); return; }
            if (Input.GetKeyDown(KeyCode.Alpha8)) { UiInput('8'); return; }
            if (Input.GetKeyDown(KeyCode.Alpha9)) { UiInput('9'); return; }
            // Digits (numpad)
            if (Input.GetKeyDown(KeyCode.Keypad0)) { UiInput('0'); return; }
            if (Input.GetKeyDown(KeyCode.Keypad1)) { UiInput('1'); return; }
            if (Input.GetKeyDown(KeyCode.Keypad2)) { UiInput('2'); return; }
            if (Input.GetKeyDown(KeyCode.Keypad3)) { UiInput('3'); return; }
            if (Input.GetKeyDown(KeyCode.Keypad4)) { UiInput('4'); return; }
            if (Input.GetKeyDown(KeyCode.Keypad5)) { UiInput('5'); return; }
            if (Input.GetKeyDown(KeyCode.Keypad6)) { UiInput('6'); return; }
            if (Input.GetKeyDown(KeyCode.Keypad7)) { UiInput('7'); return; }
            if (Input.GetKeyDown(KeyCode.Keypad8)) { UiInput('8'); return; }
            if (Input.GetKeyDown(KeyCode.Keypad9)) { UiInput('9'); return; }
            // Decimal
            if (Input.GetKeyDown(KeyCode.Period) || Input.GetKeyDown(KeyCode.KeypadPeriod)) { UiInput('.'); return; }
            // Subtract (main - and numpad -)
            if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus)) { UiInput('-'); return; }
            // Divide (numpad / and main /)
            if (Input.GetKeyDown(KeyCode.KeypadDivide) || Input.GetKeyDown(KeyCode.Slash)) { UiInput('/'); return; }
            // Numpad add
            if (Input.GetKeyDown(KeyCode.KeypadPlus)) { UiInput('+'); return; }
            // Numpad multiply
            if (Input.GetKeyDown(KeyCode.KeypadMultiply)) { UiInput('*'); return; }
            // Equals / Enter
            if (Input.GetKeyDown(KeyCode.Equals)) { UiInput('='); return; }
            if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return)) { UiInput('\r'); return; }
            // Backspace
            if (Input.GetKeyDown(KeyCode.Backspace)) { UiInput('\b'); return; }
            // Escape = C (Clear)
            if (Input.GetKeyDown(KeyCode.Escape)) { UiInput('C'); return; }
            // Delete = AC (no char mapping, keep direct)
            if (Input.GetKeyDown(KeyCode.Delete)) { ClearAll(); return; }
        }

        /// <summary>
        /// Parse operator int constant to display symbol.
        /// </summary>
        private string GetOperatorSymbol(int op)
        {
            switch (op)
            {
                case OP_ADD: return "+";
                case OP_SUB: return "-";
                case OP_MUL: return "×";
                case OP_DIV: return "÷";
                default:     return "";
            }
        }

        /// <summary>
        /// Update both result and expression displays.
        /// </summary>
        private void UpdateDisplay()
        {
            if (resultText != null)
            {
                resultText.text = display;
            }
            if (expressionText != null)
            {
                expressionText.text = expressionDisplay;
            }
        }

        /// <summary>
        /// Handle digit input.
        /// </summary>
        private void InputDigit(int digit)
        {
            if (hasError) return;

            if (isNewEntry)
            {
                // After equals: move result to upper display and reset for new calculation
                if (currentOperator == OP_NONE && expressionDisplay != "")
                {
                    // Strip "= " prefix from display (e.g. "= 6" → "6")
                    string resultOnly = display;
                    if (display.StartsWith("= "))
                    {
                        resultOnly = display.Substring(2);
                    }
                    expressionDisplay = resultOnly;
                    previousValue = float.Parse(resultOnly);
                    UpdateDisplay();
                }
                display = digit.ToString();
                isNewEntry = false;
            }
            else
            {
                if (display == "0")
                {
                    display = digit.ToString();
                }
                else
                {
                    if (display.Length >= 15) return;
                    display += digit.ToString();
                }
            }

            UpdateDisplay();
        }

        // ==============================
        // Decimal point
        // ==============================

        public void InputDecimal()
        {
            if (hasError) return;

            if (isNewEntry)
            {
                display = "0.";
                isNewEntry = false;
            }
            else
            {
                // Only add decimal if not already present
                if (!display.Contains("."))
                {
                    display += ".";
                }
            }

            UpdateDisplay();
        }

        // ==============================
        // Sign toggle (+/-)
        // ==============================

        public void ToggleSign()
        {
            if (hasError || display == "0") return;

            if (display.StartsWith("-"))
            {
                display = display.Substring(1);
            }
            else
            {
                display = "-" + display;
            }

            UpdateDisplay();
        }

        // ==============================
        // Percentage (%)
        // ==============================

        public void InputPercent()
        {
            if (hasError) return;

            float val = float.Parse(display);
            val /= 100f;
            display = val.ToString();
            isNewEntry = true;

            UpdateDisplay();
        }

        // ==============================
        // Operators: +, -, ×, ÷
        // ==============================

        public void OperatorAdd()    { SetOperator(OP_ADD); }
        public void OperatorSub()    { SetOperator(OP_SUB); }
        public void OperatorMul()    { SetOperator(OP_MUL); }
        public void OperatorDiv()    { SetOperator(OP_DIV); }

        /// <summary>
        /// Store current value and prepare for next input.
        /// If there's a pending operation, evaluate it first.
        /// </summary>
        private void SetOperator(int op)
        {
            if (hasError) return;

            // If there's already a pending operator, evaluate first
            if (currentOperator != OP_NONE && !isNewEntry)
            {
                Evaluate();
            }

            // Only overwrite previousValue when user has entered a new number
            if (!isNewEntry)
            {
                previousValue = float.Parse(display);
            }
            // else: user is just switching operator, keep previousValue unchanged

            currentOperator = op;
            display = "";
            isNewEntry = true;

            // Update expression display: "previousValue operator "
            expressionDisplay = previousValue.ToString() + " " + GetOperatorSymbol(op) + " ";
            UpdateDisplay();
        }

        // ==============================
        // Equals
        // ==============================

        public void Equals()
        {
            if (hasError || currentOperator == OP_NONE) return;

            // Show equation without "=" in upper display: "2 × 3"
            expressionDisplay = expressionDisplay + display;
            UpdateDisplay();

            Evaluate();
            currentOperator = OP_NONE;
            isNewEntry = true;

            // Prepend "= " to result in lower display
            display = "= " + display;
            UpdateDisplay();
        }

        /// <summary>
        /// Perform the pending arithmetic operation.
        /// </summary>
        private void Evaluate()
        {
            currentValue = float.Parse(display);

            switch (currentOperator)
            {
                case OP_ADD:
                    previousValue += currentValue;
                    break;
                case OP_SUB:
                    previousValue -= currentValue;
                    break;
                case OP_MUL:
                    previousValue *= currentValue;
                    break;
                case OP_DIV:
                    if (currentValue == 0f)
                    {
                        display = "Error";
                        hasError = true;
                        UpdateDisplay();
                        return;
                    }
                    previousValue /= currentValue;
                    break;
            }

            display = previousValue.ToString();
            UpdateDisplay();
        }

        // ==============================
        // Clear
        // ==============================

        public void Clear()
        {
            display = "0";
            isNewEntry = true;
            hasError = false;
            expressionDisplay = "";
            UpdateDisplay();
        }

        public void ClearAll()
        {
            display = "0";
            previousValue = 0f;
            currentValue = 0f;
            currentOperator = OP_NONE;
            isNewEntry = true;
            hasError = false;
            expressionDisplay = "";
            UpdateDisplay();
        }

        // ==============================
        // Backspace
        // ==============================

        public void Backspace()
        {
            if (hasError || isNewEntry) return;

            if (display.Length > 1)
            {
                // Remove last character, but handle "-" edge case
                if (display.Length == 2 && display.StartsWith("-"))
                {
                    display = "0";
                }
                else
                {
                    display = display.Substring(0, display.Length - 1);
                }
            }
            else
            {
                display = "0";
            }

            UpdateDisplay();
        }

        public void UiInput(char c)
        {
            Debug.Log($"UiInput: {c}");
            // Digits '0'-'9'
            if (c == '0') { InputDigit(0); return; }
            if (c == '1') { InputDigit(1); return; }
            if (c == '2') { InputDigit(2); return; }
            if (c == '3') { InputDigit(3); return; }
            if (c == '4') { InputDigit(4); return; }
            if (c == '5') { InputDigit(5); return; }
            if (c == '6') { InputDigit(6); return; }
            if (c == '7') { InputDigit(7); return; }
            if (c == '8') { InputDigit(8); return; }
            if (c == '9') { InputDigit(9); return; }

            // Operators
            if (c == '/') { SetOperator(OP_DIV); return; }
            if (c == '*') { SetOperator(OP_MUL); return; }
            if (c == '-') { SetOperator(OP_SUB); return; }
            if (c == '+') { SetOperator(OP_ADD); return; }

            // Decimal '.'
            if (c == '.') { InputDecimal(); return; }

            // Equals '=' or Enter
            if (c == '=') { Equals(); return; }
            if (c == '\r') { Equals(); return; } // Enter

            // Clear / Back
            // Percent '%'
            if (c == '%') { InputPercent(); return; }

            // Clear / Back
            if (c == '\b') { Backspace(); return; } // Backspace
            if (c == 'C') { Clear(); return; }
            if (c == 'c') { Clear(); return; }
        }

        #region  ui input Key
        public void Key_7()    { UiInput('7'); }
        public void Key_8()    { UiInput('8'); }
        public void Key_9()    { UiInput('9'); }
        public void Key_4()    { UiInput('4'); }
        public void Key_5()    { UiInput('5'); }
        public void Key_6()    { UiInput('6'); }
        public void Key_1()    { UiInput('1'); }
        public void Key_2()    { UiInput('2'); }
        public void Key_3()    { UiInput('3'); }
        public void Key_0()    { UiInput('0'); }
        public void Key_00()   { UiInput('0'); UiInput('0'); }
        public void Key_point() { UiInput('.'); }
        public void Key_div()   { UiInput('/'); }
        public void Key_C()     { UiInput('C'); }
        public void Key_mul()   { UiInput('*'); }
        public void Key_dec()   { UiInput('-'); }
        public void Key_add()   { UiInput('+'); }
        public void Key_eq()    { UiInput('='); }
        #endregion  ui input Key
		// end method
    }
}


