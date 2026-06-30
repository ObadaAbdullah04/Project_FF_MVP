namespace Project.UI
{
    using UnityEngine;

    public class OpenParentGateButton : MonoBehaviour
    {
        public void OpenParentGate()
        {
            if (MenuManager.Instance != null)
            {
                MenuManager.Instance.ShowParentGate();
            }
            else
            {
                Debug.LogError("MenuManager is missing! Cannot open Parent Gate.");
            }
        }
    }
}
