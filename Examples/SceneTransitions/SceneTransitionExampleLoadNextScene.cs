using UnityEngine;
using System.Collections;
using Kathulhu;

public class SceneTransitionExampleLoadNextScene : MonoBehaviour {

    public string levelToLoad = "kathulhu_scenetransitions_02";

    public string[] additionalLevelsToLoad = new string[] { "kathulhu_scenetransitions_02_add" };
    public bool additive = false;
    public bool useLoadingScreen = true;


    IEnumerator Start()
    {
        yield return new WaitForSeconds( 0.5f );

        GameController.LoadScene( levelToLoad, additive, additionalLevelsToLoad, useLoadingScreen );
    }

}
