using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
public class LightMaskGenerator : MonoBehaviour
{
    [Header("Camera")]
    [Tooltip("The camera the darkness overlay will be applied to. Defaults to main camera if not specified.")]
    [SerializeField] private Camera cameraToApplyTo;
    [SerializeField] private GameObject baseOverlay;

    [Header("SpriteMasks")]
    [Tooltip("The object the light mask will be centered on and following.")]
    [SerializeField] private GameObject spriteMaskParent;
    [Tooltip("The container object acting as a parent for proper positioning and rotation of lower spritemasks.")]
    [SerializeField] private GameObject spriteMaskContainer;
    [SerializeField] private GameObject spriteMaskCircle;
    [SerializeField] private GameObject spriteMaskCone;

    [Header("Configuration")]
    [Tooltip("The size of the smallest circle that will be generated. This should probably be bigger than the size of the spriteMaskParent." +
        "This script also assumes the circle will be perfectly round, rather than oval.")]
    [SerializeField] private float minScaleCircle;
    [Tooltip("Minimum width of the cone. Cone might not scale nicely if X and Y are the same.")]
    [SerializeField] private float minScaleConeX;
    [Tooltip("Minimum length of the cone. Cone might not scale nicely if X and Y are the same.")]
    [SerializeField] private float minScaleConeY;

    [Space(8)]
    [Tooltip("The lowest allowed sorting layer allowed for the masks to use.")]
    [SerializeField] private int lowestSortingLayer;

    [Space(8)]
    [Tooltip("The number of iterations to generate. More = smoother light, more resource heavy and vice versa.")]
    [SerializeField] private int iterations;

    [HideInInspector] public bool ranYet = false;

    private float maxScaleCircle;
    private float maxScaleConeX;
    private float maxScaleConeY;
    private int highestLayer;
    private float circleScaleChangeperIteration;
    private float coneXScaleChangeperIteration;
    private float coneYScaleChangeperIteration;

    [ButtonMethod]
    private void BakeLighting()
    {
        DestroyImmediate(spriteMaskParent.GetComponentInChildren<CheckPointRegister>()?.gameObject);
        GameObject SMC = Instantiate(spriteMaskContainer);
        
        for (int i = 1; i < iterations; i++)
        {
            //generate circle
            SpriteMask objCircle = Instantiate(spriteMaskCircle).GetComponent<SpriteMask>();
            //objCircle.GetComponent<SpriteMask>().frontSortingOrder = lowestSortingLayer + i;
            //objCircle.GetComponent<SpriteMask>().backSortingOrder = lowestSortingLayer + i - 1;
            objCircle.frontSortingOrder = lowestSortingLayer + i + 2;
            objCircle.backSortingOrder = lowestSortingLayer + i - 1;
            //Debug.LogError(objCircle.frontSortingOrder + " " + objCircle.backSortingOrder);

            objCircle.transform.localScale = new Vector3(
                spriteMaskCircle.transform.localScale.x - (i * circleScaleChangeperIteration),
                spriteMaskCircle.transform.localScale.y - (i * circleScaleChangeperIteration),
                spriteMaskCircle.transform.localScale.z);
            objCircle.transform.position = spriteMaskCircle.transform.position;
            objCircle.transform.rotation = spriteMaskCircle.transform.rotation;
            objCircle.transform.parent = SMC.transform;
            //generate cone
            SpriteMask objCone = Instantiate(spriteMaskCone).GetComponent<SpriteMask>();
            //objCone.GetComponent<SpriteMask>().frontSortingOrder = lowestSortingLayer + i;
            //objCone.GetComponent<SpriteMask>().backSortingOrder = lowestSortingLayer + i - 1;
            objCone.frontSortingOrder = lowestSortingLayer + i + 2;
            objCone.backSortingOrder = lowestSortingLayer + i - 1;
            objCone.transform.localScale = new Vector3(
                spriteMaskCone.transform.localScale.x - (i * coneXScaleChangeperIteration),
                spriteMaskCone.transform.localScale.y - (i * coneYScaleChangeperIteration),
                spriteMaskCone.transform.localScale.z);
            objCone.transform.position = spriteMaskCone.transform.position;
            objCone.transform.rotation = spriteMaskCone.transform.rotation;
            objCone.transform.parent = SMC.transform;
        }

        //center lights on parent object
        SMC.transform.position = new Vector3(spriteMaskParent.transform.position.x, spriteMaskParent.transform.position.y, SMC.transform.position.z);
        SMC.transform.rotation = spriteMaskParent.transform.rotation;
        SMC.transform.parent = spriteMaskParent.transform;
        #if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(spriteMaskParent);
        #endif
    }


    // Start is called before the first frame update
    void Start()
    {
        spriteMaskContainer.SetActive(false);
        if (!ranYet)
            ranYet = true;
        else return;
        if (cameraToApplyTo == null)
        {
            cameraToApplyTo = Camera.main;
        }

        //make sure overlay is in position
        baseOverlay.transform.position = new Vector3(cameraToApplyTo.transform.position.x, cameraToApplyTo.transform.position.y, baseOverlay.transform.position.z);
        baseOverlay.transform.rotation = cameraToApplyTo.transform.rotation;



        //generate shadow overlays
        for (int i = 1; i < iterations; i++)
        {
            SpriteRenderer obj = Instantiate(baseOverlay).GetComponent<SpriteRenderer>();
            obj.sortingOrder = lowestSortingLayer + i-1;
            //Debug.LogError(obj.sortingOrder);
            obj.transform.position = baseOverlay.transform.position;
            obj.transform.rotation = baseOverlay.transform.rotation;
            obj.transform.parent = cameraToApplyTo.transform;
        }
        baseOverlay.transform.parent = cameraToApplyTo.transform; //for some reason, instantiated overlays get offset if i do this before generating the instantiated objects
        //generate spriteMasks (light)

    }

    private void OnValidate()
    {
        //Get max scale values from objects
        maxScaleCircle = spriteMaskCircle.transform.localScale.x;
        maxScaleConeX = spriteMaskCone.transform.localScale.x;
        maxScaleConeY = spriteMaskCone.transform.localScale.y;
        //Clamp values to prevent scaling wrong direction
        minScaleCircle = Mathf.Clamp(minScaleCircle, 0f, maxScaleCircle);
        minScaleConeX = Mathf.Clamp(minScaleConeX, 0f, maxScaleConeX);
        minScaleConeY = Mathf.Clamp(minScaleConeY, 0f, maxScaleConeY);
        //other setup
        highestLayer = lowestSortingLayer + iterations;
        circleScaleChangeperIteration = (maxScaleCircle - minScaleCircle) / iterations;
        coneXScaleChangeperIteration = (maxScaleConeX - minScaleConeX) / iterations;
        coneYScaleChangeperIteration = (maxScaleConeY - minScaleConeY) / iterations;
        baseOverlay.GetComponent<SpriteRenderer>().sortingOrder = lowestSortingLayer;
        spriteMaskCircle.GetComponent<SpriteMask>().frontSortingOrder = lowestSortingLayer;
        spriteMaskCircle.GetComponent<SpriteMask>().backSortingOrder = -1;
        spriteMaskCone.GetComponent<SpriteMask>().frontSortingOrder = lowestSortingLayer;
        spriteMaskCone.GetComponent<SpriteMask>().backSortingOrder = -1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
