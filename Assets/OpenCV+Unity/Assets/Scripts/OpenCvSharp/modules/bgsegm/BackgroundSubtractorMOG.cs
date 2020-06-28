using System;

namespace OpenCvSharp
{
    // ReSharper disable InconsistentNaming

    /// <summary>
    /// Gaussian Mixture-based Backbround/Foreground Segmentation Algorithm
    /// </summary>
    public class BackgroundSubtractorMOG : BackgroundSubtractor
    {
        /// <summary>
        /// cv::Ptr&lt;T&gt;
        /// </summary>
        private Ptr<BackgroundSubtractorMOG> objectPtr;

        /// <summary>
        /// 
        /// </summary>
        private bool disposed;

        #region Init & Disposal

        /// <summary>
        /// 
        /// </summary>
        /// <param name="history"></param>
        /// <param name="nMixtures"></param>
        /// <param name="backgroundRatio"></param>
        /// <param name="noiseSigma"></param>
        /// <returns></returns>
        public static BackgroundSubtractorMOG Create(
            int history = 200, int nMixtures = 5, double backgroundRatio = 0.7, double noiseSigma = 0)
        {
            IntPtr ptr = NativeMethods.bgsegm_createBackgroundSubtractorMOG(
                history, nMixtures, backgroundRatio, noiseSigma);
            return new BackgroundSubtractorMOG(ptr);
        }

        internal BackgroundSubtractorMOG(IntPtr ptr)
        {
            this.objectPtr = new Ptr<BackgroundSubtractorMOG>(ptr);
            this.ptr = objectPtr.Get(); 
        }

#if LANG_JP
    /// <summary>
    /// ƒŠƒ\[ƒX‚Ì‰ð•ú
    /// </summary>
    /// <param name="disposing">
    /// true‚Ìê‡‚ÍA‚±‚Ìƒƒ\ƒbƒh‚ªƒ†[ƒUƒR[ƒh‚©‚ç’¼Ú‚ªŒÄ‚Î‚ê‚½‚±‚Æ‚ðŽ¦‚·Bƒ}ƒl[ƒWEƒAƒ“ƒ}ƒl[ƒW‘o•û‚ÌƒŠƒ\[ƒX‚ª‰ð•ú‚³‚ê‚éB
    /// false‚Ìê‡‚ÍA‚±‚Ìƒƒ\ƒbƒh‚Íƒ‰ƒ“ƒ^ƒCƒ€‚©‚çƒtƒ@ƒCƒiƒ‰ƒCƒU‚É‚æ‚Á‚ÄŒÄ‚Î‚êA‚à‚¤‚Ù‚©‚ÌƒIƒuƒWƒFƒNƒg‚©‚çŽQÆ‚³‚ê‚Ä‚¢‚È‚¢‚±‚Æ‚ðŽ¦‚·BƒAƒ“ƒ}ƒl[ƒWƒŠƒ\[ƒX‚Ì‚Ý‰ð•ú‚³‚ê‚éB
    ///</param>
#else
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">
        /// If disposing equals true, the method has been called directly or indirectly by a user's code. Managed and unmanaged resources can be disposed.
        /// If false, the method has been called by the runtime from inside the finalizer and you should not reference other objects. Only unmanaged resources can be disposed.
        /// </param>
#endif
        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                try
                {
                    if (disposing)
                    {
                    }
                    if (IsEnabledDispose)
                    {
                        if (objectPtr != null)
                        {
                            objectPtr.Dispose();
                        }
                        objectPtr = null;
                        ptr = IntPtr.Zero;
                    }
                    disposed = true;
                }
                finally
                {
                    base.Dispose(disposing);
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public int History
        {
            get
            {
                if (disposed)
                    throw new ObjectDisposedException(GetType().Name);
                return NativeMethods.bgsegm_BackgroundSubtractorMOG_getHistory(ptr);
            }
            set
            {
                if (disposed)
                    throw new ObjectDisposedException(GetType().Name);
                NativeMethods.bgsegm_BackgroundSubtractorMOG_setHistory(ptr, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int NMixtures
        {
            get
            {
                if (disposed)
                    throw new ObjectDisposedException(GetType().Name);
                return NativeMethods.bgsegm_BackgroundSubtractorMOG_getNMixtures(ptr);
            }
            set
            {
                if (disposed)
                    throw new ObjectDisposedException(GetType().Name);
                NativeMethods.bgsegm_BackgroundSubtractorMOG_setNMixtures(ptr, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public double BackgroundRatio
        {
            get
            {
                if (disposed)
                    throw new ObjectDisposedException(GetType().Name);
                return NativeMethods.bgsegm_BackgroundSubtractorMOG_getBackgroundRatio(ptr);
            }
            set
            {
                if (disposed)
                    throw new ObjectDisposedException(GetType().Name);
                NativeMethods.bgsegm_BackgroundSubtractorMOG_setBackgroundRatio(ptr, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public double NoiseSigma
        {
            get
            {
                if (disposed)
                    throw new ObjectDisposedException(GetType().Name);
                return NativeMethods.bgsegm_BackgroundSubtractorMOG_getNoiseSigma(ptr);
            }
            set
            {
                if (disposed)
                    throw new ObjectDisposedException(GetType().Name);
                NativeMethods.bgsegm_BackgroundSubtractorMOG_setNoiseSigma(ptr, value);
            }
        }

        #endregion
    }
}