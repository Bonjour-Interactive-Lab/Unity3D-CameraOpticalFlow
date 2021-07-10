using UnityEngine;

namespace Bonjour
    {
    public sealed class WebcamInput : MonoBehaviour
    {
        [SerializeField] string deviceName = "";
        [SerializeField] Vector2Int resolution = new Vector2Int(1920, 1080);
        [SerializeField] int frameRate = 30;


        WebCamTexture webcam;
        [SerializeField] public RenderTexture targetBuffer;
        [SerializeField] bool clampFPSToCameraFPS;


        void Start()
        {
            if(clampFPSToCameraFPS){
                // ! Only usefull if you want to clamp the FPS to the speed of youir camera 
                QualitySettings.vSyncCount = 0;
                Application.targetFrameRate = frameRate;
            }

            webcam       = new WebCamTexture(deviceName, resolution.x, resolution.y, frameRate);
            // targetBuffer  = new RenderTexture(_resolution.x, _resolution.y, 0);
            webcam.Play();
        }

        void OnDestroy()
        {
            if (webcam != null) Destroy(webcam);
            // if (targetBuffer != null) Destroy(targetBuffer);
        }

        void Update()
        {
            if (!webcam.didUpdateThisFrame) return;

            var aspect1 = (float)webcam.width / webcam.height;
            var aspect2 = (float)resolution.x / resolution.y;
            var gap = aspect2 / aspect1;

            var vflip = webcam.videoVerticallyMirrored;
            var scale = new Vector2(gap, vflip ? -1 : 1);
            var offset = new Vector2((1 - gap) / 2, vflip ? 1 : 0);

            Graphics.Blit(webcam, targetBuffer, scale, offset);
        }

    }
}
