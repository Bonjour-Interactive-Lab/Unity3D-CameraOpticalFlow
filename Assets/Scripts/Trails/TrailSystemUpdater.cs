using UnityEngine;
using Bonjour.Vision;

// todo : Create controller to handle Trail↔OF binding
// todo : Create CRT from code using binding params
// todo : cleanup trail TrailSystemUpdater.cs
// todo : cleanu trail shader
namespace Bonjour
{
    public class TrailSystemUpdater : MonoBehaviour
    {
        [SerializeField] CustomRenderTexture _texture;
        [SerializeField, Range(1, 16)] int _stepsPerFrame = 4;

        public Material CRTMat;

        private OpticalFlow opticalFlow;

        void Start()
        {
            opticalFlow = GetComponent<OpticalFlow>();
            
            _texture.initializationTexture = opticalFlow.GetOpticalFlowMap();
            _texture.Initialize();

            // CRTMat.SetTexture("_NewFrame", opticalFlow.GetOpticalFlowMap());
            _texture.Update(_stepsPerFrame);
        }

        void Update()
        {
            CRTMat.SetTexture("_NewFrame", opticalFlow.GetOpticalFlowMap());
            _texture.Update(_stepsPerFrame);
        }
    }
}
