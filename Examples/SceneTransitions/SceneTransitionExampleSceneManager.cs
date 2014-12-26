using UnityEngine;
using System.Collections;
using Kathulhu;

public class SceneTransitionExampleSceneManager : SceneManager {

    public override IEnumerator Load()
    {
        LoadSceneProgressUpdateEvent evt = new LoadSceneProgressUpdateEvent();

        evt.progress = 0f;
        evt.message = "Initiating scene load.";
        EventDispatcher.Event( evt );
        yield return new WaitForSeconds( 1 );
        evt.progress = 0.33f;
        evt.message = "Loading something.";
        EventDispatcher.Event( evt );
        yield return new WaitForSeconds( 1 );
        evt.progress = 0.66f;
        evt.message = "Loading another something.";
        EventDispatcher.Event( evt );
        yield return new WaitForSeconds( 1 );
        evt.progress = 1f;
        evt.message = "Loading completed! Entering the scene";
        EventDispatcher.Event( evt );
        yield return new WaitForSeconds( 1f );
    }

}
