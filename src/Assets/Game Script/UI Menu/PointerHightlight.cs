using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PointerHightlight : MonoBehaviour,IPointerEnterHandler, IPointerExitHandler
{
    [Header("Input Button")]
    [SerializeField] private string nameButton;
    [SerializeField] private Sprite bgSprite;
    [SerializeField] private float hoverScale = 1.2f; 
    [SerializeField] private float duration = 0.15f; 

    [Space(5)]

    [Header("UI Elements ห้ามยุ่ง")]
    [SerializeField] private RectTransform[] highlightElement;
    [SerializeField] private TextMeshProUGUI textElements; 
    [SerializeField] private Image bgButton;

    private Vector3 originalScale;
    private Coroutine scaleRoutine;
    private void Awake()
    {
        // เก็บขนาดเริ่มต้นไว้เพื่อหดกลับให้ถูกต้อง
        originalScale = transform.localScale;
    }
    private void OnEnable()
    {
        transform.localScale = originalScale;
        LoopSetElement(false);
    }
    private void StartSmoothScale(Vector3 target)
    {
        if (scaleRoutine != null) StopCoroutine(scaleRoutine);
        scaleRoutine = StartCoroutine(ScaleProcess(target));
    }
    private IEnumerator ScaleProcess(Vector3 target)
    {
        Vector3 startScale = transform.localScale;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float percent = elapsed / duration;

            transform.localScale = Vector3.Lerp(startScale, target, Mathf.SmoothStep(0, 1, percent));
            yield return null;
        }

        transform.localScale = target;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        LoopSetElement(true);
        StartSmoothScale(originalScale * hoverScale);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        LoopSetElement(false);
        StartSmoothScale(originalScale);
    }

    private void LoopSetElement(bool setActive)
    {
        if (highlightElement.Length <= 0) return;

        for(int i = 0; i < highlightElement.Length; i++)
        {
            highlightElement[i].gameObject.SetActive(setActive);
        }
    }
    private void OnValidate()
    {
        textElements.text = nameButton;
        bgButton.sprite = bgSprite;
    }
}
