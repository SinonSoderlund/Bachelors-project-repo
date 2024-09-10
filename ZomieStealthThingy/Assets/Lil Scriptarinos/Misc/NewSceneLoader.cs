using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class NewSceneLoader : MonoBehaviour
{
    [SerializeField] int targetIndex;
    [SerializeField] float fadeDuration;
    private SpriteRenderer Fader;
    
    private void Start()
    {
        if(GameObject.FindGameObjectWithTag("F2B") != null)
        Fader = GameObject.FindGameObjectWithTag("F2B").GetComponent<SpriteRenderer>    ();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == PlayerMovementScript.player.gameObject)
            StartCoroutine(F2B());
    }

    public void seceload()
    {
        SceneManager.LoadScene(targetIndex);

    }

    IEnumerator F2B()
    {
        float tcounter=0;
        Time.timeScale = 0;
        while (true)
        {
            tcounter += Time.unscaledDeltaTime;
            float m = Mathf.Lerp(0, 1, (tcounter / fadeDuration));
            Color fcol = Fader.color;
            fcol.a = m;
            Fader.color = fcol;
            yield return new WaitForEndOfFrame();
            if (tcounter > fadeDuration)
                break;
        }
        seceload();

    }
}