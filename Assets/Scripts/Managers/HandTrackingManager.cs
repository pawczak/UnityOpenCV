using UnityEngine;
using UnityEngine.UI;

namespace OpenCvSharp.Demo {
    public class HandTrackingManager : MonoBehaviour {
        public CameraOutput cameraOutput;
        public OpenCVView openCVView;

        private BackgroundSubtractor backgroundSubtractor;

        void Start() {
            // backgroundSubtractor = BackgroundSubtractorMOG.Create(15, 3, 0.2f, 0);
            backgroundSubtractor = BackgroundSubtractorKNN.Create(50, 400, false);
        }

        void Update() {
            if (cameraOutput != null) {
                Mat mat = Unity.TextureToMat(cameraOutput.cameraTexture);
                Mat outputMat = new Mat();
                // Cv2.Rectangle(mat,, );
                // Cv2.CvtColor(mat, outputMat, ColorConversionCodes.BGR2GRAY);
                backgroundSubtractor.Apply(mat, outputMat);

                Texture2D texture = Unity.MatToTexture(outputMat);
                openCVView.outputTexture.texture = texture;
            }
        }
    }
}