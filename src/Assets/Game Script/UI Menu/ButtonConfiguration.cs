using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonConfiguration : MonoBehaviour
{
    [SerializeField] private bool scaleWidthByParentLayout = false;

    [Header("Button References")]
    [SerializeField] ContentSizeFitter csf;
    [SerializeField] private LayoutGroup layoutGroup;

    private void OnValidate()
    {
        csf.enabled = !scaleWidthByParentLayout;
        csf.horizontalFit = scaleWidthByParentLayout ? ContentSizeFitter.FitMode.Unconstrained : ContentSizeFitter.FitMode.PreferredSize;

        // สั่งให้ Layout Rebuild ทันทีเพื่อให้เห็นผลในหน้า Scene
        LayoutRebuilder.MarkLayoutForRebuild(transform as RectTransform);
    }
}
