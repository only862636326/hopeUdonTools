
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class HuKeyInput : UdonSharpBehaviour
{
    void Start()
    {
        ;
    }

    void Update()
    {
        DetectPcKey();
    }

    public void KeyEnter( KeyCode key)
    {
        // hugf.Log("ToggleEvnKey_Enter"); 
        // Debug.Log(key);
    }
    
    public void KeyExit( KeyCode key)
    {
        //hugf.Log("ToggleEvnKey_Exit"); 
    }
    public void KeyLongPress( KeyCode key)
    {
        //hugf.Log("ToggleEvnKey_LongPress"); 
    }
    public void KeyShortPress( KeyCode key)
    {
        //hugf.Log("ToggleEvnKey_ShortPress"); 
        Debug.Log($"KeyShortPress: {key}");
    }
    
    public void KeyDoublePress( KeyCode key)
    {
        //hugf.Log("ToggleEvnKey_DoublePress"); 
    }

    public string _key_name;

    public void TrgEnterKey()
    {
        // this.SendCustomEvent(_key_name);
    }
    public void TrgExitKey()
    {
        // this.SendCustomEvent(_key_name);
    }
    
    #region PC Key Detect

    private void DetectPcKey()
    {
        // --- Function keys ---
        if (Input.GetKeyDown(KeyCode.Escape))      { KeyEnter(KeyCode.Escape); }
        if (Input.GetKeyDown(KeyCode.F1))           { KeyEnter(KeyCode.F1); }
        if (Input.GetKeyDown(KeyCode.F2))           { KeyEnter(KeyCode.F2); }
        if (Input.GetKeyDown(KeyCode.F3))           { KeyEnter(KeyCode.F3); }
        if (Input.GetKeyDown(KeyCode.F4))           { KeyEnter(KeyCode.F4); }
        if (Input.GetKeyDown(KeyCode.F5))           { KeyEnter(KeyCode.F5); }
        if (Input.GetKeyDown(KeyCode.F6))           { KeyEnter(KeyCode.F6); }
        if (Input.GetKeyDown(KeyCode.F7))           { KeyEnter(KeyCode.F7); }
        if (Input.GetKeyDown(KeyCode.F8))           { KeyEnter(KeyCode.F8); }
        if (Input.GetKeyDown(KeyCode.F9))           { KeyEnter(KeyCode.F9); }
        if (Input.GetKeyDown(KeyCode.F10))          { KeyEnter(KeyCode.F10); }
        if (Input.GetKeyDown(KeyCode.F11))          { KeyEnter(KeyCode.F11); }
        if (Input.GetKeyDown(KeyCode.F12))          { KeyEnter(KeyCode.F12); }

        // --- Number row ---
        if (Input.GetKeyDown(KeyCode.BackQuote))    { KeyEnter(KeyCode.BackQuote); }
        if (Input.GetKeyDown(KeyCode.Alpha1))       { KeyEnter(KeyCode.Alpha1); }
        if (Input.GetKeyDown(KeyCode.Alpha2))       { KeyEnter(KeyCode.Alpha2); }
        if (Input.GetKeyDown(KeyCode.Alpha3))       { KeyEnter(KeyCode.Alpha3); }
        if (Input.GetKeyDown(KeyCode.Alpha4))       { KeyEnter(KeyCode.Alpha4); }
        if (Input.GetKeyDown(KeyCode.Alpha5))       { KeyEnter(KeyCode.Alpha5); }
        if (Input.GetKeyDown(KeyCode.Alpha6))       { KeyEnter(KeyCode.Alpha6); }
        if (Input.GetKeyDown(KeyCode.Alpha7))       { KeyEnter(KeyCode.Alpha7); }
        if (Input.GetKeyDown(KeyCode.Alpha8))       { KeyEnter(KeyCode.Alpha8); }
        if (Input.GetKeyDown(KeyCode.Alpha9))       { KeyEnter(KeyCode.Alpha9); }
        if (Input.GetKeyDown(KeyCode.Alpha0))       { KeyEnter(KeyCode.Alpha0); }
        if (Input.GetKeyDown(KeyCode.Minus))        { KeyEnter(KeyCode.Minus); }
        if (Input.GetKeyDown(KeyCode.Equals))       { KeyEnter(KeyCode.Equals); }
        if (Input.GetKeyDown(KeyCode.Backspace))    { KeyEnter(KeyCode.Backspace); }

        // --- Top row: Tab ---
        if (Input.GetKeyDown(KeyCode.Tab))          { KeyEnter(KeyCode.Tab); }

        // --- Letter keys Q-P ---
        if (Input.GetKeyDown(KeyCode.Q))            { KeyEnter(KeyCode.Q); }
        if (Input.GetKeyDown(KeyCode.W))            { KeyEnter(KeyCode.W); }
        if (Input.GetKeyDown(KeyCode.E))            { KeyEnter(KeyCode.E); }
        if (Input.GetKeyDown(KeyCode.R))            { KeyEnter(KeyCode.R); }
        if (Input.GetKeyDown(KeyCode.T))            { KeyEnter(KeyCode.T); }
        if (Input.GetKeyDown(KeyCode.Y))            { KeyEnter(KeyCode.Y); }
        if (Input.GetKeyDown(KeyCode.U))            { KeyEnter(KeyCode.U); }
        if (Input.GetKeyDown(KeyCode.I))            { KeyEnter(KeyCode.I); }
        if (Input.GetKeyDown(KeyCode.O))            { KeyEnter(KeyCode.O); }
        if (Input.GetKeyDown(KeyCode.P))            { KeyEnter(KeyCode.P); }
        if (Input.GetKeyDown(KeyCode.LeftBracket))  { KeyEnter(KeyCode.LeftBracket); }
        if (Input.GetKeyDown(KeyCode.RightBracket)) { KeyEnter(KeyCode.RightBracket); }
        if (Input.GetKeyDown(KeyCode.Backslash))    { KeyEnter(KeyCode.Backslash); }

        // --- CapsLock ---
        if (Input.GetKeyDown(KeyCode.CapsLock))     { KeyEnter(KeyCode.CapsLock); }

        // --- Letter keys A-L ---
        if (Input.GetKeyDown(KeyCode.A))            { KeyEnter(KeyCode.A); }
        if (Input.GetKeyDown(KeyCode.S))            { KeyEnter(KeyCode.S); }
        if (Input.GetKeyDown(KeyCode.D))            { KeyEnter(KeyCode.D); }
        if (Input.GetKeyDown(KeyCode.F))            { KeyEnter(KeyCode.F); }
        if (Input.GetKeyDown(KeyCode.G))            { KeyEnter(KeyCode.G); }
        if (Input.GetKeyDown(KeyCode.H))            { KeyEnter(KeyCode.H); }
        if (Input.GetKeyDown(KeyCode.J))            { KeyEnter(KeyCode.J); }
        if (Input.GetKeyDown(KeyCode.K))            { KeyEnter(KeyCode.K); }
        if (Input.GetKeyDown(KeyCode.L))            { KeyEnter(KeyCode.L); }
        if (Input.GetKeyDown(KeyCode.Semicolon))    { KeyEnter(KeyCode.Semicolon); }
        if (Input.GetKeyDown(KeyCode.Quote))        { KeyEnter(KeyCode.Quote); }
        if (Input.GetKeyDown(KeyCode.Return))       { KeyEnter(KeyCode.Return); }

        // --- Modifier keys ---
        if (Input.GetKeyDown(KeyCode.LeftShift))    { KeyEnter(KeyCode.LeftShift); }
        if (Input.GetKeyDown(KeyCode.RightShift))   { KeyEnter(KeyCode.RightShift); }

        // --- Letter keys Z-M ---
        if (Input.GetKeyDown(KeyCode.Z))            { KeyEnter(KeyCode.Z); }
        if (Input.GetKeyDown(KeyCode.X))            { KeyEnter(KeyCode.X); }
        if (Input.GetKeyDown(KeyCode.C))            { KeyEnter(KeyCode.C); }
        if (Input.GetKeyDown(KeyCode.V))            { KeyEnter(KeyCode.V); }
        if (Input.GetKeyDown(KeyCode.B))            { KeyEnter(KeyCode.B); }
        if (Input.GetKeyDown(KeyCode.N))            { KeyEnter(KeyCode.N); }
        if (Input.GetKeyDown(KeyCode.M))            { KeyEnter(KeyCode.M); }
        if (Input.GetKeyDown(KeyCode.Comma))        { KeyEnter(KeyCode.Comma); }
        if (Input.GetKeyDown(KeyCode.Period))       { KeyEnter(KeyCode.Period); }
        if (Input.GetKeyDown(KeyCode.Slash))        { KeyEnter(KeyCode.Slash); }

        // --- More modifiers ---
        if (Input.GetKeyDown(KeyCode.LeftControl))  { KeyEnter(KeyCode.LeftControl); }
        if (Input.GetKeyDown(KeyCode.RightControl)) { KeyEnter(KeyCode.RightControl); }
        if (Input.GetKeyDown(KeyCode.LeftWindows))  { KeyEnter(KeyCode.LeftWindows); }
        if (Input.GetKeyDown(KeyCode.RightWindows)) { KeyEnter(KeyCode.RightWindows); }
        if (Input.GetKeyDown(KeyCode.LeftAlt))      { KeyEnter(KeyCode.LeftAlt); }
        if (Input.GetKeyDown(KeyCode.RightAlt))     { KeyEnter(KeyCode.RightAlt); }
        if (Input.GetKeyDown(KeyCode.Space))        { KeyEnter(KeyCode.Space); }
        if (Input.GetKeyDown(KeyCode.Menu))         { KeyEnter(KeyCode.Menu); }

        // --- Navigation & editing ---
        if (Input.GetKeyDown(KeyCode.Delete))       { KeyEnter(KeyCode.Delete); }
        if (Input.GetKeyDown(KeyCode.Home))         { KeyEnter(KeyCode.Home); }
        if (Input.GetKeyDown(KeyCode.PageUp))       { KeyEnter(KeyCode.PageUp); }
        if (Input.GetKeyDown(KeyCode.End))          { KeyEnter(KeyCode.End); }
        if (Input.GetKeyDown(KeyCode.PageDown))     { KeyEnter(KeyCode.PageDown); }

        // --- Arrow keys ---
        if (Input.GetKeyDown(KeyCode.UpArrow))      { KeyEnter(KeyCode.UpArrow); }
        if (Input.GetKeyDown(KeyCode.LeftArrow))    { KeyEnter(KeyCode.LeftArrow); }
        if (Input.GetKeyDown(KeyCode.DownArrow))    { KeyEnter(KeyCode.DownArrow); }
        if (Input.GetKeyDown(KeyCode.RightArrow))   { KeyEnter(KeyCode.RightArrow); }
    }

    #endregion PC Key Detect

    #region ToggleEvnKey
    public void ToggleEvnKey_Esc()      { KeyShortPress(KeyCode.Escape); }
    public void ToggleEvnKey_F1()       { KeyShortPress(KeyCode.F1); }
    public void ToggleEvnKey_F2()       { KeyShortPress(KeyCode.F2); }
    public void ToggleEvnKey_F3()       { KeyShortPress(KeyCode.F3); }
    public void ToggleEvnKey_F4()       { KeyShortPress(KeyCode.F4); }
    public void ToggleEvnKey_F5()       { KeyShortPress(KeyCode.F5); }
    public void ToggleEvnKey_F6()       { KeyShortPress(KeyCode.F6); }
    public void ToggleEvnKey_F7()       { KeyShortPress(KeyCode.F7); }
    public void ToggleEvnKey_F8()       { KeyShortPress(KeyCode.F8); }
    public void ToggleEvnKey_F9()       { KeyShortPress(KeyCode.F9); }
    public void ToggleEvnKey_F10()      { KeyShortPress(KeyCode.F10); }
    public void ToggleEvnKey_F11()      { KeyShortPress(KeyCode.F11); }
    public void ToggleEvnKey_F12()      { KeyShortPress(KeyCode.F12); }
    public void ToggleEvnKey_Grave()    { KeyShortPress(KeyCode.BackQuote); }
    public void ToggleEvnKey_1()        { KeyShortPress(KeyCode.Alpha1); }
    public void ToggleEvnKey_2()        { KeyShortPress(KeyCode.Alpha2); }
    public void ToggleEvnKey_3()        { KeyShortPress(KeyCode.Alpha3); }
    public void ToggleEvnKey_4()        { KeyShortPress(KeyCode.Alpha4); }
    public void ToggleEvnKey_5()        { KeyShortPress(KeyCode.Alpha5); }
    public void ToggleEvnKey_6()        { KeyShortPress(KeyCode.Alpha6); }
    public void ToggleEvnKey_7()        { KeyShortPress(KeyCode.Alpha7); }
    public void ToggleEvnKey_8()        { KeyShortPress(KeyCode.Alpha8); }
    public void ToggleEvnKey_9()        { KeyShortPress(KeyCode.Alpha9); }
    public void ToggleEvnKey_0()        { KeyShortPress(KeyCode.Alpha0); }
    public void ToggleEvnKey_Minus()    { KeyShortPress(KeyCode.Minus); }
    public void ToggleEvnKey_Equals()   { KeyShortPress(KeyCode.Equals); }
    public void ToggleEvnKey_Bksp()     { KeyShortPress(KeyCode.Backspace); }
    public void ToggleEvnKey_Tab()      { KeyShortPress(KeyCode.Tab); }
    public void ToggleEvnKey_Q()        { KeyShortPress(KeyCode.Q); }
    public void ToggleEvnKey_W()        { KeyShortPress(KeyCode.W); }
    public void ToggleEvnKey_E()        { KeyShortPress(KeyCode.E); }
    public void ToggleEvnKey_R()        { KeyShortPress(KeyCode.R); }
    public void ToggleEvnKey_T()        { KeyShortPress(KeyCode.T); }
    public void ToggleEvnKey_Y()        { KeyShortPress(KeyCode.Y); }
    public void ToggleEvnKey_U()        { KeyShortPress(KeyCode.U); }
    public void ToggleEvnKey_I()        { KeyShortPress(KeyCode.I); }
    public void ToggleEvnKey_O()        { KeyShortPress(KeyCode.O); }
    public void ToggleEvnKey_P()        { KeyShortPress(KeyCode.P); }
    public void ToggleEvnKey_LBracket() { KeyShortPress(KeyCode.LeftBracket); }
    public void ToggleEvnKey_RBracket() { KeyShortPress(KeyCode.RightBracket); }
    public void ToggleEvnKey_Backslash(){ KeyShortPress(KeyCode.Backslash); }
    public void ToggleEvnKey_Caps()     { KeyShortPress(KeyCode.CapsLock); }
    public void ToggleEvnKey_A()        { KeyShortPress(KeyCode.A); }
    public void ToggleEvnKey_S()        { KeyShortPress(KeyCode.S); }
    public void ToggleEvnKey_D()        { KeyShortPress(KeyCode.D); }
    public void ToggleEvnKey_F()        { KeyShortPress(KeyCode.F); }
    public void ToggleEvnKey_G()        { KeyShortPress(KeyCode.G); }
    public void ToggleEvnKey_H()        { KeyShortPress(KeyCode.H); }
    public void ToggleEvnKey_J()        { KeyShortPress(KeyCode.J); }
    public void ToggleEvnKey_K()        { KeyShortPress(KeyCode.K); }
    public void ToggleEvnKey_L()        { KeyShortPress(KeyCode.L); }
    public void ToggleEvnKey_Semicolon(){ KeyShortPress(KeyCode.Semicolon); }
    public void ToggleEvnKey_Quote()    { KeyShortPress(KeyCode.Quote); }
    public void ToggleEvnKey_Enter()    { KeyShortPress(KeyCode.Return); }
    public void ToggleEvnKey_Shift()    { KeyShortPress(KeyCode.LeftShift); }
    public void ToggleEvnKey_Z()        { KeyShortPress(KeyCode.Z); }
    public void ToggleEvnKey_X()        { KeyShortPress(KeyCode.X); }
    public void ToggleEvnKey_C()        { KeyShortPress(KeyCode.C); }
    public void ToggleEvnKey_V()        { KeyShortPress(KeyCode.V); }
    public void ToggleEvnKey_B()        { KeyShortPress(KeyCode.B); }
    public void ToggleEvnKey_N()        { KeyShortPress(KeyCode.N); }
    public void ToggleEvnKey_M()        { KeyShortPress(KeyCode.M); }
    public void ToggleEvnKey_Comma()    { KeyShortPress(KeyCode.Comma); }
    public void ToggleEvnKey_Period()   { KeyShortPress(KeyCode.Period); }
    public void ToggleEvnKey_Slash()    { KeyShortPress(KeyCode.Slash); }
    public void ToggleEvnKey_Ctrl()     { KeyShortPress(KeyCode.LeftControl); }
    public void ToggleEvnKey_Win()      { KeyShortPress(KeyCode.LeftWindows); }
    public void ToggleEvnKey_Alt()      { KeyShortPress(KeyCode.LeftAlt); }
    public void ToggleEvnKey_Space()    { KeyShortPress(KeyCode.Space); }
    public void ToggleEvnKey_Fn()       { /* no KeyCode */ }
    public void ToggleEvnKey_Menu()     { KeyShortPress(KeyCode.Menu); }
    public void ToggleEvnKey_Del()      { KeyShortPress(KeyCode.Delete); }
    public void ToggleEvnKey_Home()     { KeyShortPress(KeyCode.Home); }
    public void ToggleEvnKey_PgUp()     { KeyShortPress(KeyCode.PageUp); }
    public void ToggleEvnKey_End()      { KeyShortPress(KeyCode.End); }
    public void ToggleEvnKey_PgDn()     { KeyShortPress(KeyCode.PageDown); }
    public void ToggleEvnKey_Up()       { KeyShortPress(KeyCode.UpArrow); }
    public void ToggleEvnKey_Left()     { KeyShortPress(KeyCode.LeftArrow); }
    public void ToggleEvnKey_Down()     { KeyShortPress(KeyCode.DownArrow); }
    public void ToggleEvnKey_Right()    { KeyShortPress(KeyCode.RightArrow); }
    public void ToggleEvnKey_ShiftL()   { KeyShortPress(KeyCode.LeftShift); }
    public void ToggleEvnKey_ShiftR()   { KeyShortPress(KeyCode.RightShift); }
    public void ToggleEvnKey_CtrlL()    { KeyShortPress(KeyCode.LeftControl); }
    public void ToggleEvnKey_AltL()     { KeyShortPress(KeyCode.LeftAlt); }
    public void ToggleEvnKey_AltR()     { KeyShortPress(KeyCode.RightAlt); }
    public void ToggleEvnKey_CtrlR()    { KeyShortPress(KeyCode.RightControl); }
    #endregion ToggleEvnKey

    // end method
}
