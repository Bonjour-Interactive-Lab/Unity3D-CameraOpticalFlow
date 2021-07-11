using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Bonjour.Vision.Ressources;

namespace Bonjour.Vision
{
    public class AverageVelocity : MonoBehaviour
    {
        [Header("Ressource")]
        public AverageVelocityRessourceSet averageVelocityRessource;

        [Header("Average params")]
        [Tooltip("Is average computed on Trail or Optical Flow")] public bool isComputedOnTrail = false;
        [Tooltip("Define the resolution on which we will compute the average")] public int resolution = 4;
        [Range(0, 1)]
        [Tooltip("Define the range for min velocity")] public float threshold = 0.1f;
        

        [HideInInspector] public Vector2 averageVelocity;
        private Vector2[] averageArray  = new Vector2[1];

        private OpticalFlow of;
        private OFTrailSystemUpdater trail;

        //RenderTexture to copy into
        private RenderTexture sourceCopy;

        //compute shader params
        private ComputeShader compute;
        private int kernelHandle;
        private ComputeBuffer averageVelocityBuffer;
        [Tooltip("Log the size of the buffer")] public bool logBufferSize;

        private void Start(){
            InitBuffers();
        }

        private void Update(){
            ComputeAverageVelocity();
        }

        private void InitBuffers(){
            //init average vel + buffer
            averageVelocity         = Vector2.zero;
            averageArray[0]         = averageVelocity;
            averageVelocityBuffer   = new ComputeBuffer(1, sizeof(float) * 2);
            averageVelocityBuffer.SetData(averageArray);

            //Get OF or Trail
            of      = this.GetComponent<OpticalFlow>();
            trail   = this.GetComponent<OFTrailSystemUpdater>();
            
            //Init RT
            sourceCopy = new RenderTexture((of.opticalFlowWidth/of.resolution)/resolution, (of.opticalFlowHeight/of.resolution)/resolution, 24, RenderTextureFormat.ARGBFloat);

            //Init Compute Buffer
            compute         = Instantiate(averageVelocityRessource.averageVelocityCS);
            kernelHandle    = compute.FindKernel("CSMain");

            compute.SetBuffer(kernelHandle, "_AverageVelocity", averageVelocityBuffer);
            compute.SetTexture(kernelHandle, "_Source", sourceCopy);
            compute.SetVector("_Resolution", new Vector2(sourceCopy.width, sourceCopy.height));

            if(logBufferSize) Debug.Log($"Average buffer source size set at: {sourceCopy.width}×{sourceCopy.height}");
        }

        private void ComputeAverageVelocity(){
            if(trail && trail.GetOFTrail() == null) return; //Trail is created at first loop so we need to jump this frame (lazy implementation ;))

            Graphics.Blit(isComputedOnTrail ? trail.GetOFTrail() : of.GetOpticalFlowMap(), sourceCopy);
            
            compute.SetFloat("_Threshold", threshold);
            compute.Dispatch(kernelHandle, 1, 1, 1);

            averageVelocityBuffer.GetData(averageArray);
            averageVelocity = (Vector2) averageArray[0];
        }

        private void OnDisable()
        {
            if (averageVelocityBuffer != null)
            {
                averageVelocityBuffer.Release();
            }
        }

        public Vector2 GetAverageVelocity(){
            return averageVelocity;
        }

    }
}