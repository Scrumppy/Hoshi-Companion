using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI fpsText;
    [SerializeField] private TextMeshProUGUI drawCallsText;

    private float deltaTime;

    private void Start()
    {
        Application.targetFrameRate = 75;
        InvokeRepeating(nameof(CalculateFPS), 0, 1f);
    }

   private void CalculateFPS()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        fps = Mathf.Clamp(fps, 0, Application.targetFrameRate);

        fpsText.text = $"{fps:0.} FPS";
    }
}
