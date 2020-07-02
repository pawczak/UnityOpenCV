using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OpenCvSharp.Demo {
    public class CameraOutput : MonoBehaviour {
        [Header("Target Area")] public List<Rect> subTargetAreas = new List<Rect>();
        public int subTargetAreasColumnCount = 3;
        public int subTargetAreasRowCount = 3;

        public int subTargetAreasCount {
            get => subTargetAreasColumnCount * subTargetAreasRowCount;
        }

        [Range(1, 100)] public int subTargetAreaSize = 10;

        public Rect targetArea;
        public Vector2 targetAreaPercentSize = new Vector2(0.1f, 0.1f);
        public Color targetAreaColor;

        public RawImage cameraOutputImage;
        public WebCamTexture cameraTexture;

        void Start() {
            cameraTexture = new WebCamTexture();
            cameraTexture.Play();
            setTargetArea();
        }

        private void Update() {
            //memory leak fix
            if (cameraOutputImage.texture != null) {
                Destroy(cameraOutputImage.texture);
            }

            Mat inputTexture = Unity.TextureToMat(cameraTexture);
            cameraOutputImage.texture = Unity.MatToTexture(addTargetAreaRect(inputTexture));
        }

        private void setTargetArea() {
            float targetAreaWidth = cameraTexture.width * targetAreaPercentSize.x;
            float targetAreaHeight = cameraTexture.height * targetAreaPercentSize.y;
            targetArea = new Rect(new Point((cameraTexture.width / 2) - targetAreaWidth * 0.5, (cameraTexture.height / 2) - targetAreaHeight * 0.5),
                new Size(cameraTexture.width * targetAreaPercentSize.x, cameraTexture.height * targetAreaPercentSize.y));

            int subAreaStepX = Mathf.RoundToInt(targetAreaWidth / subTargetAreasColumnCount);
            int subAreaStepY = Mathf.RoundToInt(targetAreaHeight / subTargetAreasRowCount);

            Point subAreaStartingPosition = targetArea.TopLeft;
            subAreaStartingPosition.X += subAreaStepX / 2;
            subAreaStartingPosition.Y += subAreaStepY / 2;
            Size subAreaPositionOffset = new Size(subTargetAreaSize / 2, subTargetAreaSize / 2);
            for (int i = 0; i < subTargetAreasCount; i++) {
                Rect subAreaRect = new Rect();
                subAreaRect.Size = new Size(subTargetAreaSize, subTargetAreaSize);

                int rowIndex = i / subTargetAreasColumnCount;
                int columnIndex = i % subTargetAreasColumnCount;

                subAreaRect.X = subAreaStartingPosition.X + subAreaStepX * columnIndex - subAreaPositionOffset.Width;
                subAreaRect.Y = subAreaStartingPosition.Y + subAreaStepY * rowIndex - subAreaPositionOffset.Height;

                subTargetAreas.Add(subAreaRect);
            }
        }

        private Mat addTargetAreaRect(Mat inputMat) {
            for (int i = 0; i < subTargetAreas.Count; i++) {
                Cv2.Rectangle(inputMat, subTargetAreas[i].TopLeft, subTargetAreas[i].BottomRight, new Scalar(0, 255, 0), 5);
            }

            Cv2.Rectangle(inputMat, targetArea.TopLeft, targetArea.BottomRight, new Scalar(0, 255, 0), 5);

            return inputMat;
        }
    }
}