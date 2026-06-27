namespace Project.UI
{
    using UnityEngine;
    using Project.Core;
    using Project.Data;

    public class OpenParentGateButton : MonoBehaviour
    {
        [SerializeField] private GameSceneConfig _sceneConfig;
        [SerializeField] private string _parentGateSceneName = "Parent Gate";

        public void OpenParentGate()
        {
            if (SceneLoader.Instance != null)
            {
                string targetScene = (_sceneConfig != null) ? _sceneConfig.ParentGateSceneName : _parentGateSceneName;
                SceneLoader.Instance.LoadSceneAdditively(targetScene, null);
            }
        }
    }
}
