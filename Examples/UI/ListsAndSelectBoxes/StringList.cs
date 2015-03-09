
using UnityEngine;
using System.Collections;
using Kathulhu;
using System;

public class StringList : BaseList<string>
{    

    protected override void Awake()
    {
        base.Awake();

        foreach ( var ancient in Enum.GetValues(typeof(CthulhuMythosDeities)) )
            Add( ancient.ToString() );
    }

}

