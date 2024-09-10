using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


/// <summary>
/// Namepsace for various utilty functions
/// </summary>
namespace UtilitySpace
{

    ////enums corresponding to various control variables
    //public enum InputClasses { MovementClass, None };
    //public enum InputType {/*MovementClass*/ Forward, Back, Left, Right,/*None*/ Sneak, Interact,PauseMenu, FireGun, DialogueNext,DialogueBack }


    //enum for the zombie activity states
    public enum activityState { Chasing, Guard, Investigating, Patroll, Returning, None };
    /// <summary>
    /// Controll class for a simple and modular token keeping system
    /// </summary>
    public class TokenController
    {
        //System token
        private bool token;
        //Token return delegate, send the refrence to this token
        public delegate void TokenReturn (ref bool b);
        public TokenReturn TR;
        //token give delegate, sends ref to this token as well as the token user instance that is to receive the token
        public delegate void TokenGive(ref bool b, tokenUser Target);
        public TokenGive TG;
        /// <summary>
        /// Constructor, simplys ets the token value to true (but is also yakno, a constructor
        /// </summary>
        public TokenController()
        {
            token = true;
        }
        /// <summary>
        /// Function for adding a token user to this TC, simeply adds it to the give and return delegates
        /// </summary>
        /// <param name="TU">The token user to be added</param>
        public void addUser(tokenUser TU)
        {
            TR += TU.returnToken;
            TG += TU.tokenGet;
        }
        /// <summary>
        /// Function for removing a token user from this TC, requests token return if TU being removed has it, then unsubscribes TU from delelgates
        /// </summary>
        /// <param name="TU">The token user being rmeoved</param>
        public void removeUser(tokenUser TU)
        {
            if (TU.hasToken())
                TR(ref token);
            TR -= TU.returnToken;
            TG -= TU.tokenGet;
        }
        /// <summary>
        /// Function for giving the token to the supplied token user
        /// </summary>
        /// <param name="TU">The token user to be granted the token</param>
        public void GiveToken(tokenUser TU)
        {
            //if target has token, do nothing
            if (!TU.hasToken())
            {//if token is not available, request its return
                if (!token)
                    TR(ref token);
                //givee token to supplied TU
                TG(ref token, TU);
            }
        }
    }
    /// <summary>
    /// User class for token passing system
    /// </summary>
    public class tokenUser
    {//just realized that i in no way block a token user form being added to more than one TC which could be problematic, might fix in the future?
        //token ref
        private bool token;
        /// <summary>
        /// Connstructor
        /// </summary>
        public tokenUser()
        {
            token = false;
        }
        /// <summary>
        /// function for chekcing wether or not this tu has the token
        /// </summary>
        /// <returns></returns>
        public bool hasToken()
        {
            return token;
        }
        /// <summary>
        /// Function for requesting that this TU returns the token if they have it
        /// </summary>
        /// <param name="b">TC token refrence</param>
        public void returnToken(ref bool b)
        {
            if (token)
            {
                b = token;
                token = false;
            }
        }
        /// <summary>
        /// Function for being called by the TC TG delelgate
        /// </summary>
        /// <param name="b">TC token ref</param>
        /// <param name="TRef">Ref to the receiving TU</param>
        public void tokenGet(ref bool b, tokenUser TRef)
        {
            if(TRef == this)
            {
                token = b;
                b = false;
            }
        }

    }

    /// <summary>
    /// data holder class for dialogues
    /// </summary>
    [Serializable]
    public class DialogueEntry
    {//i feel like this is pretty self explanatory
        [SerializeField] public string SpeakerName;
        [SerializeField] public Sprite SpeakerIcon;
        [SerializeField] public Color IconColor;
        [SerializeField, TextArea] public string DialogueText;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="SN">name string</param>
        /// <param name="SI">character image</param>
        /// <param name="IC">character color</param>
        /// <param name="DT">dialogue text string</param>
        public DialogueEntry(string SN, Sprite SI, Color IC, string DT)
        {
            SpeakerName = SN;
            SpeakerIcon = SI;
            IconColor = IC;
            DialogueText = DT;
        }

    }

