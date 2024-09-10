#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;
public class ControlGenerator : MonoBehaviour
{

    [Header("Definitions")]
    [SerializeField, Tooltip("Starts with 'Assets', as the '[YourProjectNameHere]' folder is root")] string DefinesTextPath = "Assets/ControllDefs/ControlsDefs.txt";
    [SerializeField, Tooltip("Starts with '/[YourFolderHere]', as the 'Assets' folder is root")] string DefinesCsPath = "/ControllDefs/ControllDefs.cs";
    [SerializeField] string[] InputClasses = new string[] { "MovementClass", "None"  };
    [SerializeField] string[] InputType = new string[] {/*MovementClass*/ "Forward", "Back", "Left", "Right",/*None*/ "Sneak", "Interact", "PauseMenu", "FireGun", "DialogueNext", "DialogueBack"  };

    [MyBox.ButtonMethod]
    private void PrintFilePaths()
    {
        string ApplicationPath = Application.dataPath;
        Debug.Log("CS file path: " + ApplicationPath + DefinesCsPath);
        ApplicationPath = ApplicationPath.Remove(ApplicationPath.Length - 6, 6);
        Debug.Log("Text file path: " + ApplicationPath + DefinesTextPath);
        Debug.Log(NewControllArray[0].ReduceToString());
    }

    [MyBox.ButtonMethod]
    private void GenerateDefinition()
    {
        string StringTypeClasses = "";
        if (InputClasses != null)
        {
            StringTypeClasses = InputClasses[0];
            for (int i = 1; i < InputClasses.Length; i++)
                StringTypeClasses += ", " + InputClasses[i];
        }
        else return;
        string StringTypeInput = "";
        if (InputType != null)
        {
            StringTypeInput = InputType[0];
            for (int i = 1; i < InputType.Length; i++)
                StringTypeInput += ", " + InputType[i];
        }
        else return;

        string Content = (AssetDatabase.LoadAssetAtPath(DefinesTextPath, typeof(TextAsset)) as TextAsset).text;
        if (Content != null)
        {
            Content = Content.Replace(" InsertClass ", StringTypeClasses);
            Content = Content.Replace(" InsertType ", StringTypeInput);
            using (StreamWriter Writer = new StreamWriter(Application.dataPath + DefinesCsPath)) { Writer.Write(Content); }
            AssetDatabase.Refresh();
        }
        else Debug.LogError("No such file");
    }

    [SerializeField] Controll[] NewControllArray;

    [MyBox.ButtonMethod]
    private void SetControlls()
    {
        string StringNCA;
        StringNCA = NewControllArray[0].ReduceToString();


        string StringTypeClasses = "";
        if (InputClasses != null)
        {
            StringTypeClasses = InputClasses[0];
            for (int i = 1; i < InputClasses.Length; i++)
                StringTypeClasses += ", " + InputClasses[i];
        }
        else return;
        string StringTypeInput = "";
        if (InputType != null)
        {
            StringTypeInput = InputType[0];
            for (int i = 1; i < InputType.Length; i++)
                StringTypeInput += ", " + InputType[i];
        }
        else return;
        //Regex.Replace()
        string Content = (AssetDatabase.LoadAssetAtPath( DefinesCsPath, typeof(TextAsset)) as TextAsset).text;
        if (Content != null)
        {
            Content = Content.Replace(" new Controll(none,0,0,KeyCode.None,false,false) ", StringNCA);
            using (StreamWriter Writer = new StreamWriter(Application.dataPath + DefinesCsPath)) { Writer.Write(Content); }
            AssetDatabase.Refresh();
        }
        else Debug.LogError("No such file");
    }
}
#endif