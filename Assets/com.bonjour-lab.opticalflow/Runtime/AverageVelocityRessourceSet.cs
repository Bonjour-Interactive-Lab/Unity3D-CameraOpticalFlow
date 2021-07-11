using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bonjour.Vision.Ressources
{
    [CreateAssetMenu(fileName = "AverageVelocityParams", menuName = "ScriptableObject/AverageVelocity Ressource Set")]
    public sealed class AverageVelocityRessourceSet : ScriptableObject
    {
        [Tooltip("Define the compute shader to use computing the average Veolcity")] public ComputeShader averageVelocityCS;
    }
}
