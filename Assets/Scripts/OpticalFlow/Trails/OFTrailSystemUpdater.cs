using UnityEngine;
using Bonjour.Vision;

// todo : Create controller to handle Trail↔OF binding
// todo : Create CRT from code using binding params
namespace Bonjour.Vision
{
    public class OFTrailSystemUpdater : MonoBehaviour
    {
        private CustomRenderTexture texture;
        [Range(1, 16)] public int stepsPerFrame = 4;

        private Material CRTMat;
        private OpticalFlow opticalFlow;
        [Range(.75f, 1f)]public float trailPower = 0.9f;

        public Material debugMaterial;

        void Start()
        {
            opticalFlow = GetComponent<OpticalFlow>();
            CRTMat  = new Material(Shader.Find("Hidden/CustomRenderTexture/Trail"));
        }

        void Update()
        {
            if(texture == null){
                texture = new CustomRenderTexture(opticalFlow.GetOpticalFlowMap().width, opticalFlow.GetOpticalFlowMap().height, opticalFlow.GetOpticalFlowMap().format);

                texture.initializationTexture   = opticalFlow.GetOpticalFlowMap();
                texture.initializationColor     = Color.black;
                texture.material                = CRTMat;
                texture.updateMode              = CustomRenderTextureUpdateMode.OnDemand;
                texture.doubleBuffered          = true;

                texture.Initialize();
                texture.Update(stepsPerFrame);

            }else{
                CRTMat.SetTexture("_NewFrame", opticalFlow.GetOpticalFlowMap());
                CRTMat.SetFloat("_TrailForce", trailPower);
                texture.Update(stepsPerFrame);

                debugMaterial.SetTexture("_OF", texture);
            }
        }

        public CustomRenderTexture GetOFTrail(){
            return texture;
        }
    }
}
