using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
public class CircleLight : MonoBehaviour
{
    [Header("Objects")]
    [Tooltip("If the light is supposed to be attached to and follow another object, this is where you specify that object.")]
    [SerializeField] private GameObject parentObject;
    [Tooltip("The sprite mask to use as base object for the light.")]
    [SerializeField] private GameObject baseLight;

    [Header("Configuration")]
    [Tooltip("The size of the smallest sprite that will be generated. This script  assumes a circular spritemask for the lightsource.")]
    [SerializeField] private float minScale;

    [Space(8)]
    [Tooltip("The lowest allowed sorting layer allowed for the masks to use.")]
    [SerializeField] private int lowestSortingLayer;

    [Space(8)]
    [Tooltip("The number of iterations to generate. More = smoother light, more resource heavy and vice versa.")]
    [SerializeField] private int iterations;

    private float maxScale;
    private float scaleChangeperIteration;

    // Start is called before the first frame update
    [ButtonMethod]
    void BakeLighitng()
    {
        SpriteMask[] DL = GetComponentsInChildren<SpriteMask>();
        for(int i = 0; i < DL.Length; i++)
        {
            if (DL[i].gameObject != baseLight)
                DestroyImmediate(DL[i].gameObject);
        }

        //generate spriteMasks (light)
        for (int i = 1; i < iterations; i++)
        {
            //generate circle
            GameObject objCircle = Instantiate(baseLight);
            objCircle.transform.position = new Vector3(baseLight.transform.position.x, baseLight.transform.position.y, baseLight.transform.position.z);
            objCircle.GetComponent<SpriteMask>().frontSortingOrder = lowestSortingLayer + i;
            objCircle.GetComponent<SpriteMask>().backSortingOrder = lowestSortingLayer + i - 1;
            objCircle.transform.localScale = new Vector3(
                baseLight.transform.localScale.x - (i * scaleChangeperIteration),
                baseLight.transform.localScale.y - (i * scaleChangeperIteration),
                baseLight.transform.localScale.z);
            objCircle.transform.parent = this.transform;
        }

        if (parentObject != null)
        {
            this.transform.position = new Vector3(parentObject.transform.position.x, parentObject.transform.position.y, this.transform.position.z);
            this.transform.rotation = parentObject.transform.rotation;
            this.transform.parent = parentObject.transform;
        }
        #if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(gameObject);
        #endif
    }

    private void OnValidate()
    {
        //Get max scale value
        maxScale = baseLight.transform.localScale.x;
        //Clamp values to prevent scaling wrong direction
        minScale = Mathf.Clamp(minScale, 0f, maxScale);
        //other
        scaleChangeperIteration = (maxScale - minScale) / iterations;
        baseLight.GetComponent<SpriteMask>().frontSortingOrder = lowestSortingLayer;
        baseLight.GetComponent<SpriteMask>().backSortingOrder = -1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
