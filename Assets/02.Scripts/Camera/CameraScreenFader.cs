using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// 시점 전환 시 화면을 검게 덮었다가 걷어내는 페이드 연출을 담당한다.
public class CameraScreenFader
{
    private CanvasGroup _fadeCanvasGroup;

    public CameraScreenFader(Transform parent)
    {
        CreateFadeCanvas(parent);
    }

    // 페이드 아웃 → onMidpoint 호출 → 페이드 인 순서로 진행한다.
    public IEnumerator Fade(float duration, Action onMidpoint)
    {
        float halfDuration = duration * 0.5f;
        float timer = 0f;

        while (timer < halfDuration)
        {
            timer += Time.deltaTime;
            SetAlpha(Mathf.Clamp01(timer / halfDuration));
            yield return null;
        }

        SetAlpha(1f);
        onMidpoint?.Invoke();

        timer = 0f;
        while (timer < halfDuration)
        {
            timer += Time.deltaTime;
            SetAlpha(1f - Mathf.Clamp01(timer / halfDuration));
            yield return null;
        }

        SetAlpha(0f);
    }

    private void CreateFadeCanvas(Transform parent)
    {
        GameObject canvasObject = new GameObject("FadeCanvas");
        canvasObject.transform.SetParent(parent, false);

        Canvas fadeCanvas = canvasObject.AddComponent<Canvas>();
        fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        fadeCanvas.sortingOrder = 999;

        canvasObject.AddComponent<CanvasScaler>();

        _fadeCanvasGroup = canvasObject.AddComponent<CanvasGroup>();
        _fadeCanvasGroup.alpha = 0f;
        _fadeCanvasGroup.blocksRaycasts = false;

        GameObject imageObject = new GameObject("FadeImage");
        imageObject.transform.SetParent(canvasObject.transform, false);

        Image fadeImage = imageObject.AddComponent<Image>();
        fadeImage.color = Color.black;
        fadeImage.raycastTarget = false;

        RectTransform rectTransform = fadeImage.rectTransform;
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
    }

    private void SetAlpha(float alpha)
    {
        if (_fadeCanvasGroup != null)
        {
            _fadeCanvasGroup.alpha = alpha;
        }
    }
}
