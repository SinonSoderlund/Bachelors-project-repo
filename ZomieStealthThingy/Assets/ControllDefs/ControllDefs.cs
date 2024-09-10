
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UtilitySpace
{


    //enums corresponding to various control variables
    public enum InputClasses {MovementClass, None};
    public enum InputType {Forward, Back, Left, Right, Sneak, Interact, PauseMenu, FireGun, DialogueNext, DialogueBack}


    public static class ControllDefs
    {
        static string none;
        public static Controll[] Controlls = new Controll[] { new Controll(none,0,0,KeyCode.None,false,false) };
    }
}