using System.Collections;
using UnityEngine;
using UnityEngine.UI; // Use TMPro if you are using TextMeshPro instead

public class LoadingAnimation : MonoBehaviour
{
    public Text loadingText; // Drag your Text or TextMeshPro component here
    private string baseText = "Carregando";
    private int dotCount = 0;

    private void Start()
    {
        if (loadingText == null)
        {
            loadingText = GetComponent<Text>();
        }

        StartCoroutine(AnimateLoadingText());
    }

    private IEnumerator AnimateLoadingText()
    {
        while (true)
        {
            // Update text with base text and a variable number of dots
            loadingText.text = baseText + new string('.', dotCount);

            // Increment dot count (0, 1, 2, 3)
            dotCount = (dotCount + 1) % 4; // Resets to 0 after 3

            // Wait for 0.5 seconds before the next update
            yield return new WaitForSeconds(0.5f);
        }
    }
}
