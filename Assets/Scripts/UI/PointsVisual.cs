using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MyBox;

public class PointsVisual : MonoBehaviour
{
    [SerializeField] TMP_Text textbox;

    public void Setup(string text, Vector3 position, float duration, int topSize, Color textColor)
    {
        this.transform.localPosition = position;
        this.transform.SetAsLastSibling();
        textbox.text = KeywordTooltip.instance.EditText(text);
        textbox.color = textColor;
        
        transform.localScale = Vector3.zero;
        this.gameObject.SetActive(true);
        StartCoroutine(ExpandContract());

        IEnumerator ExpandContract()
        {
            Vector2 maxSize = (topSize <= 2) ? new(2, 2): new(topSize, topSize);
            float elapsedTime = 0f;
            float waitTime = duration / 2;

            while (elapsedTime < waitTime)
            {
                transform.localScale = Vector3.Lerp(Vector3.zero, maxSize, elapsedTime / waitTime);
                elapsedTime += Time.deltaTime;
                transform.SetAsLastSibling();
                yield return null;
            }

            elapsedTime = 0f;

            while (elapsedTime < waitTime)
            {
                transform.localScale = Vector3.Lerp(maxSize, Vector3.zero, elapsedTime / waitTime);
                elapsedTime += Time.deltaTime;
                transform.SetAsLastSibling();
                yield return null;
            }
            TurnManager.inst.ReturnVisual(this);
        }
    }
}

