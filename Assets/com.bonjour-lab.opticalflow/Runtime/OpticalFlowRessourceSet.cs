using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bonjour.Vision.Ressources
{
    [CreateAssetMenu(fileName = "OpticalFlowParams", menuName = "ScriptableObject/OpticalFlow Ressource Set")]
    public sealed class OpticalFlowRessourceSet : ScriptableObject
    {
        [Tooltip("Define the compute shader to use for Optical Flow")] public ComputeShader opticalFlowCS;
        [Tooltip("Define the shader to use for denoising the source image")] public Shader denoiser;
    }
}
