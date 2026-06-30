using System;
using UnityEngine;
using UnityEngine.Advertisements;
namespace Templates.ColorCube
{


public class UnityAdsExample : MonoBehaviour//, IUnityAdsListener 
{   //DON'T FORGET TO UNCOMMENT "IUnityAdsListener" IN LINE ABOVE! Line above should look like this: public class UnityAdsExample : MonoBehaviour, IUnityAdsListener 

    // string gameId = "PUT YOUR GAME ID HERE";

   /* void Start () {
        Advertisement.AddListener (this);
        Advertisement.Initialize (gameId);
    }
*/

	public void ShowVideos()
    {
        print("Open the script UnityAdsExample and uncomment all code (if you don't have unity ads imported please download it from asset store) and put your game id. That's it!");
        print("If you want to use other ad network just call GameObject.Find(\"Canvas\").GetComponent<MenuSelect>().GameContinue(); when on ad finish event.");
        //Advertisement.Show();
    }

   /* public void OnUnityAdsDidFinish (string placementId, ShowResult showResult) {
        // Define conditional logic for each ad completion status:
        if (showResult == ShowResult.Finished) {
            GameObject.Find("Canvas").GetComponent<MenuSelect>().GameContinue();
        } else if (showResult == ShowResult.Skipped) {
       
        } else if (showResult == ShowResult.Failed) {
            
        }
    }

    public void OnUnityAdsReady (string placementId) {

    }

    public void OnUnityAdsDidError (string message) {

    }

    public void OnUnityAdsDidStart (string placementId) {

    }*/ 
}
}
