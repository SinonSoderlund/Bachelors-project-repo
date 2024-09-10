using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Namespace for helper classes relating to the player
/// </summary>
namespace PlayerClasses
{
    /// <summary>
    /// Class for storing frame-to-frame movement data
    /// </summary>
    public class MovementManagerObject
    {
        //movemnt axis store variables
        private int MoveX, MoveY;
        //string used to build an animation key
        private string MoveCombinatorial;
        //value for keeping track of distance mvoed
        private float DMoved;
        /// <summary>
        /// Initilizes values to 0/empty
        /// </summary>
        public MovementManagerObject()
        {
            MoveX = 0;
            MoveY = 0;
            MoveCombinatorial = "";
        }
        /// <summary>
        /// Copy costrcutor that carries over DMoved
        /// </summary>
        /// <param name="Old">previous MMO</param>
        public MovementManagerObject(MovementManagerObject Old)
        {
            DMoved = Old.DMoved;
        }

        /// <summary>
        /// Function for accesing axis movement variables, 1 is x-axis, 2 is y-axis
        /// </summary>
        /// <param name="i">the axis to be accessed</param>
        /// <returns></returns>
        public int this[int i]
        {

            get
            {
                if (i == 1)
                    return MoveX;
                if (i == 2)
                    return MoveY;
                else
                    return 0;
            }
            set
            {
                if (i == 1)
                    MoveX = value;
                if (i == 2)
                    MoveY = value;
            }

        }
        /// <summary>
        /// Function for accessing the animation combinatorial, 3 is for access
        /// </summary>
        /// <param name="i">just insert 3</param>
        /// <returns></returns>
        public string this[long i]
        {

            get
            {
                if (i == 3)
                    return MoveCombinatorial;
                else
                    return null;
            }
            set
            {
                if (i == 3)
                    MoveCombinatorial = value;
            }

        }

        /// <summary>
        /// Function for turning movement data into a normalized vector 3
        /// </summary>
        /// <returns></returns>
        public Vector3 VectorAssembly(float SpeedMod)
        {
            Vector2 v = (new Vector2(MoveX, MoveY).normalized) * Time.deltaTime * SpeedMod;
            DMoved += v.magnitude;
            return v;
        }
        /// <summary>
        /// Evalues if DMoved is greater than target, if so resets DMoved, returns true
        /// </summary>
        /// <param name="target">target value</param>
        /// <returns></returns>
        public bool EvaluatedMovedDist(float target)
        {
            if (DMoved > target)
            {
                //Debug.Log(DMoved);
                DMoved = 0;
                return true;
            }
            else return false;
        }

        /// <summary>
        /// Function the chekc wether or not there is a currently enscribed move value
        /// </summary>
        /// <returns></returns>
        public bool MoveCheck()
        {
            return (MoveX.Abs() + MoveY.Abs() != 0);
        }

    }
    
    

}