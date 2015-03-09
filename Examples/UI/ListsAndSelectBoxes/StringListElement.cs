using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Kathulhu;

[RequireComponent( typeof( Text ) )]
public class StringListElement : BaseListElement<string>
{

    private Text _text;

    protected override void Awake()
    {
        base.Awake();

        _text = GetComponent<Text>();
    }

    public override void UpdateElement()
    {
        _text.text = Data;
    }
}
