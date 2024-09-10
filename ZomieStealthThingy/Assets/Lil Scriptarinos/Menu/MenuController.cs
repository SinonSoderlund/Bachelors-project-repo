using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UtilitySpace;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    //various UI refrences
    [SerializeField] private Button StartGame;
    [SerializeField] private Button StartGun;

    [SerializeField] private int GameSceneIndex;
    [SerializeField] private Button Settings;
    [SerializeField] private GameObject Parent;
    [SerializeField] private GameObject MainScreen;
    [SerializeField] private GameObject SettingScreen;  
    [SerializeField] private Button QuitGame;

    [SerializeField] private Button backToMain;
    [SerializeField] private GameObject BMH;
    [SerializeField] private Button L1, L2, L3, BM;
    // Start is called before the first frame update
    void Start()
    {
        //Just ties the buttons together with various event delegates, i dont feel like i really have to explain this stuff in detail
        StartGame.onClick.AddListener(delegate { InputManagement.InputManager.setGun(true); BMH.SetActive(true); });
        StartGun.onClick.AddListener(delegate { InputManagement.InputManager.setGun(false); BMH.SetActive(true);  });
        Settings.onClick.AddListener(delegate { Parent.transform.position -= (SettingScreen.transform.position- MainScreen.transform.position); });
        QuitGame.onClick.AddListener(delegate { Application.Quit(); });
        backToMain.onClick.AddListener(delegate { Parent.transform.position += (SettingScreen.transform.position - MainScreen.transform.position); });
        L1.onClick.AddListener(delegate { SceneManager.LoadScene(GameSceneIndex); });
        L2.onClick.AddListener(delegate { SceneManager.LoadScene(GameSceneIndex+2); });
        L3.onClick.AddListener(delegate { SceneManager.LoadScene(GameSceneIndex+3); });
        BM.onClick.AddListener(delegate { BMH.SetActive(false); });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
