using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FPSCounter : MonoBehaviour {
    public TextMeshProUGUI label;

    private int frameCount = 0;
    private int frameRate = 0;
    private float elapsedTime = 0;
    [Range(0, 2)] public float refreshRate = 0.5f;

    [Header("Colors")] public List<float> colorFPSValues;
    public List<Color> colors;

    void Update() {
        // FPS calculation
        frameCount++;
        elapsedTime += Time.deltaTime;
        if (elapsedTime > refreshRate) {
            frameRate = (int) System.Math.Round(frameCount / elapsedTime, 1, System.MidpointRounding.AwayFromZero);
            frameCount = 0;
            elapsedTime = 0;
            updateLabel();
        }
    }

    private void updateLabel() {
        label.text = $"FPS: {frameRate}";

        for (int i = 0; i < colorFPSValues.Count; i++) {
            if (colors.Count - 1 == i || frameRate <= colorFPSValues[i] || colorFPSValues[i] <= frameRate && frameRate <= colorFPSValues[i + 1]) {
                label.color = colors[i];
                break;
            }
        }
    }
}