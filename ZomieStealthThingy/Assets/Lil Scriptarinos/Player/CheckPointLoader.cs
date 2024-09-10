using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class CheckPointLoader : MonoBehaviour
{
    // Start is called before the first frame update

   // [SerializeField] GameObject PlayerPrefab;
   //Static ref to this
    internal static CheckPointLoader CPL;
    //ref to world
    GameObject WorldRef;
    //ref to world copy
    GameObject lastCheckpointState;
    bool isBlock = true;
    private bool initCheckSet = true;
    //Ref ti death UI
    [SerializeField] GameObject DeathCanvas;
    //ref to reload button
    [SerializeField] Button Reload;
    private void Awake()
    {//sets static ref or delete this
        if (CPL == null)
            CPL = this;
        else
            Destroy(gameObject);

        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    }

    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (arg0.buildIndex == 0 || arg0.buildIndex == 5)
            return;
        WorldRef = GameObject.FindGameObjectWithTag("World");
        initCheckSet = false;
        Time.timeScale = 1;
       // OnCheckPoint();
    }

    public void couply()
    {
        isBlock = false;
    }

    //If player dies, delete current world, activate death ui and add button listener
    public void DeathTime()
    {
        //Time.timeScale = 0;
        Destroy(WorldRef);
        DeathCanvas.SetActive(true);
        Reload.onClick.AddListener(delegate { RecreateWorld(); });
    }
    //set old world as cope of saved world, restart game state, unload death ui setup
    public void RecreateWorld()
    {
        WorldRef = Instantiate(lastCheckpointState);
        WorldRef.SetActive(true);
        DeathCanvas.SetActive(false);
        Time.timeScale = 1;
        Reload.onClick.RemoveAllListeners();
        //if (AudioCoupler.audioCoupler.bloky && !isBlock)
        //    AudioCoupler.audioCoupler.swapBlock();
    }
    //create new world copy
    public void OnCheckPoint()
    {
        Destroy(lastCheckpointState);
        lastCheckpointState = Instantiate(WorldRef);
        lastCheckpointState.SetActive(false);
    }


    private void LateUpdate()
    {
        if(!initCheckSet)
        {
            initCheckSet = true;
            OnCheckPoint();
        }
    }
}
