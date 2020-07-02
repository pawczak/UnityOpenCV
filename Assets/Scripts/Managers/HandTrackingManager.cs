using UnityEngine;

namespace OpenCvSharp.Demo {
    public class HandTrackingManager : MonoBehaviour {
        public CameraOutput cameraOutput;
        public OpenCVView openCVView;

        private BackgroundSubtractor backgroundSubtractor;

        private Mat handHist;
        private Mat handHSV;
        private Mat handMask;
        private bool calibrated = false;

        [Header("TreshHold filter")] public bool treshHoldFilter = true;
        [Range(0, 255)] public float treshHoldLow;

        [Range(0, 255)] public float treshHoldHigh;

        [Header("Filter2D Filter")] public bool filter2D = false;
        public MorphShapes filter2DShape = MorphShapes.Ellipse;
        public Vector2Int filter2DSize = new Vector2Int(3, 3);

        [Header("Erode Filter")] public bool erodeFilter = false;
        [Range(0, 25)] public int erodeIterations = 1;

        [Header("Dilatate Filter")] public bool dilatateFilter = false;
        [Range(0, 25)] public int dilatateIterations = 1;

        // [Header("BackgroundSubstractor")] public BackgroundSubractorType bgSubctractorType;
        // private BackgroundSubtractorGMG bgSubstractorGMG;
        // private BackgroundSubtractorKNN bgSubstractorKNN;
        // private BackgroundSubtractorMOG bgSubstractorMOG;

        void Start() {
            handHist = new Mat();
            handHSV = new Mat();
            handMask = new Mat();
            // bgSubstractorGMG = BackgroundSubtractorGMG.Create(50, 0.6);
            // bgSubstractorKNN = BackgroundSubtractorKNN.Create(50, 400, false);
            // bgSubstractorMOG = BackgroundSubtractorMOG.Create(50, 3, 0.2f, 0);
        }

        void Update() {
            if (cameraOutput != null) {
                //memory leak fix
                if (openCVView.outputTexture.texture != null) {
                    Destroy(openCVView.outputTexture.texture);
                }

                Mat cameraOutputMat = Unity.TextureToMat(cameraOutput.cameraTexture);
                Mat outputMat = new Mat();

                //TODO: detect hand and nail processing


                if (Input.GetKeyDown("space") || Input.touches.Length > 0) {
                    calibrate(cameraOutputMat);
                }

                if (calibrated) {
                    Cv2.CvtColor(cameraOutputMat, handHSV, ColorConversionCodes.RGB2HSV);
                    Cv2.InRange(handHSV, new Scalar(0, 140, 60), new Scalar(255, 255, 180), handHSV);

                    openCVView.outputTexture.texture = Unity.MatToTexture(histMasking(cameraOutputMat));
                }
            }
        }

        private void calibrate(Mat mat) {
            Debug.Log("calibrate");

            Cv2.CvtColor(mat, handHist, ColorConversionCodes.BGR2HSV);

            Mat hsvROI = handHist.GetRectSubPix(cameraOutput.targetArea.Size, cameraOutput.targetArea.Center);
            Cv2.CalcHist(new[] {hsvROI}, new[] {0, 1}, new Mat(), handHist, 2, new[] {180, 255}, new[] {new Rangef(0, 180), new Rangef(0, 255)});
            Cv2.Normalize(handHist, handHist, 0, 255, NormTypes.MinMax);

            calibrated = true;
        }

        private Mat histMasking(Mat cameraMat) {
            Mat hsvMat = new Mat();
            Mat dstMat = new Mat();
            Cv2.CvtColor(cameraMat, hsvMat, ColorConversionCodes.BGR2HSV);
            Cv2.CalcBackProject(new[] {hsvMat}, new[] {0, 1}, handHist, dstMat, new[] {new Rangef(0, 180), new Rangef(0, 255)});


            if (filter2D) {
                Mat disc = Cv2.GetStructuringElement(filter2DShape, (new Size(filter2DSize.x, filter2DSize.y)));
                Cv2.Filter2D(dstMat, dstMat, -1, disc);
            }

            if (treshHoldFilter) {
                Cv2.Threshold(dstMat, dstMat, treshHoldLow, treshHoldHigh, ThresholdTypes.Binary);
            }

            if (dilatateFilter) {
                Cv2.Dilate(dstMat, dstMat, new Mat(), null, dilatateIterations);
            }

            if (erodeFilter) {
                Cv2.Erode(dstMat, dstMat, new Mat(), null, erodeIterations);
            }
            // Cv2.Merge(new []{dstMat, dstMat, dstMat}, dstMat);
            // if (binaryFilter) {
            //     // Cv2.BitwiseAnd(cameraMat, dstMat);
            // }

            return dstMat;
        }
    }

    public enum BackgroundSubractorType {
        BackgroundSubtractorTypeKNN,
        BackgroundSubtractorTypeMOG,
        BackgroundSubtractorTypeGMG
    }
}