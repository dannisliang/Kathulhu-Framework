namespace Kathulhu
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;

    public class HxGrid : MonoBehaviour
    {
        [SerializeField]
        private GameObject _hxTilePrefab;//set in the inspector

        public float TileSize
        {
            get { return _tileSize; }
            set
            {
                _tileSize = value;
                TileHeight = _tileSize * 2;
                TileWidth = ( Mathf.Sqrt( 3 ) / 2f ) * TileHeight;
                TileHorizontalDistance = TileWidth;
                TileVerticalDistance = ( 3f / 4f ) * TileHeight;
            }
        }

        public float TileHeight
        {
            get;
            private set;
        }

        public float TileWidth
        {
            get;
            private set;
        }

        public float TileHorizontalDistance
        {
            get;
            private set;
        }

        public float TileVerticalDistance
        {
            get;
            private set;
        }

        public Mesh TileMesh
        {
            get;
            private set;
        }

        public List<HxTile> Tiles
        {
            get { return _tiles; }
        }

        private float _tileSize = 1;
        private float raycastOriginHeight = 100;

        private List<HxTile> _tiles = new List<HxTile>();

        public void GenerateGrid( int radius )
        {
            Tiles.Clear();

            GenerateTileMesh();

            for ( int x = -radius; x <= radius; x++ )
            {
                for ( int y = -radius; y <= radius; y++ )
                {
                    for ( int z = -radius; z <= radius; z++ )
                    {
                        if ( x + y + z == 0 )
                        {
                            HexCoord coord = new HexCoord( x, z );

                            bool validPosition = true;
                            foreach ( var corner in coord.Corners() )
                            {
                                Vector3 pos = new Vector3( corner.x, 0, corner.y ) * TileSize;
                                validPosition = validPosition & RaycastIsValid( pos );
                                if ( !validPosition ) break;
                            }
                            if ( validPosition )
                            {
                                HxTile tile = CreateHxTile();
                                Tiles.Add( tile );
                                tile.Size = TileSize;
                                tile.Coord = coord;
                                tile.Mesh = TileMesh;
                                tile.IsWalkable = true;
                            }

                        }
                    }
                }
            }
        }

        void GenerateTileMesh()
        {
            int numTiles = 1;
            int numTris = numTiles * 4;
            int numVerts = numTiles * 6;

            // Generate the mesh data
            Vector3[] vertices = new Vector3[numVerts];
            Vector3[] normals = new Vector3[numVerts];
            Vector2[] uv = new Vector2[numVerts];
            int[] triangles = new int[numTris * 3];


            Vector3[] corners = new Vector3[6];
            float angle;
            for ( int i = 0; i < 6; i++ )
            {
                angle = 2 * Mathf.PI / 6 * ( i + 0.5f );
                corners[i] = new Vector3( TileSize * Mathf.Cos( angle ), 0, TileSize * Mathf.Sin( angle ) );
            }

            //corners vertices
            vertices[0] = corners[0];
            vertices[1] = corners[1];
            vertices[2] = corners[2];
            vertices[3] = corners[3];
            vertices[4] = corners[4];
            vertices[5] = corners[5];

            //corners vertices
            uv[0] = new Vector2( 1, 1f / 4f );
            uv[1] = new Vector2( 0.5f, 0 );
            uv[2] = new Vector2( 0, 1f / 4f );
            uv[3] = new Vector2( 0, 3f / 4f );
            uv[4] = new Vector2( 0.5f, 1 );
            uv[5] = new Vector2( 1, 3f / 4f );

            //pointy-topped hexs
            //triangle A
            triangles[0] = 1;
            triangles[1] = 0;
            triangles[2] = 2;
            //triangle B
            triangles[3] = 2;
            triangles[4] = 0;
            triangles[5] = 5;
            //triangle C
            triangles[6] = 2;
            triangles[7] = 5;
            triangles[8] = 3;
            //triangle D
            triangles[9] = 3;
            triangles[10] = 5;
            triangles[11] = 4;

            //All normals point up
            for ( int i = 0; i < normals.Length; i++ ) normals[i] = Vector3.up;

            Mesh _HexagonTileMesh = new Mesh();
            _HexagonTileMesh.vertices = vertices;
            _HexagonTileMesh.triangles = triangles;
            _HexagonTileMesh.normals = normals;
            _HexagonTileMesh.uv = uv;

            TileMesh = _HexagonTileMesh;
        }

        HxTile CreateHxTile()
        {
            GameObject go = Instantiate( _hxTilePrefab ) as GameObject;
            go.transform.parent = transform;

            return go.GetComponent<HxTile>();
        }


        public void UpdateGrid()
        {
            //update the Mesh of all HxTileViewModels
            //IEnumerator<HxTileViewModel> e = hxGrid.Tiles.GetEnumerator();
            //while ( e.MoveNext() )
            //{
            //    e.Current.ResetPosition();
            //    e.Current.Mesh = hxGrid.TileMesh;
            //}
        }

        public bool RaycastIsValid( Vector3 point )
        {
            RaycastHit hit;
            Ray ray = new UnityEngine.Ray( point + raycastOriginHeight * Vector3.up, -Vector3.up );
            //check for floor
            if ( Physics.Raycast( ray, out hit, raycastOriginHeight + 0.5f, 1 << LayerMask.NameToLayer( "Floor" ) ) )
            {
                //check for obstacle
                if ( !Physics.Raycast( ray, out hit, raycastOriginHeight + 0.5f, 1 << LayerMask.NameToLayer( "Obstacles" ) ) )
                {
                    return true;
                }
            }
            return false;
        }

    }
}
