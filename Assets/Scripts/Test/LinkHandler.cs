using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class LinkHandler : MonoBehaviour
{
    private TMP_Text _tmpTextBox;
    private Canvas _canvasToCheck;
    private Camera _cameraToUse;
    private RectTransform _textBoxRectTransform;

    private int _currentlyActiveLinkedElement;

    public delegate void HoverOnLinkEvent(string keyword, Vector3 mousePos);
    public static event HoverOnLinkEvent OnHoverOnLinkEvent;

    public delegate void CloseTooltipEvent();
    public static event CloseTooltipEvent OnCloseTooltipEvent;

    private void Awake()
    {
        _tmpTextBox = GetComponent<TMP_Text>();
        _canvasToCheck = GetComponentInParent<Canvas>();
        _textBoxRectTransform = GetComponent<RectTransform>();

        if (_canvasToCheck.renderMode == RenderMode.ScreenSpaceOverlay)
            _cameraToUse = null;
        else
            _cameraToUse = _canvasToCheck.worldCamera;
    }

    private void Update()
    {
        CheckForLinkAtMousePosition();
    }

    private void CheckForLinkAtMousePosition()
    {
        Vector3 mousePosition = new(Input.mousePosition.x, Input.mousePosition.y, 0);

        if (TMP_TextUtilities.IsIntersectingRectTransform(_textBoxRectTransform, mousePosition, _cameraToUse))
        {
            int intersectingLink = TMP_TextUtilities.FindIntersectingLink(_tmpTextBox, mousePosition, _cameraToUse);

            if (_currentlyActiveLinkedElement != intersectingLink)
                OnCloseTooltipEvent?.Invoke();

            if (intersectingLink == -1)
                return;

            TMP_LinkInfo linkInfo = _tmpTextBox.textInfo.linkInfo[intersectingLink];

            OnHoverOnLinkEvent?.Invoke(linkInfo.GetLinkID(), mousePosition);
            _currentlyActiveLinkedElement = intersectingLink;
        }
    }
}
