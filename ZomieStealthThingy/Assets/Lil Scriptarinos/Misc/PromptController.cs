using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UtilitySpace;
using UnityEngine.SceneManagement;

public class PromptController : MonoBehaviour
{
    //static ref to this
    public static PromptController promptController;
    //refs to UI elements
    [SerializeField] private Text ActionPrompt,TutorialPrompt,DialogueName,DialogueText;
    [SerializeField] private Image DialogueImage;
    [SerializeField] private Button DialogueButton;
    //static ref to input manager, for whatever reason
    private InputManagement IM;
    //ref to prompt requests
    private List<PromptRequest> PRL = new List<PromptRequest>();
    //start color of the Dialogue icon sprite
    private Color SC;
    //controll flags for dialogue system
    private int nextDialogue = 0;
    private bool isDialogue = false;
    //Token controller, used to controll prompt interaction allowance
    private TokenController TC = new TokenController();
    //player ref
    private Transform player;
    //short cooldown that shuts down SAAS after PRR in order to avoid double interactions
    private Timer retractionLockdown = new Timer(0.1), PMT = new Timer(0.3), DFUT = new Timer(0.1), NDLT = new Timer(0.2);
    // Start is called before the first frame update
    bool stop = false;
    bool OnPause = false;
    bool opDS = false;
    float opTS = 1;
    bool QTMA = false;
    bool DFUTT = false;
    [SerializeField] GameObject PHeader, QTMHeader;
    [SerializeField] Button ResBut, QTMBut, RetBut, QBut;
    private void Awake()
    {//sets static ref to this
        if (promptController != this)
            promptController = this;
        //fetches start color
        SC = DialogueImage.color;
    }
    void Start()
    {//fetches external static refs
        IM = InputManagement.InputManager;
        player = PlayerMovementScript.player.transform;
        //ActionPrompt.text = "Press " + IM.getInputKeyName(InputType.Interact) + " to hide.";
    }

    // Update is called once per frame
    void Update()
    {//if there isnt currently dialogue, evaluate prompts
        if(!isDialogue)
        SetActionActiveState();

        if (IsDialogue() && !OnPause && NDLT == true)
            if (IM.GetInput(InputType.DialogueNext))
                SetNextDialogue(1);
            else if (IM.GetInput(InputType.DialogueBack))
                SetNextDialogue(-1);

        if (IM.GetInput(InputType.PauseMenu) && PMT == true)
            onPauseMenu();
    }


    void onPauseMenu()
    {
        PMT.Reset();
        OnPause = !OnPause;
        if (OnPause)
        {
            if (isDialogue)
            {
                opDS = true;
                DFUTT = false;
            }
            else
                setDialogueFlag();
            opTS = Time.timeScale;
            Time.timeScale = 0;
            ResBut.onClick.AddListener(delegate{ onPauseMenu(); });
            QTMBut.onClick.AddListener(delegate { onQTM(); });
        }
        else
        {
            Time.timeScale = opTS;
            if (opDS)
            {
                opDS = false;
            }
            else UnsetDialogFlag();
            ResBut.onClick.RemoveAllListeners();
            QTMBut.onClick.RemoveAllListeners();
            if (QTMA)
                onQTM();
        }
        PHeader.SetActive(OnPause);
    }

    void onQTM()
    {
        QTMA = !QTMA;
        if(QTMA)
        {
            RetBut.onClick.AddListener(delegate { onQTM(); });
            QBut.onClick.AddListener(delegate { SceneManager.LoadScene(0); });
        }
        else
        {
            RetBut.onClick.RemoveAllListeners();
            QBut.onClick.RemoveAllListeners();
        }
        QTMHeader.SetActive(QTMA);
    }


    /// <summary>
    /// Sets the tutoral prompt from an external tutorial prompt script, i should proll ymodernize this n style with the action prompt system tbh, maybe comibe the two?
    /// </summary>
    /// <param name="prompt">The stirng prompt to be displayed</param>
    public void newTutorialPrompt(string prompt)
    {
        TutorialPrompt.enabled = true;
        TutorialPrompt.text = prompt;
    }
    //Hides tutorial prompt text upon range exit
    public void TutorialRangeExit()
    {
        TutorialPrompt.enabled = false;
    }

