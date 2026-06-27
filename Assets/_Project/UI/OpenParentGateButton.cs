namespace Project.UI
{
    using UnityEngine;
    using Project.Core;

    public class OpenParentGateButton : MonoBehaviour
    {
        [SerializeField] private string _parentGateSceneName = "Parent Gate";

        public void OpenParentGate()
        {
            if (SceneLoader.Instance != null)
            {
                SceneLoader.Instance.LoadSceneAdditively(_parentGateSceneName, null);
            }
        }
    }
}