    /// <summary>
    /// Refrence system for keeping track of all currently existing prompts that try to get displayed, as well as token keeping system for retricting interactions with unshowed prompts
    /// </summary>
    public class PromptRequest
    {
        //ref to the transform of the requestion object, is used for sellecting prompt to be shown
        internal Transform tRef;
        //the string text to be shown as the prompt
        internal string pText;
        //Token user instance, is used ot block unshown prompts from having their corresponding interaction being triggered
        internal tokenUser TokU;
        /// <summary>
        /// Constructor for the prompt request, takes a string and a transform, creates a token user
        /// </summary>
        /// <param name="t">Transform of thsi object</param>
        /// <param name="s">Prompt text</param>
        public PromptRequest(Transform t, string s)
        {
            tRef = t;
            pText = s;
            TokU = new tokenUser();
        }

    }




    /// <summary>
    /// Timer class, duration is set once at creation
    /// </summary>
    public class Timer
    {
        //long for storing the ammount of ticks that cirrespond to the gien duration
        long ClockTime;
        //refrence ot the datetime in ticks from when the timer was created/reset
        DateTime ReferenceTime;
        /// <summary>
        /// Function for intiliaztion a Timer
        /// </summary>
        /// <param name="duration">Duration in seconds, this cannot be changed later</param>
        public Timer(double duration)
        {
            ClockTime = (long)(duration * (double)TimeSpan.TicksPerSecond);
            ReferenceTime = DateTime.Now;
        }

        private bool status(Timer x)
        {
            //Debug.Log(DateTime.Now.Ticks - x.ReferenceTime.Ticks + " " + x.ClockTime);
            if (DateTime.Now.Ticks - x.ReferenceTime.Ticks > x.ClockTime)
                return true;
            else return false;
        }

        //Boolean comapartor operators, returns true if duration has passed, else false
        public static bool operator true(Timer x) => DateTime.Now.Ticks - x.ReferenceTime.Ticks > x.ClockTime;
        public static bool operator false(Timer x) => DateTime.Now.Ticks - x.ReferenceTime.Ticks < x.ClockTime;
        //yeah im not gonna waste time trying to explain what the operators do, i feel like its somewhat self explanitory
        public static bool operator ==(Timer x, bool y) => x.status(x) == y;
        public static bool operator !=(Timer x, bool y) => !(x.status(x) == y);


        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public static bool operator &(Timer x, bool y)
            {
            if (x == true && y == true)
                return true;
            else
                return false;
            }
        public static bool operator &(bool y, Timer x)
        {
            if (x == true && y == true)
                return true;
            else
                return false;
        }
        /// <summary>
        /// Sets refrence tiem to current time, therefore restarting the countdown
        /// </summary>
        public void Reset()
        {
            ReferenceTime = DateTime.Now;
        }

    }
    /// <summary>
    /// Has color and position values
    /// </summary>
    [Serializable]
    public class ColorVisulizer
    {
        [SerializeField] public Vector2 Coordinate;
        [SerializeField] public Color PointColor;

