using UnityEngine;
using UnityEngine.UI;

namespace OpenCvSharp.Demo {
    public class HandTrackingManager : MonoBehaviour {
        public CameraOutput cameraOutput;
        public OpenCVView openCVView;

        void Update() {
            if (cameraOutput != null) {
                Mat mat = Unity.TextureToMat(cameraOutput.cameraTexture);
                Mat outputMat = new Mat();
                Cv2.CvtColor(mat, outputMat, ColorConversionCodes.BGR2GRAY);
                Cv2.Erode(mat, outputMat, new Mat(), null, 10, BorderTypes.Isolated);
                // Cv2.Sub
                Texture2D texture = Unity.MatToTexture(outputMat);
                openCVView.outputTexture.texture = texture;
            }
        }
    }
}