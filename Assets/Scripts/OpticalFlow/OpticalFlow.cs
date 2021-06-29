using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Todo: add debug view using Shader drawing
// Todo: do not pack value into RT as we are using Signed Floating Point Texture (SFPT)
// ? ↑ Not sure about this part
// ? Quid to set an iterable system for the denoiser and/or gaussian ?
// Todo: clean up code
public class OpticalFlow : MonoBehaviour
{    
    public enum BlurType{
        NONE    = 0,
        LOW     = 5,
        MEDIUM  = 7,
        HIGH    = 9,
        ULTRA   = 13
    };

    //Compute shader
    [Header("OpticalFlow Params")]
    [HideInInspector] public int opticalFlowWidth;
    [HideInInspector] public int opticalFlowHeight;
    [Tooltip("Defines the scale resolution from the source")] public int resolution;
    private int kernelHandle;
    [Tooltip("Defines the compute Shader to use for Optical flow")] public ComputeShader compute;
    private RenderTexture opticalFlow;

    [Range(0, 1)]
    public float lambda = 0.01f;
    [Range(0, 1)]
    public float threshold = 0.01f;
    public Vector2 scale = new Vector2(1.0f, 1.0f);
    private Material gaussianBlur;
    [Tooltip("Define the type of blur")] public BlurType blurType = BlurType.ULTRA;
    [Tooltip("Define the amount of blur for the output")] [Range(1f, 100f)] public float blurSize = 1f;

    [Header("Source images")]
    [Tooltip("Source image to analyze")] public RenderTexture sourceToAnalyze;
    [Tooltip("Horizontal mirror on the source image")] public bool mirrorHor = false;
    [Tooltip("Define a denoiser methods for source")] public Shader denoiserShader;
    private bool useDenoiser;
    private Material denoiser;
    [HideInInspector] public RenderTexture current;
    [HideInInspector] public RenderTexture previous;
    private Vector2 rtScale, rtOffset;

    [Header("Debug")]
    public bool showOpticalFlowMap;
    public float debugMapScale;
    private RenderTexture debugView;
    private Material debugViewer;
   
    
    private void Awake() {
    }

    private void Start()
    { 
        InitOpticalFlow();
    }

    private void Update()
    {
        ComputeOpticalFlow();
    }

    public void InitOpticalFlow(){
        InitSources();
        InitDenoiser();
        InitGaussianBlur();
        InitBuffers();
        InitDebugViewer();
    }

    private void InitBuffers(){
        kernelHandle = compute.FindKernel("CSMain");
        
        opticalFlow = new RenderTexture(opticalFlowWidth, opticalFlowHeight, 24, RenderTextureFormat.ARGBFloat);
        opticalFlow.filterMode = FilterMode.Trilinear;
        opticalFlow.wrapMode = TextureWrapMode.Clamp;
        opticalFlow.enableRandomWrite = true;
        opticalFlow.Create();

        //Bind variable to CS
        compute.SetTexture(kernelHandle, "_OpticalFlowMap", opticalFlow);
        compute.SetVector("_Size", new Vector2((float)opticalFlow.width, (float)opticalFlow.height));
        compute.SetTexture(kernelHandle, "_Previous", previous);
        compute.SetTexture(kernelHandle, "_Current", current);

        Debug.Log("Buffers init at: "+opticalFlow.width+"×"+opticalFlow.height);
    }

    private void InitSources(){
        opticalFlowWidth    = sourceToAnalyze.width / resolution;
        opticalFlowHeight   = sourceToAnalyze.height / resolution;

        current     = new RenderTexture(opticalFlowWidth, opticalFlowHeight, 0);
        previous    = new RenderTexture(opticalFlowWidth, opticalFlowHeight, 0);

        CheckMirror();
        
        Graphics.Blit(sourceToAnalyze, current, rtScale, rtOffset);
        Graphics.Blit(current, previous);
    }

