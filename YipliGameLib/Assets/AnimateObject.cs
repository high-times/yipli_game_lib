using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateObject : MonoBehaviour
{
    // Start is called before the first frame update
    private void start()
    {
        StartCoroutine(ScaleUpDownAnimation());
    }

    IEnumerator ScaleUpDownAnimation()
    {
        //Animate the scale up down
        gameObject.transform.localScale *= 1.25f;
        yield return new WaitForSecondsRealtime(.25f);
        gameObject.transform.localScale *= 0.8f;
        yield return new WaitForSecondsRealtime(0.25f);
        gameObject.transform.localScale *= 0.8f;
        yield return new WaitForSecondsRealtime(0.25f);
        gameObject.transform.localScale *= 1.25f;
        yield return new WaitForSecondsRealtime(0.25f);
    }
}
