namespace EtAlii.FracturedPlanet.Navigation
{
    using UnityEngine;

    public class MarchingCubesMeshBuilder
    {
        private Vector3[] _vertices;
        private int[] _triangles;
        private readonly float _isoLevel;

        private int _vertexIndex;

        private Vector3[] _vertexList;
        private readonly Mesh _mesh;

        public MarchingCubesMeshBuilder(float isoLevel)
        {
            _isoLevel = isoLevel;

            _mesh = new Mesh();

            _vertexIndex = 0;

            _vertexList = new Vector3[12];
        }

        private Vector3 VertexInterpolate(Vector3 p1, Vector3 p2, float v1, float v2)
        {
            if (Mathf.Abs(_isoLevel - v1) < 0.000001f)
            {
                return p1;
            }

            if (Mathf.Abs(_isoLevel - v2) < 0.000001f)
            {
                return p2;
            }

            if (Mathf.Abs(v1 - v2) < 0.000001f)
            {
                return p1;
            }

            var mu = (_isoLevel - v1) / (v2 - v1);

            var p = p1 + mu * (p2 - p1);

            return p;
        }

        private void March(Voxel[] voxels, int cubeIndex)
        {
            var edgeIndex = LookupTables.EdgeTable[cubeIndex];

            _vertexList = GenerateVertexList(voxels, edgeIndex);

            var row = LookupTables.TriangleTable[cubeIndex];

            for (var i = 0; i < row.Length; i += 3)
            {
                _vertices[_vertexIndex] = _vertexList[row[i + 0]];
                _triangles[_vertexIndex] = _vertexIndex;
                _vertexIndex++;

                _vertices[_vertexIndex] = _vertexList[row[i + 1]];
                _triangles[_vertexIndex] = _vertexIndex;
                _vertexIndex++;

                _vertices[_vertexIndex] = _vertexList[row[i + 2]];
                _triangles[_vertexIndex] = _vertexIndex;
                _vertexIndex++;
            }
        }

        private Vector3[] GenerateVertexList(Voxel[] voxels, int edgeIndex)
        {
            for (var i = 0; i < 12; i++)
            {
                if ((edgeIndex & (1 << i)) == 0) continue;
                
                var edgePair = LookupTables.EdgeIndexTable[i];
                var edge1 = edgePair[0];
                var edge2 = edgePair[1];

                var voxel1 = voxels[edge1];
                var voxel2 = voxels[edge2];

                _vertexList[i] = VertexInterpolate(voxel1.localPosition, voxel2.localPosition, voxel1.density, voxel2.density);
            }

            return _vertexList;
        }

        private int CalculateCubeIndex(Voxel[] voxels, float iso)
        {
            var cubeIndex = 0;

            for (var i = 0; i < 8; i++)
            {
                if (voxels[i].density < iso)
                {
                    cubeIndex |= 1 << i;
                }
            }

            return cubeIndex;
        }

        public Mesh Build(Voxel[,,] voxels)
        {
            var cubeIndexes = GenerateCubeIndexes(voxels);
            var vertexCount = GenerateVertexCount(cubeIndexes);

            if (vertexCount <= 0)
            {
                return new Mesh();
            }

            _vertices = new Vector3[vertexCount];
            _triangles = new int[vertexCount];

            for (var x = 0; x < voxels.GetLength(0) - 1; x++)
            {
                for (var y = 0; y < voxels.GetLength(1) - 1; y++)
                {
                    for (var z = 0; z < voxels.GetLength(2) - 1; z++)
                    {
                        var cubeIndex = cubeIndexes[x, y, z];
                        if (cubeIndex == 0 || cubeIndex == 255) continue;

                        March(GetPoints(x, y, z, voxels), cubeIndex);
                    }
                }
            }

            _vertexIndex = 0;

            _mesh.Clear();

            _mesh.vertices = _vertices;
            _mesh.SetTriangles(_triangles, 0);
            _mesh.RecalculateNormals();

            return _mesh;
        }

        private Voxel[] GetPoints(int x, int y, int z, Voxel[,,] voxels)
        {
            var initVoxels = new Voxel[8];

            for (var i = 0; i < 8; i++)
            {
                var p = voxels[
                    x + LookupTables.CubePointsX[i], 
                    y + LookupTables.CubePointsY[i],
                    z + LookupTables.CubePointsZ[i]];
                initVoxels[i] = p;
            }

            return initVoxels;
        }

        private int[,,] GenerateCubeIndexes(Voxel[,,] voxels)
        {
            var cubeIndexes = new int[voxels.GetLength(0) - 1, voxels.GetLength(1) - 1, voxels.GetLength(2) - 1];

            for (var x = 0; x < voxels.GetLength(0) - 1; x++)
            {
                for (var y = 0; y < voxels.GetLength(1) - 1; y++)
                {
                    for (var z = 0; z < voxels.GetLength(2) - 1; z++)
                    {
                        var initVoxels = GetPoints(x, y, z, voxels);

                        cubeIndexes[x, y, z] = CalculateCubeIndex(initVoxels, _isoLevel);
                    }
                }
            }
            return cubeIndexes;
        }

        private int GenerateVertexCount(int[,,] cubeIndexes)
        {
            var vertexCount = 0;

            for (var x = 0; x < cubeIndexes.GetLength(0); x++)
            {
                for (var y = 0; y < cubeIndexes.GetLength(1); y++)
                {
                    for (var z = 0; z < cubeIndexes.GetLength(2); z++)
                    {
                        var cubeIndex = cubeIndexes[x, y, z];
                        var row = LookupTables.TriangleTable[cubeIndex];
                        vertexCount += row.Length;
                    }
                }
            }
            return vertexCount;
        }
    }
}