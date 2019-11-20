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
        private Point[] _initPoints;
        private readonly Mesh _mesh;
        private int[,,] _cubeIndexes;

        private readonly Vector3 _zero = Vector3.zero;

        public MarchingCubesMeshBuilder(Point[,,] points, float isoLevel, int seed)
        {
            _isoLevel = isoLevel;

            _mesh = new Mesh();

            _vertexIndex = 0;

            _vertexList = new Vector3[12];
            _initPoints = new Point[8];
            _cubeIndexes = new int[points.GetLength(0) - 1, points.GetLength(1) - 1, points.GetLength(2) - 1];
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

        private void March(Point[] points, int cubeIndex)
        {
            var edgeIndex = LookupTables.EdgeTable[cubeIndex];

            _vertexList = GenerateVertexList(points, edgeIndex);

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

        private Vector3[] GenerateVertexList(Point[] points, int edgeIndex)
        {
            for (var i = 0; i < 12; i++)
            {
                if ((edgeIndex & (1 << i)) != 0)
                {
                    var edgePair = LookupTables.EdgeIndexTable[i];
                    var edge1 = edgePair[0];
                    var edge2 = edgePair[1];

                    var point1 = points[edge1];
                    var point2 = points[edge2];

                    _vertexList[i] = VertexInterpolate(point1.localPosition, point2.localPosition, point1.density,
                        point2.density);
                }
            }

            return _vertexList;
        }

        private int CalculateCubeIndex(Point[] points, float iso)
        {
            var cubeIndex = 0;

            for (var i = 0; i < 8; i++)
                if (points[i].density < iso)
                    cubeIndex |= 1 << i;

            return cubeIndex;
        }

        public Mesh Build(Point[,,] points)
        {
            _cubeIndexes = GenerateCubeIndexes(points);
            var vertexCount = GenerateVertexCount(_cubeIndexes);

            if (vertexCount <= 0)
            {
                return new Mesh();
            }

            _vertices = new Vector3[vertexCount];
            _triangles = new int[vertexCount];

            for (var x = 0; x < points.GetLength(0) - 1; x++)
            {
                for (var y = 0; y < points.GetLength(1) - 1; y++)
                {
                    for (var z = 0; z < points.GetLength(2) - 1; z++)
                    {
                        var cubeIndex = _cubeIndexes[x, y, z];
                        if (cubeIndex == 0 || cubeIndex == 255) continue;

                        March(GetPoints(x, y, z, points), cubeIndex);
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

        private Point[] GetPoints(int x, int y, int z, Point[,,] points)
        {
            for (var i = 0; i < 8; i++)
            {
                var p = points[x + LookupTables.CubePointsX[i], y + LookupTables.CubePointsY[i],
                    z + LookupTables.CubePointsZ[i]];
                _initPoints[i] = p;
            }

            return _initPoints;
        }

        private int[,,] GenerateCubeIndexes(Point[,,] points)
        {
            for (var x = 0; x < points.GetLength(0) - 1; x++)
            {
                for (var y = 0; y < points.GetLength(1) - 1; y++)
                {
                    for (var z = 0; z < points.GetLength(2) - 1; z++)
                    {
                        _initPoints = GetPoints(x, y, z, points);

                        _cubeIndexes[x, y, z] = CalculateCubeIndex(_initPoints, _isoLevel);
                    }
                }
            }

            return _cubeIndexes;
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