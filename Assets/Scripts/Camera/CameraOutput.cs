using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraOutput : MonoBehaviour {
    public RawImage cameraOutputImage;
    public WebCamTexture cameraTexture;

    void Start() {
        cameraTexture = new WebCamTexture();
        cameraOutputImage.texture = cameraTexture;
        cameraTexture.Play();
    }
}