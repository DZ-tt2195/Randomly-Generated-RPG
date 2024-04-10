using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PointsVisual : MonoBehaviour
{
    [SerializeField] TMP_Text textbox;

    private void Start()
    {
        transform.localScale = Vector3.zero; //start off invisible
    }

    public void Setup(string text, Vector3 position)
    {
        //initial information 
        this.transform.localPosition = position + new Vector3(0, 0, -1);
        textbox.text = text;
        StartCoroutine(ExpandContract());
    }

    IEnumerator ExpandContract()
    {
        //largest size this visual will be 
        Vector2 maxSize = new Vector3(1.5f, 1.5f, 1);
        float elapsedTime = 0f;
        float waitTime = PlayerPrefs.GetFloat("Animation Speed");

        while (elapsedTime < waitTime) //expand until it reaches the max size
        {
            transform.localScale = Vector3.Lerp(Vector3.zero, maxSize, elapsedTime / waitTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        elapsedTime = 0f;

        while (elapsedTime < waitTime) //shrink until it reaches 0
        {
            transform.localScale = Vector3.Lerp(maxSize, Vector3.zero, elapsedTime / waitTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(this.gameObject);
    }
}
