namespace Kathulhu
{
    using UnityEngine;
    using System.Collections;

    [RequireComponent( typeof( MeshRenderer ) )]
    [RequireComponent( typeof( MeshFilter ) )]
    [RequireComponent( typeof( MeshCollider ) )]
    public class HxTile : MonoBehaviour
    {
        public float Size
        {
            get { return _size; }
            set
            {
                _size = value;
            }
        }

        public HexCoord Coord
        {
            get { return _coord; }
            set
            {
                _coord = value;
                gameObject.name = _coord.ToString();


                Vector2 v = Coord.Position();
                Vector3 pos = new Vector3( v.x, 0, v.y );
                pos *= _size;

                Position = pos + 0.01f * Vector3.up;
            }
        }

        public Vector3 Position
        {
            get { return _position; }
            private set
            {
                _position = value;
                transform.position = _position;
            }

        }

        public Mesh Mesh
        {
            get { return _mesh; }
            set
            {
                _mesh = value;

                meshFilter.mesh = _mesh;
                meshCollider.sharedMesh = _mesh;
                meshRenderer.enabled = ( _mesh != null );
            }
        }

        public bool IsWalkable
        {
            get { return _isWalkable; }
            set
            {
                if ( value != _isWalkable )
                {
                    _isWalkable = value;
                }
            }
        }

        protected MeshFilter meshFilter;
        protected MeshRenderer meshRenderer;
        protected MeshCollider meshCollider;

        private float _size = 1;
        private HexCoord _coord;
        private Mesh _mesh;
        private bool _isWalkable;

        private Vector3 _position;

        protected virtual void Awake()
        {
            meshFilter = GetComponent<MeshFilter>();
            meshRenderer = GetComponent<MeshRenderer>();
            meshCollider = GetComponent<MeshCollider>();
        }

    }
}
