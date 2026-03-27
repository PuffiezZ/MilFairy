using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Cutscene : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private CinemachineVirtualCamera cvc;
    
    [Header("Call Metdhod When Finish Transition")]
    public UnityEvent callEvent;

    private Coroutine coroutine;
    
    void Awake()
    {
        if (image != null)
        {
            Color startColor = image.color;
            startColor.a = 1f;
            image.color = startColor;
        }

        if (cvc != null)
        {
            // ใช้ชื่อเต็มเพื่อความชัดเจน และป้องกันการตีกับสคริปต์อื่น
            var dolly = cvc.GetCinemachineComponent<CinemachineTrackedDolly>();
            if (dolly != null)
            {
                dolly.m_PathPosition = 0f;
            }
        }
    }
    
    public void MakeTransition(float duration,float opacity,bool enableCallEvent = false)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        coroutine = StartCoroutine(TransitionCourutine(duration, opacity, enableCallEvent));
    }
    
    private IEnumerator TransitionCourutine(float duration,float opacity, bool enableCallEvent)
    {
        if (image == null) yield break;

        float elapsedTime = 0f;
        Color startColor = image.color;
        float startOpacity = startColor.a;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            startColor.a = Mathf.Lerp(startOpacity, opacity, elapsedTime / duration);
            image.color = startColor;
            yield return null;
        }

        startColor.a = opacity;
        image.color = startColor;
        
        if(enableCallEvent)
        {
            callEvent?.Invoke();
        }
    }
}
