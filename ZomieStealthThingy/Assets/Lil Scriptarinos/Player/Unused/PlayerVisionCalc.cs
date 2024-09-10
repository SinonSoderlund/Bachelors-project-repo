using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UtilitySpace;
//Unused atm because it simply doesnt work
public class PlayerVisionCalc : MonoBehaviour
{
    //value relating to player vision and image resolution
    [SerializeField, Range(0,360)] private float playerFoV;
    [SerializeField,Range(0,20)] private float playerVisionRange;
    [SerializeField, Range(0.001f, 1)] private float UnitsPerPixel;
    //texture and sprite to be created, ref to sprite mask
    Texture2D T2D;
    Sprite NS;
    [SerializeField] SpriteMask spriteMask;
    //various othe rvariables
    private int TextureSize;
    private Vector2Int centerPoint; Timer t;
    // Start is called before the first frame update
    void Start()
    {
        //generates texture, converts it to a sprite and applies it to the sprtiemask
        onFoVUpdate();
        NS = Sprite.Create(T2D, new Rect(0, 0, TextureSize, TextureSize), new Vector2(0.5f, 0.5f), 1/UnitsPerPixel);
        spriteMask.sprite = NS;
        t = new Timer(1);
    }

   // Update is called once per frame
    void Update()
    {
        if (t)
        {//same aas start but fires once every second
            t.Reset();
            onFoVUpdate();
            NS = Sprite.Create(T2D, new Rect(0, 0, TextureSize, TextureSize), new Vector2(0.5f, 0.5f), 1/UnitsPerPixel);
            spriteMask.sprite = NS;
        }
    }


    public void onFoVUpdate()
    {
        //calculate image size based on PVR and UPP
        TextureSize = (int)(1 / UnitsPerPixel * playerVisionRange);
        //if image size is even, increase by 1
        if (TextureSize % 2 == 0)
            TextureSize++;
        //create new texture2d of size 1x1. set color to transparant, upscale to desired resolution (seems like the easiest way to get wholle transparent background)
        T2D = new Texture2D(1, 1);
        T2D.SetPixel(0, 0, new Color(0, 0, 0, 0));
        T2D.Apply();
        T2D.Resize(TextureSize, TextureSize);
        //calculate center point
        int center = (TextureSize / 2);
        centerPoint = new Vector2Int(center, center);
        //calculate the two extremes of the vision cone, then get the r-value (how many radials/"shells/layers" away from the center are the points)
        Vector2Int p1 = new Vector2Int(Mathf.CeilToInt((Mathf.Sin(-playerFoV / 2) * Mathf.Deg2Rad) * TextureSize), Mathf.CeilToInt(Mathf.Cos((-playerFoV / 2) * Mathf.Deg2Rad) * TextureSize))+centerPoint;
        Vector2Int p2 = new Vector2Int(Mathf.CeilToInt(Mathf.Sin((playerFoV / 2) * Mathf.Deg2Rad) * TextureSize), Mathf.CeilToInt(Mathf.Cos((playerFoV / 2) * Mathf.Deg2Rad) * TextureSize))+centerPoint;
        int rStart = getRValue(p1, p2);
        //print(p1 + " " + p2 + " "  + centerPoint + " " + rStart +" " + TextureSize);
        //generate the radial of the corresponding r-value, then get the decimal r-fill value, ie how far along the radial are the two points, in decimal from 0-1, measured from start position in top left corner of the radial
        GridRadial GR = new GridRadial(centerPoint, rStart);
        Vector2 StrStp = GR.GetFillStartStop(p1, p2);
        //print(StrStp);
        //loop sthrough every radial down to 1
        for (int i = rStart; i > 0; i--)
        {
            //unless this is the start loop through, generate new radial
            //print(i);
            if (i != rStart)
                GR = new GridRadial(centerPoint, i);
            //get start, stop and size values for current radial
            Vector3Int S3 = GR.GetStartStopSize(StrStp);
            //print(S3);
            //everything taht refrences m is infinite llop prevention becuase this shit is broken as hell
            int m = 0;
            //loop through radial from start to stop
            for(int x = S3.x; x != S3.y+1 % S3.z; x = ((x+1)%S3.z))
            {
                //print(x + " " + S3.z);
                m++;
                if (m > S3.z)
                {
                    print(rStart + " " + S3);
                    return;
                }
                //convert 1 dimensional r-coordinate for current r-value to 2 dimsensional grid coordinate
                Vector2Int GridCoords = GR.GetRadialCoordinatesToGrid(x);
                //paint in the current pixel as comepletely non-transparant
                T2D.SetPixel(GridCoords.x, GridCoords.y, new Color(0, 0, 0, 255));
            }
        }
        //set centerpoint as non-transparent, then apply chnages
        T2D.SetPixel(centerPoint.x,centerPoint.y, new Color(0, 0, 0, 255));
        //T2D.SetPixel(centerPoint.x, 0, new Color(0, 0, 0, 255));
        T2D.Apply();
    }
    //function for getting the r-value from the two coordinate points
    public int getRValue(Vector2Int o, Vector2Int t)
    {
        //if x and y of one of the points is the same then it sits on a diagonal, a diagonal point constitutes the start of a radial qudrant and is unique for the the radial
        //else run quadrant check
        if (o.y == o.x || t.y == t.x)
            return Mathf.Abs(t.y - centerPoint.y);
        else return (quadrantCheck(o));

    }

    public int quadrantCheck(Vector2Int v)
    {
        //generates a radial based onthe vectors x-value, checks if point exists within any of a radials quadrants
        //just realized taht the implimentation is broken bcuz a posiiton of a more external radial moving across the x-axis will register on the -yaxis of a more intenal radial bcuzz i dont do range checking
        int rTrial = Mathf.Abs(v.x - centerPoint.x);
        GridRadial RT = new GridRadial(centerPoint, rTrial);
        if (RT.isPointInRadial(v))
            return rTrial;
        else return Mathf.Abs(v.y - centerPoint.y);
    }

}
