using UnityEngine;
using UnityEngine.UI;

namespace OpenCvSharp.Demo {
    public class CameraOutput : MonoBehaviour {
        [Header("Target Area")] public Rect targetArea;
        public Vector2 targetAreaPercentSize = new Vector2(0.25f, 0.25f);
        public Color targetAreaColor;

        public RawImage cameraOutputImage;
        public WebCamTexture cameraTexture;

        void Start() {
            cameraTexture = new WebCamTexture();
            cameraTexture.Play();
            cameraOutputImage.texture = cameraTexture;
            setTargetArea();
        }

        private void Update() {
            // Mat inputTexture = Unity.TextureToMat(cameraTexture);
            // cameraOutputImage.texture = addTargetAreaRect(inputTexture);
            // inputTexture.Release();
            // inputTexture = null;
        }

        private void setTargetArea() {
            float targetAreaWidth = cameraTexture.width * targetAreaPercentSize.x;
            float targetAreaHeight = cameraTexture.height * targetAreaPercentSize.y;
            targetArea = new Rect(new Point((cameraTexture.width / 2) - targetAreaWidth * 0.5, (cameraTexture.height / 2) - targetAreaHeight * 0.5),
                new Size(cameraTexture.width * targetAreaPercentSize.x, cameraTexture.height * targetAreaPercentSize.y));
        }

        private Texture addTargetAreaRect(Mat inputMat) {
            Cv2.Rectangle(inputMat, targetArea.TopLeft, targetArea.BottomRight, new Scalar(0, 255, 0), 5);
            Texture outputTexture = Unity.MatToTexture(inputMat);
            inputMat.Release();
            inputMat = null;
            return outputTexture;
        }
    }
}