using UnityEngine;

public sealed class WebcamInput : MonoBehaviour
{
    [SerializeField] string _deviceName = "";
    [SerializeField] Vector2Int _resolution = new Vector2Int(1920, 1080);
    [SerializeField] int _frameRate = 30;


    WebCamTexture _webcam;
    [SerializeField] public RenderTexture targetBuffer;


    void Start()
    {
        // ! Only usefull if you want to clamp the FPS to the speed of youir camera 
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = _frameRate;

        _webcam       = new WebCamTexture(_deviceName, _resolution.x, _resolution.y, _frameRate);
        // targetBuffer  = new RenderTexture(_resolution.x, _resolution.y, 0);
        _webcam.Play();
    }

    void OnDestroy()
    {
        if (_webcam != null) Destroy(_webcam);
        // if (targetBuffer != null) Destroy(targetBuffer);
    }

    void Update()
    {
        if (!_webcam.didUpdateThisFrame) return;

        var aspect1 = (float)_webcam.width / _webcam.height;
        var aspect2 = (float)_resolution.x / _resolution.y;
        var gap = aspect2 / aspect1;

        var vflip = _webcam.videoVerticallyMirrored;
        var scale = new Vector2(gap, vflip ? -1 : 1);
        var offset = new Vector2((1 - gap) / 2, vflip ? 1 : 0);

        Graphics.Blit(_webcam, targetBuffer, scale, offset);
    }

}
