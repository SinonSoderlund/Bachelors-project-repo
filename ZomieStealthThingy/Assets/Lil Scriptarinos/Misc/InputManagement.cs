using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UtilitySpace;

/// <summary>
/// Class for handling input management
/// </summary>

public class InputManagement : MonoBehaviour
{
    //Lenght ot inputtype enum
    int ITL;
    //names in strng format for all the input type members
    string[] ITNL = Enum.GetNames(typeof(InputType));
    //array of controls
    private Controll[] ControllArray;
    //static class imstance refrence
    public static InputManagement InputManager;
    //increadibly contrived way of doing this (setting up everything in a semi-automated manner)
    private InputClasses[] InputCLasses = new InputClasses[] { InputClasses.MovementClass, InputClasses.None };
    private InputType[] InpuTBreakList = new InputType[] { InputType.Sneak };
    private InputType[] TIYA = new InputType[] {};
    private InputType[] TLSA = new InputType[] { InputType.Interact, InputType.PauseMenu };
    private KeyCode[] DefaultInputs = new KeyCode[] { KeyCode.W, KeyCode.S, KeyCode.A, KeyCode.D, KeyCode.LeftShift, KeyCode.F,KeyCode.Escape, KeyCode.Space, KeyCode.Mouse0,KeyCode.Mouse1 };
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        //sets static refrence to this
        if (InputManager == null)
            InputManager = this;
        else
            DestroyImmediate(this);
        //sets ITL to lenght of ITNL
        ITL = ITNL.Length;
        //instantiates ControlArray to array of leght ITL
        ControllArray = new Controll[ITL];
        //break list index (increments every time a member in the break list is encountered in the main loop, moves forward the input classes
        int bi = 0;
        int ti = 0;
        int tlsi = 0;
        //loops through every inputtype and creates a control instance for it
        for(int i = 0; i < ITL; i++)
        {
            //sets name to string of enum class memebr name
            string name = ITNL[i];
            //sets current input type to cinverted version fo current index
            InputType inpt = (InputType)i;
            //check if current inputtype is in the break list, unless break index exceeds lenght of break list
            //if so it increments the break index
            if (bi < InpuTBreakList.Length && (InputType)i == InpuTBreakList[bi])
                bi++;
            //sets input class to current break index
            InputClasses inpc = InputCLasses[bi];
            //sets the input code to the current default input
            KeyCode inpcode = DefaultInputs[i];
            bool toggleState = false;
            if(ti < TIYA.Length &&(InputType)i == TIYA[ti])
            {
                toggleState = true;
                ti++;
            }
            bool TLS = false;
            if (tlsi < TLSA.Length && (InputType)i == TLSA[tlsi])
            {
                TLS = true;
                tlsi++;
            }
            //creates new control instance and puts it into its corresponding position in the controll array
            ControllArray[i] = new Controll(name, inpt, inpc, inpcode, toggleState, TLS); 
        }
    }

    public void setGun(bool set)
    {
        ControllArray[((int)InputType.FireGun)].SetDisabled(set);
    }

    private void Update()
    {//evaluate all controll entries
        foreach (Controll entry in ControllArray)
        {
            entry.Evaluate();
        }
    }
    //Fetches controll activation state from enum
    public bool GetInput(InputType inputVal)
    {
        return ControllArray[((int)inputVal)].ActivityEvaluation();
    }
    //fetches controll keycode string from enum
    public string getInputKeyName(InputType InTy)
    {
        return ControllArray[(int)InTy].GetKeyString();
    }
    public bool getInputKeyDisabled(InputType InTy)
    {
        return ControllArray[(int)InTy].checkDisabled();
    }

    //Fetches contrrect phrasing for prompt contrsuction depending on wether key is marked as hold or toggle
    public string ToggleInjectStart(InputType InTy)
    {
        if (ControllArray[(int)InTy].isToggle)
            return "Press ";
        else return "Hold ";
    }
    //same as abouve but for end prompt
    public string ToggleInjectEnd(InputType InTy)
    {
        if (ControllArray[(int)InTy].isToggle)
            return " to toggle ";
        else return " to ";
    }
    //returns the IMCA
    public Controll[] GetControllArray()
    {
        return ControllArray;
    }
    //overwrites IMCA
    public void setNewControllArray(Controll[] NCA)
    {
        ControllArray = NCA;
    }
}
[Serializable]
/// <summary>
/// Class for controll instances
/// </summary>
public class Controll
{
    //Class variable fields
    public string Name;
    public InputType inputType;
    public InputClasses inputClass;
    public KeyCode inputCode;
    public bool isToggle, toggleLocked;
    private Timer ToggleSwitch;
    private bool isActive = false;
    private bool Disabled = false;
    /// <summary>
    /// Contrsuctor for controll class
    /// </summary>
    /// <param name="name">Strng name of the controll instance</param>
    /// <param name="inputtype">inputtype enum for the instance</param>
    /// <param name="inputclass">The input class enum for the instance</param>
    /// <param name="inputcode">The keycode the instance is registered to</param>
    public Controll(string name, InputType inputtype, InputClasses inputclass, KeyCode inputcode, bool istoggle, bool isToggleLocked)
    {
        Name = name;
        inputType = inputtype;
        inputClass = inputclass;
        inputCode = inputcode;
        isToggle = istoggle;
        toggleLocked = isToggleLocked;
        if(isToggle)
            ToggleSwitch = new Timer(0.3);
    }

    /// <summary>
    /// Function for chaning the corresponding keycode for a controll instance
    /// </summary>
    /// <param name="icode">the new input keycode</param>
    public void KeycodeChange(KeyCode icode)
    {
        inputCode = icode;
    }

    public void SetDisabled(bool dstate)
    {
        Disabled = dstate;
    }

    public bool checkDisabled()
    {
        return Disabled;
    }

    /// <summary>
    /// Evaluates the input state for the current controll according to its settings
    /// </summary>
    public void Evaluate()
    {
        if (Disabled)
            return;
        if(isToggle)
        {
            if (ToggleSwitch)
                if (Input.GetKeyDown(inputCode))
                {
                    isActive = !isActive;
                    ToggleSwitch.Reset();
                }
        }
        else
        {
            isActive = Input.GetKey(inputCode);
        }
    }
    //returns keycode string
    public string GetKeyString()
    {
        return inputCode.ToString();
    }
    //returns activity state
    public bool ActivityEvaluation()
    {
        if (Disabled)
            return false;
        return isActive;
    }

    public string ReduceToString()
    {
        return "new Controll(\"" + Name + "\"," + inputType + "," + inputClass + ",Keycode." + inputCode + "," + isToggle + "," + toggleLocked + ")";
    }

}