    public void shutdown()
    {
        DestroyImmediate(this.gameObject);

    }

    /// <summary>
    /// Evaluates all prompts and shows the one closest to the player
    /// </summary>
    private void SetActionActiveState()
    {
        //if list isnt empty and retraction lock is disabled
        if (PRL.Count > 0 && (retractionLockdown == true))
        {
            //enable prompt text, set init value for dist eval
            ActionPrompt.enabled = true;
            float f = 9999999999999999;
            //Create empty prompt to prevent nullrefs
            PromptRequest PR = new PromptRequest(transform, "");
            //go through all PRs
            foreach  (PromptRequest item in PRL)
            {if (stop)
                    return;
                //calc dist to player
                float n = (player.position - item.tRef.position).sqrMagnitude;
                //if dist is less than current ref, set current item as new ref
                //print(n + " " + item.pText);
                if (n < f)
                {
                    f = n;
                    PR = item;
                }
            }
            //set prompt text to closest prompt source, give token to the PR
            ActionPrompt.text = PR.pText;
            TC.GiveToken(PR.TokU);
        }//otherwise no prompt is to be shown, disable prompt text (uh idk if this will really behave properly with the lockdown functionality but eh no issues have stemmed from it yet so let it b for now
        else ActionPrompt.enabled = false;
    }

    /// <summary>
    /// Function for adding a new prompt request to the prompt controller
    /// </summary>
    /// <param name="PR">Prompt request</param>
    public void AddPromptRequest(PromptRequest PR)
    {
        //Adds PR TO PRL, adds PR TU to PCR TC
        PRL.Add(PR);
        TC.addUser(PR.TokU);
    }
    /// <summary>
    /// Function for retracting PR from PRL, aslo retracts TU from TC list, and inits an RL
    /// </summary>
    /// <param name="PR">Prompt request</param>
    public void RetractPromptRequest(PromptRequest PR)
    {
        PRL.Remove(PR);
        TC.removeUser(PR.TokU);
        retractionLockdown.Reset();
    }
    
    /// <summary>
    /// Function for sending dialogue to be displayed
    /// </summary>
    /// <param name="DE">Current dialogue entry listing</param>
    public void SendDialogue(DialogueEntry DE)
    {
        //If dialogue mode hasnt been entered yet, do so
        if (!DialogueButton.gameObject.activeInHierarchy)
        {
            setDialogueFlag();
            Time.timeScale = 0;
            DialogueButton.gameObject.SetActive(true);
            ActionPrompt.enabled = false;
        }
        //update dialogue elements with the new DE data
        DialogueImage.sprite = DE.SpeakerIcon;
        DialogueImage.color = DE.IconColor;
        DialogueName.text = DE.SpeakerName;
        DialogueText.text = DE.DialogueText;

    }

    /// <summary>
    /// Function for exiting dialogue mode and resuming normal game operations
    /// </summary>
    public void EndDialogue()
    {
        DialogueImage.sprite = null;
        DialogueImage.color = SC;
        DialogueButton.gameObject.SetActive(false);
        Time.timeScale = 1;
        UnsetDialogFlag();
    }
    /// <summary>
    /// Function to be used to set the nextDialogue flag
    /// </summary>
    private void SetNextDialogue(int BF)
    {
        nextDialogue = BF;
        NDLT.Reset();
    }
    /// <summary>
    /// function for checking by WaitUntil if the next dialogue entry should be sent
    /// </summary>
    /// <returns></returns>
    public int NextDialogue()
    {
        int t = nextDialogue;
        nextDialogue = 0;
        return t;
    }
    /// <summary>
    /// Function for checking if a dialogue is currently running
    /// </summary>
    /// <returns></returns>
    public bool IsDialogue()
    {
        if(DFUTT == true && DFUT == true)
        {
            isDialogue = false;
        }
        return isDialogue;
    }
    private void setDialogueFlag()
    {
        isDialogue = true;
        DFUTT = false;
    }
    private void UnsetDialogFlag()
    {
        DFUT.Reset();
        DFUTT = true;
    }
}
