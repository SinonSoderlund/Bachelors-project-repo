using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UtilitySpace;

public class ControllUIContainer : MonoBehaviour
{
    //Prefab instance name field and key name field/key binding activator
    [SerializeField] private Text controllName;
    [SerializeField] private Button controllBinder;
    //text for controllbinder
    private Text controllText;
    //Toggle for setting wether or not key is toggled or held down
    [SerializeField] private Toggle controllToggle;
    //the text for the toggle
    private Text toggleText;
    //refrence for current controll
    private Controll thisControll;
    //internal refrence to toggle value
    private bool toggleVal;
    //internal refremce to bound key code
    private KeyCode codeVal;
    //System token refrence
    private bool UnitPrimed = false;
    //refrence to the key bind organizeer
    KeyBindOrganizer KBOIR;

    Timer BindTimer;

    /// <summary>
    /// Function that sets up all of the paramaters of this CUIC correctly
    /// </summary>
    /// <param name="input"></param>
    public void SetUp(Controll input)
    {
        KBOIR = KeyBindOrganizer.KBO;
        controllName.text = input.Name;
        toggleVal = input.isToggle;
        controllToggle.isOn = toggleVal;
        toggleText = controllToggle.GetComponentInChildren<Text>();
        ToggleChange();
        controllText = controllBinder.GetComponentInChildren<Text>();
        controllText.text = input.inputCode.ToString();
        codeVal = input.inputCode;
        thisControll = input;
        controllToggle.onValueChanged.AddListener(delegate { ToggleChange(); });
        controllBinder.onClick.AddListener(delegate { PrimerRequest(); });
        KBOIR.tokenReturnRequest += tokenReturn;
        KBOIR.DR += DeathTime;
        //blocks the toggle toggle if the interaction key is this
        if (thisControll.toggleLocked == true)
            controllToggle.interactable = false;
        BindTimer = new Timer(0.2);
    }

    /// <summary>
    /// Changes the toggle text whenever the toggle in interacted with
    /// </summary>
    private void ToggleChange()
    {
        if (controllToggle.isOn)
            toggleText.text = "Key is Toggle";
        else toggleText.text = "Key is Hold";
    }

    /// <summary>
    /// Return function for the primer token
    /// </summary>
    /// <param name="b">refrence to Primer token</param>
    private void tokenReturn(ref bool b)
    {
        if (UnitPrimed)
        {
            b = UnitPrimed;
            UnitPrimed = false;
        }
    }

    /// <summary>
    /// Function for requesting the primer token from the KBO
    /// </summary>
    private void PrimerRequest()
    {
        BindTimer.Reset();
        KBOIR.TokenRequest(ref UnitPrimed);
  
    }

    /// <summary>
    /// Function called by KBO via delegate in order to deinstatiate current CUICs
    /// </summary>
    private void DeathTime()
    {
        if (UnitPrimed)
            KBOIR.returnToken(ref UnitPrimed);
        KBOIR.tokenReturnRequest -= tokenReturn;
        KBOIR.DR -= DeathTime;
        Destroy(gameObject);
    }

    //used to catch the input events if the unit is primed
    private void OnGUI()
    {
        if (UnitPrimed && BindTimer == true)
            ReCode(Event.current);
    }

    /// <summary>
    /// Function for retreiving new controll instance based on internally stored data of the CUIC
    /// </summary>
    /// <returns></returns>
    public Controll DataScoop()
    {
        return new Controll(thisControll.Name, thisControll.inputType, thisControll.inputClass, codeVal, controllToggle.isOn, thisControll.toggleLocked);
    }

    /// <summary>
    /// Function for setting a new data binding for the current CUIC
    /// </summary>
    /// <param name="NewCode"></param>
    private void ReCode(Event NewCode)
    {
        //only jey events are currently supported
        if (NewCode.type == EventType.KeyDown)//|| NewCode.type == EventType.MouseDown)
        {
            //sets internal key code ref
            codeVal = NewCode.keyCode;
            
            //updates the text of the controllbinder and then returns primer token to KBO

            //InputsNew[m_currentSet.index].InputParseType = InputType.Keyboard;
            controllText.text = codeVal.ToString();
            KBOIR.returnToken(ref UnitPrimed);
        }
        #region MouseBS
        else if (NewCode.type == EventType.MouseDown && !NewCode.isScrollWheel)
        {

            if (NewCode.button == 0)
            {
                codeVal = KeyCode.Mouse0;
                controllText.text = codeVal.ToString();
                KBOIR.returnToken(ref UnitPrimed);
            }
            else if (NewCode.button == 1)
            {
                codeVal = KeyCode.Mouse1;
                controllText.text = codeVal.ToString();
                KBOIR.returnToken(ref UnitPrimed);
            }
            else if (NewCode.button == 2)
            {
                codeVal = KeyCode.Mouse2;
                controllText.text = codeVal.ToString();
                KBOIR.returnToken(ref UnitPrimed);
            }
            else if (NewCode.button == 3)
            {
                codeVal = KeyCode.Mouse3;
                controllText.text = codeVal.ToString();
                KBOIR.returnToken(ref UnitPrimed);
            }
            else if (NewCode.button == 4)
            {
                codeVal = KeyCode.Mouse4;
                controllText.text = codeVal.ToString();
                KBOIR.returnToken(ref UnitPrimed);
            }
            else if (NewCode.button == 5)
            {
                codeVal = KeyCode.Mouse5;
                controllText.text = codeVal.ToString();
                KBOIR.returnToken(ref UnitPrimed);
            }
            else if (NewCode.button == 6)
            {
                codeVal = KeyCode.Mouse6;
                controllText.text = codeVal.ToString();
                KBOIR.returnToken(ref UnitPrimed);
            }

        }
        #endregion
    }

}
