    using System.Collections;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    public class LoadingScreenView : MonoBehaviour
    {
        public GameObject loadingScreenPanel;
        public TextMeshProUGUI loadingScreenText;
        public Slider loadingScreenSlider;

        public void Awake()
        {
            loadingScreenPanel.SetActive(true);
            MapController.percentMapGenerated += UpdateLoadingScreenText; // Subscribe to the percentMapGenerated event
            
        }
        public void UpdateLoadingScreenText(float percentGenerated)
        {
            
            if((percentGenerated*100) >= 99)
            {
                StartCoroutine(FadeOutScreen());// Hide the loading screen panel when the map generation is complete
            }
            loadingScreenText.text = "MAP GENERATING\n" + (percentGenerated*100).ToString("F0") + "% DONE"; // Update the loading screen text with the current percentage
            loadingScreenSlider.value = percentGenerated; // Update the loading screen slider value with the current percentage
            // Show the loading screen panel while the map is being generated
                  
        }

        public IEnumerator FadeOutScreen(){
            float fadeDuration = 2f; // Duration of the fade out effect
            float startAlpha = loadingScreenPanel.GetComponent<CanvasGroup>().alpha; // Get the starting alpha value of the loading screen panel
            float endAlpha = 0f; // Target alpha value for the fade out effect

            for (float t = 0; t < fadeDuration; t += Time.unscaledDeltaTime)
            {
                float normalizedTime = t / fadeDuration; // Normalize the time for the fade out effect
                loadingScreenPanel.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(startAlpha, endAlpha, normalizedTime);
                if(loadingScreenPanel.GetComponent<CanvasGroup>().alpha > 0.01f){
                    yield return null;
                }
                // yield return null; // Wait for the next frame
            }
             // Set the final alpha value to ensure it is fully transparent
            loadingScreenPanel.SetActive(false); // Hide the loading screen panel after the fade out effect is complete
        }
    }