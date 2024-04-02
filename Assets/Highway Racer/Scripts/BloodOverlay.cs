using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BloodOverlay : MonoBehaviour
{
    public Image bloodOverlayImage;
    public float fadeOutDuration = 1.5f;
    public float displayDuration = 2f;
    private Coroutine currentFadeCoroutine;

    public void ShowBloodOverlay()
    {
        if (currentFadeCoroutine != null)
        {
            StopCoroutine(currentFadeCoroutine);
        }

        currentFadeCoroutine = StartCoroutine(FadeOut(bloodOverlayImage));
    }

    IEnumerator FadeOut(Image image)
    {
        Color color = image.color;
        float timer = 0f;

        while (timer < fadeOutDuration)
        {
            timer += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, timer / fadeOutDuration);
            image.color = color;
            yield return null;
        }
    }
}