using UnityEngine;

namespace OpenCvSharp.Demo {
    public class HandTrackingManager : MonoBehaviour {
        public CameraOutput cameraOutput;
        public OpenCVView openCVView;

        private BackgroundSubtractor backgroundSubtractor;

        private Mat handHist;
        private Mat handHSV;
        private Mat handMask;

        [Header("Calibration")] public ROIAreaType roiAreaType = ROIAreaType.Single;
        private bool calibrated = false;

        [Header("TreshHold filter")] public bool thresholdFilter = true;
        [Range(0, 255)] public float thresholdLow;

        [Range(0, 255)] public float thresholdHigh;

        [Header("Filter2D Filter")] public bool filter2D = false;
        public MorphShapes filter2DShape = MorphShapes.Ellipse;
        public Vector2Int filter2DSize = new Vector2Int(3, 3);

        [Header("Morphology transformation")] public bool morphologyTransformation = true;
        public MorphShapes morphologyTransformationShape = MorphShapes.Ellipse;
        public Vector2Int morphologyTransformationSize = new Vector2Int(3, 3);
        public int morphologyTransformationIterations = 1;
        public MorphTypes morphologyTransformationType = MorphTypes.Close;

        [Header("Erode Filter")] public bool erodeFilter = false;
        public MorphShapes erodeShape = MorphShapes.Ellipse;
        public Vector2Int erodeSize = new Vector2Int(3, 3);
        [Range(0, 25)] public int erodeIterations = 1;

        [Header("Dilatate Filter")] public bool dilatateFilter = false;
        public MorphShapes dilatateShape = MorphShapes.Ellipse;
        public Vector2Int dilatateSize = new Vector2Int(3, 3);
        [Range(0, 25)] public int dilatateIterations = 1;

        [Header("BinaryAnd Filter")] public bool bitwiseFilter = false;

        [Header("Camera output scale")] [Range(0, 1)]
        public float cameraOutputScale = 0.25f;

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
                Mat scaledCameraOutputMat = scaleCameraOutput(cameraOutputMat);

                if (Input.GetKeyDown("space") || Input.touches.Length > 0) {
                    calibrate(cameraOutputMat);
                }

                if (calibrated) {
                    openCVView.outputTexture.texture = Unity.MatToTexture(histMasking(scaledCameraOutputMat));
                }
            }
        }

        private Mat scaleCameraOutput(Mat cameraOutputMat) {
            float width = cameraOutputMat.Width * cameraOutputScale;
            float height = cameraOutputMat.Height * cameraOutputScale;
            Size scaledSize = new Size(width, height);

            Mat scaledCameraOutputMat = new Mat();
            Cv2.Resize(cameraOutputMat, scaledCameraOutputMat, scaledSize);

            return scaledCameraOutputMat;
        }

        /// <summary>
        /// Sets handHist Mat which is nornalized histogram computed from ROI
        /// </summary>
        /// <param name="mat"></param>
        /// TODO: add multi-step calibration (front hand, back hand, fist with visible fingernails) / or take multiple frames for creatig histogram
        private void calibrate(Mat cameraOutputMat) {
            Debug.Log("Calibration");

            Cv2.CvtColor(cameraOutputMat, handHist, ColorConversionCodes.BGR2HSV);

            int roisCount = 0;
            Mat[] ROIs = null;

            switch (roiAreaType) {
                case ROIAreaType.Single:
                    roisCount = 1;
                    ROIs = new Mat[roisCount];
                    ROIs[0] = handHist.GetRectSubPix(cameraOutput.targetArea.Size, cameraOutput.targetArea.Center);
                    break;
                case ROIAreaType.Multi:
                    roisCount = cameraOutput.subTargetAreasCount;
                    ROIs = new Mat[roisCount];
                    for (int i = 0; i < cameraOutput.subTargetAreasCount; i++) {
                        ROIs[i] = handHist.GetRectSubPix(cameraOutput.subTargetAreas[i].Size, cameraOutput.subTargetAreas[i].Center);
                    }

                    break;
            }


            Cv2.CalcHist(ROIs, new[] {0, 1}, new Mat(), handHist, 2, new[] {180, 255}, new[] {new Rangef(0, 180), new Rangef(0, 255)});
            Cv2.Normalize(handHist, handHist, 0, 255, NormTypes.MinMax);

            calibrated = true;
        }

        private Mat histMasking(Mat cameraMat) {
            Mat hsvMat = new Mat();
            Mat dstMat = new Mat();
            Cv2.CvtColor(cameraMat, hsvMat, ColorConversionCodes.BGR2HSV);
            Cv2.CalcBackProject(new[] {hsvMat}, new[] {0, 1}, handHist, dstMat, new[] {new Rangef(0, 180), new Rangef(0, 255)});

            if (filter2D) {
                Mat filter2DMat = Cv2.GetStructuringElement(filter2DShape, (new Size(filter2DSize.x, filter2DSize.y)));
                Cv2.Filter2D(dstMat, dstMat, -1, filter2DMat);
            }

            if (thresholdFilter) {
                Cv2.Threshold(dstMat, dstMat, thresholdLow, thresholdHigh, ThresholdTypes.Binary);
            }

            if (morphologyTransformation) {
                Mat morphologyTransformationMat =
                    Cv2.GetStructuringElement(morphologyTransformationShape, (new Size(morphologyTransformationSize.x, morphologyTransformationSize.y)));
                Cv2.MorphologyEx(dstMat, dstMat, morphologyTransformationType, morphologyTransformationMat, null, morphologyTransformationIterations);
            }

            if (dilatateFilter) {
                Mat dilatateShapeMat = Cv2.GetStructuringElement(this.dilatateShape, (new Size(dilatateSize.x, dilatateSize.y)));
                Cv2.Dilate(dstMat, dstMat, dilatateShapeMat, null, dilatateIterations);
            }

            if (erodeFilter) {
                Mat erodeShapeMat = Cv2.GetStructuringElement(this.erodeShape, (new Size(erodeSize.x, erodeSize.y)));
                Cv2.Erode(dstMat, dstMat, erodeShapeMat, null, erodeIterations);
            }

            handMask = dstMat;

            if (bitwiseFilter) {
                // Cv2.BitwiseAnd(cameraMat, dstMat, dstMat); //TODO: we need it to mask our hand, in this form it return exception
            }

            return dstMat;
        }
    }

    public enum BackgroundSubractorType {
        BackgroundSubtractorTypeKNN,
        BackgroundSubtractorTypeMOG,
        BackgroundSubtractorTypeGMG
    }

    public enum ROIAreaType {
        Single,
        Multi
    }

    public enum debugViewType {
        BinaryHandCMask,
        MaskedOutput
    }
}