    private void InitDebugViewer(){
        debugView                   = new RenderTexture(sourceToAnalyze.width, sourceToAnalyze.height, 24, RenderTextureFormat.ARGB32);
        debugView.filterMode        = FilterMode.Trilinear;
        debugView.wrapMode          = TextureWrapMode.Clamp;

        debugViewer                 = new Material(Shader.Find("Hidden/OFViewer"));
    }

    private void InitDenoiser(){
        if(denoiserShader != null){
            useDenoiser = true;
            denoiser = new Material(denoiserShader);
        }
    }

    private void InitGaussianBlur(){
        gaussianBlur = new Material(Shader.Find("Hidden/GaussianBlur"));
    }

    private void CheckMirror(){
        rtScale     = mirrorHor ? new Vector2(-1, 1) : new Vector2(1, 1);
        rtOffset    = mirrorHor ? new Vector2( 1, 0) : new Vector2(0, 0);
    }

    public void ComputeOpticalFlow(){
        CheckMirror();

        //Send source to current RT
        Graphics.Blit(sourceToAnalyze, current, rtScale, rtOffset);

        //Creat TMP RT for effect
        RenderTexture temp = RenderTexture.GetTemporary(opticalFlow.width, opticalFlow.height, 24, opticalFlow.format);

        //Denoise source if denoiser is set
        if(useDenoiser){
            Graphics.Blit(current, temp, denoiser); //denoise current
            Graphics.Blit(temp, current); //send tmps to current
        }

        //Compute OF
        ComputeOpticalFlowCS();

        if(blurType != BlurType.NONE){
            //Blur result
            float blurAmount = (blurSize / 100f) * (float)blurType;
            int pass = gaussianBlur.FindPass(blurType.ToString());

            gaussianBlur.SetFloat("_Sigma", blurSize);
            gaussianBlur.SetVector("_Dir", new Vector2(0, 1));
            gaussianBlur.SetFloat("_BlurSize", 1.0f / current.height);
            Graphics.Blit(opticalFlow, temp, gaussianBlur, pass); //Blur vertical

            gaussianBlur.SetVector("_Dir", new Vector2(1, 0));
            gaussianBlur.SetFloat("_BlurSize", 1.0f / current.width);
            Graphics.Blit(temp, opticalFlow, gaussianBlur, pass); //Blur horizontal
        }
        
        if(showOpticalFlowMap){
            debugViewer.SetTexture("_OF", opticalFlow);
            Graphics.Blit(sourceToAnalyze, debugView, debugViewer);
        }


        //Set current as previous frame for next frame
        Graphics.Blit(current, previous);

        //Release TMP RT
        RenderTexture.ReleaseTemporary(temp);
    }

    private void ComputeOpticalFlowCS(){
        compute.SetFloat("_Lambda", lambda);
        compute.SetFloat("_Threshold", threshold);
        compute.SetVector("_Scale", scale);
        compute.Dispatch(kernelHandle,  Mathf.CeilToInt((float)opticalFlow.width/32),  Mathf.CeilToInt((float)opticalFlow.height/32), 1);
    }

    private void OnDisable(){
        if(opticalFlow != null){
            opticalFlow.Release();
        }
        opticalFlow = null;

        if(previous != null) previous = null;

        if(denoiser != null) denoiser = null;
    }

    private void OnGUI(){
        if(showOpticalFlowMap && opticalFlow != null){
            int w = Mathf.RoundToInt(opticalFlow.width  * debugMapScale);
            int h = Mathf.RoundToInt(opticalFlow.height * debugMapScale);

            GUI.DrawTexture(new Rect(w*0, Screen.height - h, w, h), debugView);
            GUI.DrawTexture(new Rect(w*1, Screen.height - h, w, h), opticalFlow);
            // GUI.DrawTexture(new Rect(w*1, Screen.height - h, w, h), previous);
            // GUI.DrawTexture(new Rect(w*2, Screen.height - h, w, h), current);
        }
    }

    public RenderTexture GetOpticalFlowMap(){
        return this.opticalFlow;
    }
}
