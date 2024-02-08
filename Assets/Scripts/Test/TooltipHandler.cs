using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TooltipHandler : MonoBehaviour
{
    [SerializeField] private List<TooltipInfos> tooltipContentList;

    [SerializeField] private GameObject tooltipContainer;
    private TMP_Text _tooltipDescriptionTMP;

    private void Awake()
    {
        _tooltipDescriptionTMP = tooltipContainer.GetComponentInChildren<TMP_Text>();
    }

    private void OnEnable()
    {
        LinkHandler.OnHoverOnLinkEvent += GetTooltipInfo;
        LinkHandler.OnCloseTooltipEvent += CloseTooltip;
    }

    private void OnDisable()
    {
        LinkHandler.OnHoverOnLinkEvent -= GetTooltipInfo;
        LinkHandler.OnCloseTooltipEvent -= CloseTooltip;
    }

    private void GetTooltipInfo(string keyword, Vector3 mousePos)
    {
        foreach (var entry in tooltipContentList)
        {
            if (entry.Keyword == keyword)
            {
                if (!tooltipContainer.activeInHierarchy)
                {
                    tooltipContainer.transform.position = mousePos + new Vector3(-400, 0, 0); // This offset is an example, you'll probably need to find your own best fitting value.
                    tooltipContainer.SetActive(true);
                }

                _tooltipDescriptionTMP.text = entry.Description;
                return;
            }
        }

        Debug.Log($"Keyword: {keyword} not found");
    }

    public void CloseTooltip()
    {
        if (tooltipContainer.activeInHierarchy)
            tooltipContainer.SetActive(false);
    }
}