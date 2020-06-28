namespace OpenCvSharp.Demo
{
    using UnityEngine;
    using OpenCvSharp;

    public class CameraPreview : WebCamera
    {
        protected override void Awake()
        {
            base.Awake();
            // this.forceFrontalCamera = true;
        }

        // Our sketch generation function
        protected override bool ProcessTexture(WebCamTexture input, ref Texture2D output)
        {
            return true;
        }
    }
}