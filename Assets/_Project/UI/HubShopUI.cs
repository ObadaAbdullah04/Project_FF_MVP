namespace Project.UI
{
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class HubShopUI : MonoBehaviour
    {
        public void OnPlayGameClicked(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
