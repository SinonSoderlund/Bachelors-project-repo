using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UtilitySpace;
using UnityEngine.UI;
public class KeyBindOrganizer : MonoBehaviour
{
    //Various UI object refrences
    [SerializeField] private GameObject SpawnParent;
    [SerializeField] private GameObject SpawnObject;
    [SerializeField] private Button ToSetting;
    [SerializeField] private Button ToMain;
    [SerializeField] private Button Revert;
    [SerializeField] private Button Apply;
    //static refrence to this
    public static KeyBindOrganizer KBO;
    //delegate for requesting token return
    public delegate void TokenReturn(ref bool b);
    public TokenReturn tokenReturnRequest;
    //delegate for requesting that all CUICs self-terminate
    public delegate void DeathRequest();
    public DeathRequest DR;
    //the primer token
    private bool PrimerToken = true;
    //Array pf CUICs
    ControllUIContainer[] Glist;
    // Start is called before the first frame update
    void Start()
    {
        //set KBO to this, then add various listeners to make sure that functions fire of when they should given the relevant button press
        if (KBO == null)
            KBO = this;
        ToSetting.onClick.AddListener(delegate { IntantiateKeyContainers(); });
        Revert.onClick.AddListener(delegate { onRevert(); });
        Apply.onClick.AddListener(delegate {onApply(); });
        ToMain.onClick.AddListener(delegate { DR(); });
    }
    /// <summary>
    /// Function for spawning in all the needed CUICs and setting them up with their respective data
    /// </summary>
    private void IntantiateKeyContainers()
    {
        //Fetches the IM ControllArray
        Controll[] CA = InputManagement.InputManager.GetControllArray();
        //sets Glist to lenght of IMCA
        Glist = new ControllUIContainer[CA.Length];
        //Create one CUIC for each IMCA entry, and send the correctsponding controll instance to said CUIC
        for(int i = 0; i < Glist.Length;i++)
        {
            Glist[i] = Instantiate(SpawnObject, SpawnParent.transform).GetComponent<ControllUIContainer>();
            Glist[i].SetUp(CA[i]);
        }
    }
    /// <summary>
    /// Function for receiving token requests from CUICs
    /// </summary>
    /// <param name="b">the primer token of the CUIC wanting to be primed</param>
    public void TokenRequest(ref bool b)
    {
        //if token is not held by KBO, request token return
        if (!PrimerToken)
            tokenReturnRequest(ref PrimerToken);
        b = PrimerToken;
        PrimerToken = false;
    }
    /// <summary>
    /// Function for returning the token to the KBO manually
    /// </summary>
    /// <param name="b">the token refrence from the returning unit</param>
    public void returnToken(ref bool b)
    {
        if(b)
        {
            PrimerToken = b;
            b = false;
        }
    }

    /// <summary>
    /// REvert function, deletes all CUICs and then spawn in new ones with the data from the IMCA
    /// </summary>
    private void onRevert()
    {
        DR();
        IntantiateKeyContainers();
    }
    /// <summary>
    /// Function for applying new datta, scoops controlll instances from all CUICs and the overwrites the IMCA with new controlls
    /// </summary>
    private void onApply()
    {
        Controll[] NCA = new Controll[Glist.Length];
        for(int i = 0; i < Glist.Length; i++)
        {
            NCA[i] = Glist[i].DataScoop();
        }
        InputManagement.InputManager.setNewControllArray(NCA);
    }
}
