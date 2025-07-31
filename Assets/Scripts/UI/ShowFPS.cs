using TMPro;
using UnityEngine;

namespace WaveSystem.UI
{
    public class ShowFPS : MonoBehaviour
    {
        [SerializeField] private TMP_Text fpsText;
        [SerializeField] private float updateInterval = 0.5f;

        private float deltaTime;
        private float timer;

        void Update()
        {
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
            timer += Time.unscaledDeltaTime;
        }

        void LateUpdate()
        {
            if (fpsText != null && timer >= updateInterval)
            {
                float fps = 1.0f / deltaTime;
                fpsText.text = $"FPS: {Mathf.Ceil(fps)}";
                timer = 0f;
            }
        }
    }
}