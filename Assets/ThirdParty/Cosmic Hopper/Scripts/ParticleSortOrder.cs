using UnityEngine;

namespace FlappyJump
{
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticleSortOrder : MonoBehaviour
    {
        public string sortingLayerName = "Default"; // Set to your desired sorting layer
        public int orderInLayer = -1;               // Set lower than your sprites to render behind

        void Start()
        {
            ParticleSystemRenderer psRenderer = GetComponent<ParticleSystemRenderer>();
            if (psRenderer != null)
            {
                psRenderer.sortingLayerName = sortingLayerName;
                psRenderer.sortingOrder = orderInLayer;
            }
            else
            {
            }
        }
    }
}