namespace Kathulhu
{
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;
    using System.Collections.Generic;
    using System;

    /// <summary>
    /// UIFloatingText allows a UI element to display dynamic floating text 
    /// ex: Combat damage texts
    /// 
    /// usage : GetComponent<UIFloatingText>().AddEntry
    /// </summary>
    public class UIFloatingText : UIFollowTransform
    {

        protected class Entry
        {
            public float timestamp;		            //Time the entry was added
            public float duration = 0f;		        //How long the entry will be stationary
            public float offset = 0f;	            //Distance the entry will travel while fading out
            public float value = 0;		            //float value used for merging numeric entries

            public RectTransform rectTransform;     //RectTransform of the entry
            public Text textComponent;	            //Text on the game object
            public CanvasRenderer canvasrenderer;	//CanvasRenderer on the game object

            public float expirationTime { get { return timestamp + duration; } }

            public void Reset()
            {
                timestamp = Time.realtimeSinceStartup;
                duration = 0;
                offset = 0;
                value = 0;
            }

        }

        /// <summary>
        /// Sorting comparison function for floating text entries.
        /// </summary>
        static int Comparison( Entry a, Entry b )
        {
            if ( a.expirationTime < b.expirationTime ) return -1;
            if ( a.expirationTime > b.expirationTime ) return 1;
            return 0;
        }

        /// <summary>
        /// Font to use on the text elements
        /// </summary>
        public Font font;//set in the inspector

        /// <summary>
        /// The size delta for the RectTransform of the texts that will be displayed
        /// </summary>
        public Vector2 textsSize = new Vector2( 300, 20 );//set in the inspector

        /// <summary>
        /// Curve used to move entries with time.
        /// </summary>
        public AnimationCurve offsetCurve = new AnimationCurve( new Keyframe[] { new Keyframe( 0f, 0f ), new Keyframe( 3f, 40f ) } );

        /// <summary>
        /// Curve used to fade out entries with time.
        /// </summary>
        public AnimationCurve alphaCurve = new AnimationCurve( new Keyframe[] { new Keyframe( 1f, 1f ), new Keyframe( 3f, 0f ) } );

        /// <summary>
        /// Curve used to scale the entries.
        /// </summary>
        public AnimationCurve scaleCurve = new AnimationCurve( new Keyframe[] { new Keyframe( 0f, 0f ), new Keyframe( 0.25f, 1f ) } );

        /// <summary>
        /// Number of entries currently displayed
        /// </summary>
        public int Count
        {
            get { return _list.Count; }
        }

        private RectTransform _rect;
        private List<Entry> _list = new List<Entry>();
        private Queue<Entry> _entrypool = new Queue<Entry>();

        void Awake()
        {
            _rect = GetComponent<RectTransform>();
        }

        /// <summary>
        /// Returns an instance of an Entry either by using one in the pool or by creating one.
        /// </summary>    
        private Entry GetEntry()
        {
            Entry e;

            if ( _entrypool.Count > 0 )
            {
                e = _entrypool.Dequeue();
                e.Reset();
            }
            else e = CreateEntry();

            return e;
        }

        /// <summary>
        /// Creates an entry
        /// </summary>    
        private Entry CreateEntry()
        {
            Entry e = new Entry();
            e.Reset();

            GameObject go = new GameObject( "entry" );
            e.rectTransform = go.AddComponent<RectTransform>();

            e.rectTransform.SetParent( transform, false );
            e.rectTransform.sizeDelta = textsSize;

            e.textComponent = go.AddComponent<Text>();
            e.textComponent.alignment = TextAnchor.MiddleCenter;
            e.textComponent.font = font;
            e.textComponent.resizeTextForBestFit = true;

            go.AddComponent<Outline>();

            e.canvasrenderer = go.GetComponent<CanvasRenderer>();

            return e;
        }



