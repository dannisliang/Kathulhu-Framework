using UnityEngine;
using System.Collections;
using Kathulhu;

public class SceneTransitionExampleLoadNextScene : MonoBehaviour {

    public string levelToLoad = "kathulhu_scenetransitions_02";
    public bool additive = false;


    IEnumerator Start()
    {
        yield return new WaitForSeconds( 0.5f );

        if ( additive )
            GameController.LoadScene( levelToLoad, true );
        else
            GameController.LoadScene( levelToLoad );
    }

}
