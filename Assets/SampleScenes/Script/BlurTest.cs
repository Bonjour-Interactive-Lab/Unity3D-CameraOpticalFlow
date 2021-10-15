using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bonjour.Vision;

public class BlurTest : MonoBehaviour
{
    public Texture2D imageToBlur;
    private RenderTexture target;
    private Material gaussianBlur;
    public OpticalFlow.BlurType blurType;
    [Range(1f, 100f)] public float blurSize = 25f;

    private void Start(){
        gaussianBlur = new Material(Shader.Find("Hidden/GaussianBlur"));
        target = new RenderTexture(imageToBlur.width, imageToBlur.height, 0, RenderTextureFormat.ARGBFloat);
        target.enableRandomWrite = true;
        target.Create();
    }

    public void Update(){
        RenderTexture tempV = RenderTexture.GetTemporary(target.width, target.height, 0, target.format);
        RenderTexture tempH = RenderTexture.GetTemporary(target.width, target.height, 0, target.format);
        if (blurType != OpticalFlow.BlurType.NONE)
        {
            //Blur result
            float blurAmount = (blurSize / 100f) * (float)blurType;
            int pass = gaussianBlur.FindPass(blurType.ToString());

            gaussianBlur.SetFloat("_Sigma", blurSize);
            gaussianBlur.SetVector("_Dir", new Vector2(0, 1));
            gaussianBlur.SetFloat("_BlurSize", 1.0f / imageToBlur.height);
            Graphics.Blit(imageToBlur, tempV, gaussianBlur, pass); //Blur vertical

            gaussianBlur.SetVector("_Dir", new Vector2(1, 0));
            gaussianBlur.SetFloat("_BlurSize", 1.0f / imageToBlur.width);
            Graphics.Blit(tempV, target, gaussianBlur, pass); //Blur horizontal
        }

        // Graphics.Blit(tempH, target);
        //Release TMP RT
        RenderTexture.ReleaseTemporary(tempV);
        RenderTexture.ReleaseTemporary(tempH);
    }

    private void OnGUI() {
        if(target != null){
            GUI.DrawTexture(new Rect(0, 0, target.width, target.height), target);
        }
    }

}
