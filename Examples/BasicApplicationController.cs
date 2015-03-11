using UnityEngine;
using System.Collections;
using Kathulhu;

public class BasicApplicationController : ApplicationController 
{

    [SerializeField]
    private string _nextScene;

    [SerializeField]
    private string[] _additionalScenesToLoad;

    [SerializeField]
    private bool _additive = false;

    [SerializeField]
    private bool _useLoadingScreen = true;

    protected override void Initialize()
    {
        base.Initialize();

        if ( string.IsNullOrEmpty( _nextScene ) )
            return;

        LoadScene( _nextScene, _additive, _additionalScenesToLoad, _useLoadingScreen );
    }
    

}
