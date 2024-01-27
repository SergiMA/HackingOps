using UnityEngine;

namespace HackingOps.Audio.Footsteps
{
    public class TerrainDetector
    {
        private TerrainData _terrainData;
        private int _alphamapWidth;
        private int _alphamapHeight;
        private float[,,] _splatmapData;
        private int _texturesAmount;

        public TerrainDetector()
        {
            _terrainData = Terrain.activeTerrain.terrainData;
            _alphamapWidth = _terrainData.alphamapWidth;
            _alphamapHeight = _terrainData.alphamapHeight;

            _splatmapData = _terrainData.GetAlphamaps(0, 0, _alphamapWidth, _alphamapHeight);
            _texturesAmount = _splatmapData.Length / (_alphamapWidth * _alphamapHeight);
        }

        private Vector3 ConvertToSplatMapCoordinate(Vector3 worldPosition)
        {
            Vector3 splatPosition = new();
            Terrain terrain = Terrain.activeTerrain;
            Vector3 terrainPosition = terrain.transform.position;

            float relativePositionX = worldPosition.x - terrainPosition.x;
            float relativePositionZ = worldPosition.z - terrainPosition.z;

            float terrainSizeX = terrain.terrainData.size.x;
            float terrainSizeZ = terrain.terrainData.size.z;

            int terrainAlphamapWidth = terrain.terrainData.alphamapWidth;
            int terrainAlphamapHeight = terrain.terrainData.alphamapHeight;

            splatPosition.x = (relativePositionX / terrainSizeX) * terrainAlphamapWidth;
            splatPosition.z = (relativePositionZ / terrainSizeZ) * terrainAlphamapHeight;

            return splatPosition;
        }

        public int GetActiveTerrainTextureIndex(Vector3 position)
        {
            Vector3 terrainCoordinates = ConvertToSplatMapCoordinate(position);
            int activeTerrainIndex = 0;
            float largestOpacity = 0f;

            for (int i = 0; i < _texturesAmount; i++)
            {
                if (largestOpacity < _splatmapData[(int)terrainCoordinates.z, (int)terrainCoordinates.x, i])
                {
                    activeTerrainIndex = i;
                    largestOpacity = _splatmapData[(int)terrainCoordinates.z, (int)terrainCoordinates.x, i];
                }
            }

            return activeTerrainIndex;
        }
    }
}