        public ColorVisulizer(Vector2 coords, Color color)
        {
            Coordinate = coords;
            PointColor = color;
        }
    }
    /// <summary>
    /// Just a small data container
    /// </summary>
    [Serializable]
    public class RotaTime
    {
        [SerializeField] public float Delay, Degrees;
        public RotaTime()
        { }
    }
    /// <summary>
    /// Class for generating radials
    /// </summary>
    public class GridRadial
    {
        //the radials r-value and the total number of elements the eradia consists of
        internal int  RValue, RSize;
        //The quadrant points and the center value
        internal Vector2Int RLT, RRT, RRB, RLB, CV;
        /// <summary>
        /// Constructor that takes r-value and center pont and generated the quadrants and r-size
        /// </summary>
        /// <param name="CenterPoint"></param>
        /// <param name="rValue"></param>
        public GridRadial(Vector2Int CenterPoint, int rValue)
        {
            RSize = 8 * rValue;
            RValue = rValue;
            CV = CenterPoint;
            RLT = new Vector2Int(CenterPoint.x - rValue, CenterPoint.y + rValue);
            RRT = new Vector2Int(CenterPoint.x + rValue, CenterPoint.y + rValue);
            RRB = new Vector2Int(CenterPoint.x + rValue, CenterPoint.y - rValue);
            RLB = new Vector2Int(CenterPoint.x - rValue, CenterPoint.y - rValue);
        }
        /// <summary>
        /// Checkers wether or not a point is in a given radial based on qudrant values
        /// </summary>
        /// <param name="v">the vector being chekde</param>
        /// <returns></returns>
        public bool isPointInRadial(Vector2Int v)
        {
            //Faulty check to see if point is in current radian, doesnt work bcuz it doesnt rangecheck
            if (v.y == RLT.y || v.x == RRT.x || v.y == RRB.y || v.x == RLT.x)
                return true;
            else return false;
        }
        /// <summary>
        /// Gets the r-fill start and stop values from two points
        /// </summary>
        /// <param name="v1">strat point</param>
        /// <param name="v2">stop point</param>
        /// <returns></returns>
        public Vector2 GetFillStartStop(Vector2Int v1, Vector2Int v2)
        {
            Vector2 vReturn = new Vector2();
            vReturn.x = (((float)ConvertGridCoordToRadial(v1)) / ((float)RSize));
            //Debug.Log(2);
            vReturn.y = (((float)ConvertGridCoordToRadial(v2)) / ((float)RSize));
            //Debug.Log(ConvertGridCoordToRadial(v1) + " " + v1.x + " " + RLT.x);
            return vReturn;
        }
        /// <summary>
        /// function for converting 2d grid coords to 1d radial coords
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public int ConvertGridCoordToRadial(Vector2Int v)
        {//cheks per quadrant if its the correct poisiton, returns value
            if (v.y == RLT.y)
            {
                //Debug.Log(1);
                return v.x - RLT.x;
            }
            else if (v.x == RRT.x)
            {
                //Debug.Log(2);
                return (RSize / 4) + (RRT.y- v.y);
            }
            else if (v.y == RRB.y)
            {
                //Debug.Log(3);

                return (RSize / 2) + (RRB.x - v.x);
            }
            else if (v.x == RLB.x)
            {
                //Debug.Log(4);

                return (3 * (RSize / 4)) + (v.y -RLB.y);
            }
            else
            {
                //Debug.Log("blorp");
                return -1;
            }
        }
        /// <summary>
        /// Converts r-fill values to start adn stop points in r-coordinates, as well as the r-size value
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public Vector3Int GetStartStopSize(Vector2 v)
        {
            Vector3Int vReturn = new Vector3Int();
            vReturn.x =Mathf.FloorToInt(v.x * (float)RSize);
            vReturn.y = Mathf.FloorToInt(v.y * (float)RSize);
            vReturn.z = RSize;
            return vReturn;
        }
        /// <summary>
        /// Converts r-coordinates to grid coordinates
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public Vector2Int GetRadialCoordinatesToGrid(int x)
        {
            if (x*4 < RSize)
            {
                //Debug.Log(x + " " +1 + " " + RSize);
                //Debug.Log(new Vector2Int(RLT.x + x, RLT.y));
                return new Vector2Int(RLT.x + x, RLT.y);
            }
            else if (x * 2 < RSize)
            {
                //Debug.Log(x + " " +2 + " " + RSize);

                x -= (RSize / 4);
                return new Vector2Int(RRT.x, RRT.y-x);
            }
            else if ((x/4)*3 < RSize)
            {
                //Debug.Log(x + " " + 3 + " " + RSize);
                x -= (RSize / 2);
                return new Vector2Int(RRB.x-x, RRB.y);
            }
            else
            {
                //Debug.Log(x + " " + 4 + " "  + RSize);
                x -= ((RSize / 4) * 3);
                return new Vector2Int(RLB.x, RLB.y+x);
            }
        }

    }

}