        /// <summary>
        /// Public method to add an entry to the UIFloatingText
        /// </summary>
        /// <param name="obj">The object to write to display</param>
        /// <param name="c">The color of the text for this entry</param>
        /// <param name="duration">The time this entry will be displayed</param>
        public void AddEntry( object obj, Color c, float duration )
        {
            if ( !enabled )
                return;

            float value = 0;
            bool isFloatOrInt = obj is float || obj is int;

            //If entry is a value, we need to check if we can add the value to an existing entry instead of creating a new one
            if ( isFloatOrInt )
            {
                if ( obj is int )
                    value = Convert.ToSingle( obj );
                else
                    value = ( float )obj;

                //no need to add 0 to the an entry
                if ( value == 0 )
                    return;

                if ( TryToMergeValueEntry( value ) )
                    return;
            }

            Entry e = GetEntry();
            _list.Add( e );

            e.duration = duration;
            e.textComponent.color = c;
            e.value = value;

            e.textComponent.text = isFloatOrInt ? Mathf.RoundToInt( value ).ToString() : obj.ToString();

            _list.Sort( Comparison );
        }

        /// <summary>
        /// Delete an entry
        /// </summary>
        void Delete( Entry ent )
        {
            _list.Remove( ent );
            _entrypool.Enqueue( ent );
            ent.textComponent.enabled = false;
        }

        /// <summary>
        /// Will try to add a float value to an existing entry.
        /// </summary>
        /// <param name="value">The value to add</param>
        /// <returns>True if the value was added to an existing entry, false if no entry found to add the value</returns>
        private bool TryToMergeValueEntry( float value )
        {
            for ( int i = Count; i > 0; )
            {
                Entry currentEntry = _list[--i];
                if ( currentEntry.timestamp + 1f < Time.realtimeSinceStartup ) continue;
                if ( currentEntry.value != 0f )
                {
                    if ( currentEntry.value < 0f && value < 0f )//entry value and new entry value are both negative values, merge them
                    {
                        currentEntry.value += value;
                        currentEntry.textComponent.text = Mathf.RoundToInt( currentEntry.value ).ToString();
                        return true;
                    }
                    else if ( currentEntry.value > 0f && value > 0f )//entry value and new entry value are both positive values, merge them
                    {
                        currentEntry.value += value;
                        currentEntry.textComponent.text = Mathf.RoundToInt( currentEntry.value ).ToString();
                        return true;
                    }
                }
            }

            return false;
        }

        void Update()
        {

            Keyframe[] offsets = offsetCurve.keys;
            Keyframe[] alphas = alphaCurve.keys;
            Keyframe[] scales = scaleCurve.keys;

            float offsetEnd = offsets[offsets.Length - 1].time;
            float alphaEnd = alphas[alphas.Length - 1].time;
            float scalesEnd = scales[scales.Length - 1].time;
            float totalEnd = Mathf.Max( scalesEnd, Mathf.Max( offsetEnd, alphaEnd ) );

            //Set alpha value and clear expired entries
            for ( int i = Count; i > 0; )
            {
                Entry ent = _list[--i];
                float currentTime = Time.unscaledTime - ent.expirationTime;
                ent.offset = offsetCurve.Evaluate( currentTime );
                ent.canvasrenderer.SetAlpha( alphaCurve.Evaluate( currentTime ) );

                // Make the label scale in
                float s = scaleCurve.Evaluate( Time.unscaledTime - ent.timestamp );
                if ( s < 0.001f ) s = 0.001f;
                ent.rectTransform.localScale = new Vector3( s, s, s );

                // Delete the entry when needed
                if ( currentTime > totalEnd ) Delete( ent );
                else ent.textComponent.enabled = true;
            }

            float offset = 0f;

            //Set the position of entries
            for ( int i = Count; i > 0; )
            {
                Entry ent = _list[--i];
                offset = Mathf.Max( offset, ent.offset );
                ent.rectTransform.localPosition = new Vector3( 0f, offset, 0f );
                offset += Mathf.Round( ent.rectTransform.localScale.y * ent.textComponent.fontSize );
            }
        }
    }
}
