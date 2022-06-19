using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

// ReSharper disable once CheckNamespace
public class TilesMapGenerator : MonoBehaviour
{
    [Header("[Tiles prefabs]")]
    public List<GameObject> tiles;

    [Header("[Map Parameters]")]
    public string mapName;
    public Vector3 mapPosition;
    [Range(10,100)]
    public int mapSize;
    public GeneratorParametr holesCount;
    public GeneratorParametr holesSizes;
    public GeneratorParametr heightsCount;
    public GeneratorParametr heightsSizes;
    public GeneratorParametr maxHeight;
    public EnableDisable heightSmoothing;

    [Header("[Map Filling]")]
    public GeneratorParametr additionalFilling;
    public List<GameObject> treesPrefabs;
    public List<GameObject> littleStonesPrefabs;
    public List<GameObject> bigStonesPrefabs;
    public List<GameObject> bushsPrefabs;
    public List<GameObject> grassPrefabs;
    public List<GameObject> branchsPrefabs;
    public List<GameObject> logsPrefabs;

    [Header("[Map points of interest (POI)]")]
    public EnableDisable contentOnMap;
    [Range(2,10)]
    public int poiCount;
    public GameObject startTile;
    public GameObject endTile;
    public List<GameObject> interestPointTiles;

    [Header("[Map roads between POI's]")]
    public EnableDisable roads;
    [Range(0, 100)]
    public int roadsBetweenPOI;
    [Range(0, 100)]
    public int roadsFilling;
    [Range(0, 100)]
    public int roadsFenceChance;
    public GameObject roadStraight;
    public GameObject roadRotate;
    public GameObject roadCrossroad;
    public GameObject roadTriple;
    public GameObject roadEnd;
    public List<GameObject> roadBridges;

    [Header("Tap this checkbox to generate in play mode")]
    public bool TapToGenerate;

    private MapTile[,] _tiles;
    private GameObject _mapObject;

    private void FixedUpdate()
    {
        if (TapToGenerate)
        {
            if (ScalerSystem.Instance == null || ScalerSystem.Instance.mapReady)
            {
                TapToGenerate = false;
                StartGenerator();
            }
        }
    }

    private void StartGenerator()
    {
        StartCoroutine(NewMap());
    }

    public IEnumerator NewMap()
    {
        if(ScalerSystem.Instance != null)
        {
            ScalerSystem.Instance.ReverseScaling();
            while(!ScalerSystem.Instance.reversed)
            {
                yield return new WaitForSeconds(0.1f);
            }
        }

        if (_mapObject != null)
        {
            Destroy(_mapObject);
        }

        yield return new WaitForEndOfFrame();

        GenerateMap(mapPosition);

        if (ScalerSystem.Instance != null)
        {
            var meshes = _mapObject.GetComponentsInChildren<MeshRenderer>();
            foreach (var mr in meshes) mr.enabled = false;
        }

        yield return new WaitForEndOfFrame();

        AdditionalFilling(mapSize);

        if(ScalerSystem.Instance != null)
        {
            ScalerSystem.Instance.StartScaling(_mapObject.transform);

            var meshes = _mapObject.GetComponentsInChildren<MeshRenderer>();
            foreach (var mr in meshes) mr.enabled = true;
        }

        for(var i = 0; i<_mapObject.transform.childCount; i++)
        {
            var thisPos = _mapObject.transform.GetChild(i).localPosition;
            thisPos.x -= mapSize * 2f / 2f-1f;
            thisPos.z -= mapSize * 2f / 2f-1f;
            _mapObject.transform.GetChild(i).localPosition = thisPos;
        }
    }

    private void GenerateMap(Vector3 mapPos)
    {
        var mapObject = new GameObject
        {
            name = mapName,
            transform =
            {
                parent = transform,
                position = mapPos
            }
        };

        _tiles = new MapTile[mapSize, mapSize];
        for (var xPos = 0; xPos < mapSize; xPos++)
        {
            for (var zPos = 0; zPos < mapSize; zPos++)
            {
                _tiles[xPos, zPos] = new MapTile();
            }
        }

        //ROADS AND POI CREATING ----------------------------------------------
        var pointsOfInterest = new List<int[]>();

        var leftDownCorner = new Vector2(0, 0);
        var rightTopCorner = new Vector2(mapSize, mapSize);
        var maxMapDistance = Vector2.Distance(leftDownCorner, rightTopCorner);
        if (contentOnMap == EnableDisable.Enabled)
        {
            for (var i = 0; i < poiCount; i++)
            {
                var trys = 0;
                while (true)
                {
                    trys++;
                    var newPoi = new[] { Random.Range(0, mapSize), Random.Range(0, mapSize) };

                    if (!pointsOfInterest.Contains(newPoi))
                    {
                        var minDistance = -1f;

                        foreach (var pointOfInterest in pointsOfInterest)
                        {
                            var firstPoint = new Vector2(pointOfInterest[0], pointOfInterest[1]);
                            var secondPoint = new Vector2(newPoi[0], newPoi[1]);
                            var distance = Vector2.Distance(firstPoint, secondPoint);
                            if (distance < minDistance || minDistance < 0f)
                            {
                                minDistance = distance;
                            }
                        }

                        if (minDistance > maxMapDistance / 4f || minDistance < 0f || trys > mapSize*mapSize)
                        {
                            pointsOfInterest.Add(newPoi);
                            _tiles[newPoi[0], newPoi[1]].HasRoad = true;
                            break;
                        }
                    }
                }
            }

            if (roads == EnableDisable.Enabled)
            {
                for (var xPos = 0; xPos < pointsOfInterest.Count; xPos++)
                {
                    for (var zPos = xPos; zPos < pointsOfInterest.Count; zPos++)
                    {
                        if (xPos != zPos)
                        {
                            var createThisConnection = true;

                            if(xPos == 0 && zPos == 1)
                            {

                            }
                            else
                            {
                                createThisConnection = Random.Range(0, 100) < roadsBetweenPOI;
                            }

                            if (createThisConnection)
                            {
                                var minX = pointsOfInterest[xPos][0] < pointsOfInterest[zPos][0] ? pointsOfInterest[xPos][0] : pointsOfInterest[zPos][0];
                                var maxX = pointsOfInterest[xPos][0] < pointsOfInterest[zPos][0] ? pointsOfInterest[zPos][0] : pointsOfInterest[xPos][0];
                                var minZ = pointsOfInterest[xPos][1] < pointsOfInterest[zPos][1] ? pointsOfInterest[xPos][1] : pointsOfInterest[zPos][1];
                                var maxZ = pointsOfInterest[xPos][1] < pointsOfInterest[zPos][1] ? pointsOfInterest[zPos][1] : pointsOfInterest[xPos][1];

                                var down = Random.Range(0, 100) < 50;

                                bool left;
                                if (pointsOfInterest[xPos][1] > pointsOfInterest[zPos][1])
                                {
                                    if (pointsOfInterest[xPos][0] < pointsOfInterest[zPos][0])
                                    {
                                        left = down;
                                    }
                                    else
                                    {
                                        left = !down;
                                    }
                                }
                                else
                                {
                                    if (pointsOfInterest[zPos][0] < pointsOfInterest[xPos][0])
                                    {
                                        left = down;
                                    }
                                    else
                                    {
                                        left = !down;
                                    }
                                }

                                for (var p = minX; p <= maxX; p++)
                                {
                                    if (down)
                                    {
                                        _tiles[p, minZ].HasRoad = true;
                                    }
                                    else
                                    {
                                        _tiles[p, maxZ].HasRoad = true;
                                    }
                                }
                                for (var p = minZ; p < maxZ; p++)
                                {
                                    if (left)
                                    {
                                        _tiles[minX, p].HasRoad = true;
                                    }
                                    else
                                    {
                                        _tiles[maxX, p].HasRoad = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        //-------------------------------------------------------------

        //HOLES CREATING -----------------------------------------------
        if (holesCount != GeneratorParametr.None)
        {
            var holesMultiplier = 0f;

            if (holesCount == GeneratorParametr.Random)
            {
                holesCount = (GeneratorParametr)Random.Range(1, 6);
            }

            if (holesCount == GeneratorParametr.VeryLow)
            {
                holesMultiplier = 0.1f;
            }
            else if (holesCount == GeneratorParametr.Low)
            {
                holesMultiplier = 0.2f;
            }
            else if (holesCount == GeneratorParametr.Medium)
            {
                holesMultiplier = 0.3f;
            }
            else if (holesCount == GeneratorParametr.High)
            {
                holesMultiplier = 0.4f;
            }
            else if (holesCount == GeneratorParametr.VeryHigh)
            {
                holesMultiplier = 0.5f;
            }

            var holesSizesMultiplier = 0f;

            if (holesSizes == GeneratorParametr.Random || holesSizes == GeneratorParametr.None)
            {
                holesSizes = (GeneratorParametr)Random.Range(1, 6);
            }

            if (holesSizes == GeneratorParametr.VeryLow)
            {
                holesSizesMultiplier = 0.1f;
            }
            else if (holesSizes == GeneratorParametr.Low)
            {
                holesSizesMultiplier = 0.25f;
            }
            else if (holesSizes == GeneratorParametr.Medium)
            {
                holesSizesMultiplier = 0.5f;
            }
            else if (holesSizes == GeneratorParametr.High)
            {
                holesSizesMultiplier = 0.85f;
            }
            else if (holesSizes == GeneratorParametr.VeryHigh)
            {
                holesSizesMultiplier = 1f;
            }

            var holesCountToCreate = Mathf.FloorToInt(mapSize * holesMultiplier);

            var maxHoleSize = Mathf.FloorToInt(mapSize * 0.3f * holesSizesMultiplier);

            for (var i = 0; i < holesCountToCreate; i++)
            {
                var hX = Random.Range(0, mapSize);
                var hZ = Random.Range(0, mapSize);
                CreateHoles(hX, hZ, maxHoleSize, 0);
            }
        }
        //-------------------------------------------------------------

        //HEIGHTS CREATING --------------------------------------------
        if (heightsCount != GeneratorParametr.None)
        {
            var heightsMultiplier = 0f;

            if (heightsCount == GeneratorParametr.Random)
            {
                heightsCount = (GeneratorParametr)Random.Range(1, 6);
            }

            heightsMultiplier = heightsCount switch
            {
                GeneratorParametr.VeryLow => 0.1f,
                GeneratorParametr.Low => 0.2f,
                GeneratorParametr.Medium => 0.3f,
                GeneratorParametr.High => 0.4f,
                GeneratorParametr.VeryHigh => 0.5f,
                _ => heightsMultiplier
            };

            var heightsSizesMultiplier = 0f;

            if (heightsSizes == GeneratorParametr.Random || heightsSizes == GeneratorParametr.None)
            {
                heightsSizes = (GeneratorParametr)Random.Range(1, 6);
            }

            heightsSizesMultiplier = heightsSizes switch
            {
                GeneratorParametr.VeryLow => 0.1f,
                GeneratorParametr.Low => 0.25f,
                GeneratorParametr.Medium => 0.5f,
                GeneratorParametr.High => 0.85f,
                GeneratorParametr.VeryHigh => 1f,
                _ => heightsSizesMultiplier
            };

            var heightsCountToCreate = Mathf.FloorToInt(mapSize * heightsMultiplier);

            var maxHeightSize = Mathf.FloorToInt(mapSize * 0.4f * heightsSizesMultiplier);

            if (maxHeight == GeneratorParametr.Random || maxHeight == GeneratorParametr.None)
            {
                maxHeight = (GeneratorParametr)Random.Range(1, 6);
            }

            var maxHeightInTiles = maxHeight switch
            {
                GeneratorParametr.VeryLow => 1,
                GeneratorParametr.Low => 2,
                GeneratorParametr.Medium => 3,
                GeneratorParametr.High => 4,
                GeneratorParametr.VeryHigh => 5,
                _ => 0
            };

            for (var i = 0; i < heightsCountToCreate; i++)
            {
                var hX = Random.Range(0, mapSize);
                var hZ = Random.Range(0, mapSize);
                RaiseHeight(hX, hZ, maxHeightSize, maxHeightInTiles, 0, heightSmoothing);
            }
        }

        //-------------------------------------------------------------

        //HEIGHT SMOOTHING----------------------------------------------
        if (heightSmoothing == EnableDisable.Enabled)
        {
            for (var xPos = 0; xPos < mapSize; xPos++)
            {
                for (var zPos = 0; zPos < mapSize; zPos++)
                {
                    SmoothHeights(xPos, zPos);
                }
            }
        }
        //-------------------------------------------------------------

        //ROADS--------------------------------------------------------
        var roadsSumHeights = 0f;
        for(var xPos = 0; xPos<mapSize; xPos++)
        {
            for(var zPos = 0; zPos<mapSize; zPos++)
            {
                roadsSumHeights += _tiles[xPos, zPos].Height;
            }
        }

        var roadsHeight = Mathf.CeilToInt(roadsSumHeights / (mapSize * mapSize));

        if (contentOnMap == EnableDisable.Enabled && roads == EnableDisable.Enabled)
        {
            foreach (var pointOfInterest in pointsOfInterest)
            {
                var xPos = pointOfInterest[1];
                var zPos = pointOfInterest[0];

                _tiles[xPos, zPos].HasHole = false;

                if (xPos + 1 < mapSize)
                {
                    _tiles[xPos + 1, zPos].Height = roadsHeight;
                    if (zPos + 1 < mapSize)
                    {
                        _tiles[xPos + 1, zPos + 1].Height = roadsHeight;
                    }
                    if (zPos - 1 > 0)
                    {
                        _tiles[xPos + 1, zPos - 1].Height = roadsHeight;
                    }
                }
                if (zPos + 1 < mapSize)
                {
                    _tiles[xPos, zPos + 1].Height = roadsHeight;
                }
                if (xPos - 1 > 0)
                {
                    _tiles[xPos - 1, zPos].Height = roadsHeight;
                    if (zPos + 1 < mapSize)
                    {
                        _tiles[xPos - 1, zPos + 1].Height = roadsHeight;
                    }
                    if (zPos - 1 > 0)
                    {
                        _tiles[xPos - 1, zPos - 1].Height = roadsHeight;
                    }
                }
                if (zPos - 1 > 0)
                {
                    _tiles[xPos, zPos - 1].Height = roadsHeight;
                }
            }

            for (var xPos = 0; xPos < mapSize; xPos++)
            {
                for (var zPos = 0; zPos < mapSize; zPos++)
                {
                    if (_tiles[xPos, zPos].HasRoad)
                    {
                        _tiles[xPos, zPos].Height = roadsHeight;
                        SmoothHeightDown(xPos, zPos);
                    }
                }
            }
        }
        //-------------------------------------------------------------

        //SPAWNING MAP-------------------------------------------------
        var x = 0f;
        var z = 0f;

        for (var xPos = 0; xPos < mapSize; xPos++)
        {
            for (var zPos = 0; zPos < mapSize; zPos++)
            {
                if (!_tiles[xPos, zPos].HasHole)
                {
                    SpawnTile(xPos, zPos, x, z, mapPosition, mapObject.transform); // , roadsMap[zPos, xPos]
                }

                x +=  2f;
            }
            z += 2f;
            x = 0f;
        }
        //-------------------------------------------------------------

        //POIS SPAWNING------------------------------------------------
        if (contentOnMap == EnableDisable.Enabled)
        {
            for (var i = 0; i < pointsOfInterest.Count; i++)
            {
                var xPos = pointsOfInterest[i][1];
                var zPos = pointsOfInterest[i][0];
                GameObject poiObj;
                if (i == 0)
                {
                    poiObj = Instantiate(startTile);
                }
                else if (i == 1)
                {
                    poiObj = Instantiate(endTile);
                }
                else
                {
                    poiObj = Instantiate(interestPointTiles[Random.Range(0, interestPointTiles.Count)]);
                }

                poiObj.transform.position = new Vector3(xPos * 2f, mapPos.y + _tiles[xPos, zPos].Height * 2f, zPos * 2f);
                poiObj.transform.parent = mapObject.transform;

                xPos = pointsOfInterest[i][0];
                zPos = pointsOfInterest[i][1];

                if (xPos + 1 < mapSize)
                {
                    if (_tiles[xPos + 1, zPos].HasRoad)
                    {
                        poiObj.transform.localEulerAngles = new Vector3(0f, -90f, 0f);
                    }
                }
                if (zPos + 1 < mapSize)
                {
                    if (_tiles[xPos, zPos + 1].HasRoad)
                    {
                        poiObj.transform.localEulerAngles = new Vector3(0f, -180f, 0f);
                    }
                }
                if (xPos - 1 > 0)
                {
                    if (_tiles[xPos - 1, zPos].HasRoad)
                    {
                        poiObj.transform.localEulerAngles = new Vector3(0f, -270f, 0f);
                    }
                }
                if (zPos - 1 > 0)
                {
                    if (_tiles[xPos, zPos - 1].HasRoad)
                    {
                        poiObj.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
                    }
                }
            }
        }
        //-------------------------------------------------------------

        //ROADS SPAWNING---------------------------------------------
        if (contentOnMap == EnableDisable.Enabled && roads == EnableDisable.Enabled)
        {
            var bridgeNumber = -1;
            if (roadBridges.Count > 0)
            {
                bridgeNumber = Random.Range(0, roadBridges.Count);
            }

            for (var xPos = 0; xPos < mapSize; xPos++)
            {
                for (var zPos = 0; zPos < mapSize; zPos++)
                {
                    if (_tiles[xPos, zPos].HasRoad)
                    {
                        var right = false;
                        var left = false;
                        var up = false;
                        var down = false;

                        if (xPos + 1 < mapSize) //RightTile checking
                        {
                            if (_tiles[xPos + 1, zPos].HasRoad && !_tiles[xPos + 1, zPos].HasHole) //RightTile road +
                            {
                                right = true;
                            }
                        }
                        if (xPos - 1 >= 0)
                        {
                            if (_tiles[xPos - 1, zPos].HasRoad && !_tiles[xPos - 1, zPos].HasHole)
                            {
                                left = true;
                            }
                        }
                        if (zPos + 1 < mapSize)
                        {
                            if (_tiles[xPos, zPos + 1].HasRoad && !_tiles[xPos, zPos + 1].HasHole)
                            {
                                up = true;
                            }
                        }
                        if (zPos - 1 >= 0)
                        {
                            if (_tiles[xPos, zPos - 1].HasRoad && !_tiles[xPos, zPos - 1].HasHole)
                            {
                                down = true;
                            }
                        }

                        if (up && down && !IsHole(xPos, zPos))
                        {
                            if (Random.Range(0, 100) < 100 - roadsFilling)
                            {
                                _tiles[xPos, zPos].HasRoad = false;
                            }
                        }
                        else if (left && right && !IsHole(xPos, zPos))
                        {
                            if (Random.Range(0, 100) < 100 - roadsFilling)
                            {
                                _tiles[xPos, zPos].HasRoad = false;
                            }
                        }
                    }
                }
            }

            for (var xPos = 0; xPos < mapSize; xPos++)
            {
                for (var zPos = 0; zPos < mapSize; zPos++)
                {
                    if (_tiles[xPos, zPos].HasRoad && RoadIsNotPOI(xPos, zPos, pointsOfInterest))
                    {
                        var yEulers = 0f;
                        var spawned = true;

                        GameObject road = null;

                        var right = false;
                        var left = false;
                        var up = false;
                        var down = false;

                        if (xPos + 1 < mapSize) //RightTile checking
                        {
                            if (_tiles[xPos + 1, zPos].HasRoad) //RightTile road +
                            {
                                right = true;
                            }
                        }
                        if (xPos - 1 >= 0)
                        {
                            if (_tiles[xPos - 1, zPos].HasRoad)
                            {
                                left = true;
                            }
                        }
                        if (zPos + 1 < mapSize)
                        {
                            if (_tiles[xPos, zPos + 1].HasRoad)
                            {
                                up = true;
                            }
                        }
                        if (zPos - 1 >= 0)
                        {
                            if (_tiles[xPos, zPos - 1].HasRoad)
                            {
                                down = true;
                            }
                        }

                        if (up && down && right && left)
                        {
                            road = Instantiate(roadCrossroad);
                            yEulers = 90f * Random.Range(0, 4);
                        }
                        else if (up && down && right)
                        {
                            road = Instantiate(roadTriple);
                            yEulers = 0f;
                        }
                        else if (up && left && right)
                        {
                            road = Instantiate(roadTriple);
                            yEulers = -90f;
                        }
                        else if (up && left && down)
                        {
                            road = Instantiate(roadTriple);
                            yEulers = -180f;
                        }
                        else if (right && left && down)
                        {
                            road = Instantiate(roadTriple);
                            yEulers = -270f;
                        }
                        else if (right && left)
                        {
                            if (_tiles[xPos, zPos].HasHole)
                            {
                                road = Instantiate(roadBridges[bridgeNumber]);
                                yEulers = 90f;
                            }
                            else
                            {
                                if (RoadIsNotPOI(xPos + 1, zPos, pointsOfInterest) && RoadIsNotPOI(xPos - 1, zPos, pointsOfInterest))
                                {
                                    road = Instantiate(roadStraight);
                                    if (Random.Range(0, 100) < roadsFenceChance)
                                    {
                                        road.transform.GetChild(1).gameObject.SetActive(true);
                                    }

                                    if (Random.Range(0, 100) < roadsFenceChance)
                                    {
                                        road.transform.GetChild(2).gameObject.SetActive(true);
                                    }
                                    yEulers = 90f;
                                }
                                else
                                {
                                    road = Instantiate(_tiles[xPos, zPos].HasHole ? roadBridges[bridgeNumber] : roadEnd);
                                    yEulers = RoadIsNotPOI(xPos + 1, zPos, pointsOfInterest) ? -90f : 90f;
                                }
                            }
                        }
                        else if (up && down)
                        {
                            if (_tiles[xPos, zPos].HasHole)
                            {
                                road = Instantiate(roadBridges[bridgeNumber]);
                                yEulers = 0f;
                            }
                            else
                            {
                                if (RoadIsNotPOI(xPos + 1, zPos, pointsOfInterest) && RoadIsNotPOI(xPos - 1, zPos, pointsOfInterest))
                                {
                                    road = Instantiate(roadStraight);
                                    if (Random.Range(0, 100) < roadsFenceChance)
                                    {
                                        road.transform.GetChild(1).gameObject.SetActive(true);
                                    }

                                    if (Random.Range(0, 100) < roadsFenceChance)
                                    {
                                        road.transform.GetChild(2).gameObject.SetActive(true);
                                    }
                                    yEulers = 0f;
                                }
                                else
                                {
                                    road = Instantiate(_tiles[xPos, zPos].HasHole ? roadBridges[bridgeNumber] : roadEnd);
                                    yEulers = RoadIsNotPOI(xPos + 1, zPos, pointsOfInterest) ? -90f : 90f;
                                }
                            }
                        }
                        else if (right && down)
                        {
                            road = Instantiate(roadRotate);
                            yEulers = 0f;
                        }
                        else if (right && up)
                        {
                            road = Instantiate(roadRotate);
                            yEulers = -90f;
                        }
                        else if (left && up)
                        {
                            road = Instantiate(roadRotate);
                            yEulers = 180f;
                        }
                        else if (left && down)
                        {
                            road = Instantiate(roadRotate);
                            yEulers = 90f;
                        }
                        else if (up)
                        {
                            road = Instantiate(_tiles[xPos, zPos].HasHole ? roadBridges[bridgeNumber] : roadEnd);
                            yEulers = 180f;
                        }
                        else if (down)
                        {
                            road = Instantiate(_tiles[xPos, zPos].HasHole ? roadBridges[bridgeNumber] : roadEnd);
                            yEulers = 0f;
                        }
                        else if (right)
                        {
                            road = Instantiate(_tiles[xPos, zPos].HasHole ? roadBridges[bridgeNumber] : roadEnd);
                            yEulers = -90f;
                        }
                        else if (left)
                        {
                            road = Instantiate(_tiles[xPos, zPos].HasHole ? roadBridges[bridgeNumber] : roadEnd);
                            yEulers = 90f;
                        }
                        else
                        {
                            spawned = false;
                        }

                        //Additional Tiles
                        if ((left && down) || (right && down) || (right && up) || (up && left) || (up && down && right && left) ||
                            (up && down && right) || (up && left && right) || (up && left && down) || (right && left && down))
                        {
                            if (_tiles[xPos, zPos].HasHole)
                            {
                                SpawnTile(xPos, zPos, xPos * 2, zPos * 2, mapPosition, mapObject.transform); // , true
                            }
                        }

                        if (spawned)
                        {
                            road.transform.position = new Vector3(xPos * 2f, mapPos.y + _tiles[xPos, zPos].Height * 2f, zPos * 2f);
                            road.transform.parent = mapObject.transform;
                            road.transform.localEulerAngles = new Vector3(0f, yEulers, 0f);
                        }
                    }
                }
            }
        }

        _mapObject = mapObject;
    }

    private void AdditionalFilling(int sizeOfMap)
    {

        if (additionalFilling == GeneratorParametr.Random)
        {
            additionalFilling = (GeneratorParametr)Random.Range(1, 6);
        }

        if (additionalFilling == GeneratorParametr.None)
        {
            return;
        }

        var countsCycle = (int)(sizeOfMap/5f) * (int)additionalFilling;

        var circlesRange = sizeOfMap/6f + sizeOfMap / 30f * (int)additionalFilling;

        var objectsCounts = sizeOfMap/2.5f + sizeOfMap/6 * (int)additionalFilling;

        //Debug.Log(countsCycle + " " + circlesRange + " " + objectsCounts);

        var treesPoints = new List<Vector3>();
        var bushsPoints = new List<Vector3>();
        var bigStonesPoints = new List<Vector3>();
        var grassPoint = new List<Vector3>();

        for (var a = 0; a < countsCycle; a++)
        {
            var circleTreesPos = new Vector3(Random.Range(0f, sizeOfMap * 2f), 15f, Random.Range(0f, sizeOfMap * 2f));

            for (var i = 0; i < objectsCounts; i++) //Trees
            {
                var rayPos = circleTreesPos;
                rayPos.x += Random.Range(-circlesRange, circlesRange);
                rayPos.z += Random.Range(-circlesRange, circlesRange);
                if (Physics.Raycast(rayPos, Vector3.down, out var hit, Mathf.Infinity))
                {
                    if (hit.transform.name.Contains("Tile") && IsPosAvailableByDistance(hit.point,treesPoints,1.5f) && IsPosNotInPOI(hit.point))
                    {
                        var tree = Instantiate(treesPrefabs[Random.Range(0, treesPrefabs.Count)], hit.point, Quaternion.identity, _mapObject.transform);
                        tree.transform.eulerAngles = new Vector3(Random.Range(-7.5f, 7.5f), Random.Range(0f, 360f), Random.Range(-7.5f, 7.5f));
                        treesPoints.Add(hit.point);
                    }
                }
            }

            for (var i = 0; i < objectsCounts/3; i++) //Bushs
            {
                var rayPos = circleTreesPos;
                rayPos.x += Random.Range(-circlesRange, circlesRange);
                rayPos.z += Random.Range(-circlesRange, circlesRange);
                if (!Physics.Raycast(rayPos, Vector3.down, out var hit, Mathf.Infinity) ||
                    !hit.transform.name.Contains("Tile") ||
                    !IsPosAvailableByDistance(hit.point, bushsPoints, 2f) ||
                    !IsPosInRangeOf(hit.point, treesPoints, 4f) ||
                    !IsPosNotInPOI(hit.point))
                {
                    continue;
                }

                var tree = Instantiate(bushsPrefabs[Random.Range(0, bushsPrefabs.Count)], hit.point, Quaternion.identity, _mapObject.transform);
                tree.transform.eulerAngles = new Vector3(0f, Random.Range(0f, 360f), 0f);
                bushsPoints.Add(hit.point);
            }

            for (var i = 0; i < objectsCounts * 4; i++) //Grass
            {
                var rayPos = circleTreesPos;
                rayPos.x += Random.Range(-circlesRange, circlesRange);
                rayPos.z += Random.Range(-circlesRange, circlesRange);
                if (!Physics.Raycast(rayPos, Vector3.down, out var hit, Mathf.Infinity) ||
                    !hit.transform.name.Contains("Tile") ||
                    !IsPosAvailableByDistance(hit.point, grassPoint, 0.25f) ||
                    !IsPosNotInPOI(hit.point))
                {
                    continue;
                }

                var tree = Instantiate(grassPrefabs[Random.Range(0, grassPrefabs.Count)], hit.point, Quaternion.identity, _mapObject.transform);
                tree.transform.eulerAngles = new Vector3(0f, Random.Range(0f, 360f), 0f);
                grassPoint.Add(hit.point);
            }

            for (var i = 0; i < objectsCounts; i++) //Little stones
            {
                var rayPos = circleTreesPos;
                rayPos.x += Random.Range(-circlesRange, circlesRange);
                rayPos.z += Random.Range(-circlesRange, circlesRange);
                if (!Physics.Raycast(rayPos, Vector3.down, out var hit, Mathf.Infinity) ||
                    !hit.transform.name.Contains("Tile") ||
                    !IsPosNotInPOI(hit.point))
                {
                    continue;
                }

                var tree = Instantiate(littleStonesPrefabs[Random.Range(0, littleStonesPrefabs.Count)], hit.point, Quaternion.identity, _mapObject.transform);
                tree.transform.eulerAngles = new Vector3(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
            }
        }


        for (var i = 0; i < objectsCounts * countsCycle*(int)additionalFilling; i++) //Grass
        {
            var rayPos = new Vector3(Random.Range(0f, sizeOfMap * 2f), 15f, Random.Range(0f, sizeOfMap * 2f));
            if (!Physics.Raycast(rayPos, Vector3.down, out var hit, Mathf.Infinity) ||
                !hit.transform.name.Contains("Tile") ||
                !IsPosAvailableByDistance(hit.point, grassPoint, 0.25f) ||
                !IsPosNotInPOI(hit.point))
            {
                continue;
            }

            var tree = Instantiate(grassPrefabs[Random.Range(0, grassPrefabs.Count)], hit.point, Quaternion.identity, _mapObject.transform);
            tree.transform.eulerAngles = new Vector3(0f, Random.Range(0f, 360f), 0f);
            grassPoint.Add(hit.point);
        }

        for (var i = 0; i < objectsCounts / 2; i++) //big stones
        {
            var rayPos = new Vector3(Random.Range(0f, sizeOfMap * 2f), 15f, Random.Range(0f, sizeOfMap * 2f));
            if (!Physics.Raycast(rayPos, Vector3.down, out var hit, Mathf.Infinity) ||
                !hit.transform.name.Contains("Tile") ||
                !IsPosAvailableByDistance(hit.point, bigStonesPoints, 10f) ||
                !IsPosInRangeOf(hit.point, treesPoints, 8f) ||
                !IsPosNotInPOI(hit.point))
            {
                continue;
            }

            var tree = Instantiate(bigStonesPrefabs[Random.Range(0, bigStonesPrefabs.Count)], hit.point, Quaternion.identity, _mapObject.transform);
            tree.transform.eulerAngles = new Vector3(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
            bigStonesPoints.Add(hit.point);
        }

        for (var i = 0; i < objectsCounts / 2; i++) //branchs
        {
            var rayPos = new Vector3(Random.Range(0f, sizeOfMap * 2f), 15f, Random.Range(0f, sizeOfMap * 2f));
            if (!Physics.Raycast(rayPos, Vector3.down, out var hit, Mathf.Infinity) ||
                !hit.transform.name.Contains("Tile") ||
                !IsPosAvailableByDistance(hit.point, bigStonesPoints, 10f) ||
                !IsPosInRangeOf(hit.point, treesPoints, 8f) ||
                !IsPosNotInPOI(hit.point))
            {
                continue;
            }

            var tree = Instantiate(branchsPrefabs[Random.Range(0, branchsPrefabs.Count)], hit.point, Quaternion.identity, _mapObject.transform);
            tree.transform.eulerAngles = new Vector3(0f, Random.Range(0f, 360f), 0f);
            //branchPoints.Add(hit.point);
        }

        for (var i = 0; i < objectsCounts / 2; i++) //logs
        {
            var rayPos = new Vector3(Random.Range(0f, sizeOfMap * 2f), 15f, Random.Range(0f, sizeOfMap * 2f));
            if (!Physics.Raycast(rayPos, Vector3.down, out var hit, Mathf.Infinity) ||
                !hit.transform.name.Contains("Tile") ||
                !IsPosAvailableByDistance(hit.point, bigStonesPoints, 10f) ||
                !IsPosInRangeOf(hit.point, treesPoints, 8f) ||
                !IsPosNotInPOI(hit.point))
            {
                continue;
            }

            var tree = Instantiate(logsPrefabs[Random.Range(0, logsPrefabs.Count)], hit.point, Quaternion.identity, _mapObject.transform);
            tree.transform.eulerAngles = new Vector3(0f, Random.Range(0f, 360f),0f);
            //logPoints.Add(hit.point);
        }
    }

    private bool IsPosNotInPOI(Vector3 posToCheck)
    {
        var x = Mathf.CeilToInt(posToCheck.x / 2f);
        var z = Mathf.CeilToInt(posToCheck.z / 2f);

        if (x >= 0 && z >= 0 && x < mapSize && z < mapSize)
        {
            return !_tiles[x, z].HasRoad;
        }

        return false;
    }

    private bool IsPosAvailableByDistance(Vector3 posToCheck, List<Vector3> otherPoses, float minDistance)
    {
        var minDistanceWeHas = -1f;

        foreach (var otherPose in otherPoses)
        {
            var distance = Vector3.Distance(posToCheck, otherPose);
            if (distance < minDistanceWeHas || minDistanceWeHas < 0f)
            {
                minDistanceWeHas = distance;
            }
        }

        return minDistanceWeHas > minDistance || minDistanceWeHas < 0f;
    }

    private bool IsPosInRangeOf(Vector3 posToCheck, List<Vector3> otherPoses, float needDistance)
    {
        var minDistanceWeHas = -1f;

        foreach (var otherPose in otherPoses)
        {
            var distance = Vector3.Distance(posToCheck, otherPose);
            if (distance < minDistanceWeHas || minDistanceWeHas < 0f)
            {
                minDistanceWeHas = distance;
            }
        }

        return minDistanceWeHas < needDistance;
    }

    private bool RoadIsNotPOI(int x, int z, List<int[]> poi)
    {
        var roadIsNotPoi = true;

        for (var i = 0; i < poi.Count; i++)
        {
            if(poi[i][0] == x && poi[i][1] == z)
            {
                roadIsNotPoi = false;
                break;
            }
        }

        return roadIsNotPoi;
    }

    private void SpawnTile(int xPos, int zPos, float x, float z, Vector3 mapPos, Transform parent) // , bool isRoad
    {
        var height = _tiles[xPos, zPos].Height;
        var newPart = Instantiate(tiles[Random.Range(0,tiles.Count)], parent, true);
        newPart.name = string.Concat(newPart.name.TakeWhile(c => c != '(')) + $" ({xPos:+00;-00} x {zPos:+00;-00})";
        var posY = mapPos.y;

        var heightForThisTile = height;

        if (heightForThisTile > 0)
        {
            posY += heightForThisTile * 2f;
        }
        newPart.transform.position = new Vector3(mapPos.x + x, posY, mapPos.z + z);

        if (heightForThisTile > 0)
        {
            newPart.transform.GetChild(1).localScale = new Vector3(1f, 1f + heightForThisTile * 1.2f, 1f);
            newPart.transform.GetChild(1).localPosition = new Vector3(0f, -0.7f * heightForThisTile, 0f);
        }

        var rotRandom = Random.Range(0, 4);
        if (rotRandom == 0)
        {
            newPart.transform.localEulerAngles = new Vector3(0f, 90f, 0f);
        }
        else if (rotRandom == 1)
        {
            newPart.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
        }
        else if (rotRandom == 2)
        {
            newPart.transform.localEulerAngles = new Vector3(0f, 270f, 0f);
        }
        else
        {
            newPart.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
        }
    }

    private void SmoothHeights(int x, int z)
    {
        var currentHeight = _tiles[x, z].Height;
        var heightDifference = 0;

        if (z - 1 > 0)
        {
            var difference = _tiles[x, z - 1].Height - currentHeight;
            if (difference > heightDifference)
            {
                heightDifference = difference;
            }
        }
        if (z + 1 < mapSize)
        {
            var difference = _tiles[x, z + 1].Height - currentHeight;
            if (difference > heightDifference)
            {
                heightDifference = difference;
            }
        }
        if (x - 1 > 0)
        {
            var difference = _tiles[x - 1, z].Height - currentHeight;
            if (difference > heightDifference)
            {
                heightDifference = difference;
            }
        }
        if (x + 1 < mapSize)
        {
            var difference = _tiles[x + 1, z].Height - currentHeight;
            if (difference > heightDifference)
            {
                heightDifference = difference;
            }
        }

        if (heightDifference > 1)
        {
            _tiles[x, z].Height = currentHeight + 1;
        }

        //return currentMap;
    }

    private void SmoothHeightDown(int x, int z)
    {
        var myHeight = _tiles[x, z].Height;

        if (z - 1 > 0)
        {
            if (_tiles[x, z - 1].Height > myHeight)
            {
                _tiles[x, z - 1].Height = Random.Range(myHeight, myHeight + 2);
                SmoothHeightDown(x, z - 1);
            }
        }
        if (z + 1 < mapSize)
        {
            if (_tiles[x, z + 1].Height > myHeight)
            {
                _tiles[x, z + 1].Height = Random.Range(myHeight, myHeight + 2);
                SmoothHeightDown(x, z + 1);
            }
        }
        if (x - 1 > 0)
        {
            if (_tiles[x-1, z].Height > myHeight)
            {
                _tiles[x-1, z].Height = Random.Range(myHeight, myHeight + 2);
                SmoothHeightDown(x - 1, z);
            }
        }
        if (x + 1 < mapSize)
        {
            if (_tiles[x + 1, z].Height > myHeight)
            {
                _tiles[x + 1, z].Height = Random.Range(myHeight, myHeight + 2);
                SmoothHeightDown(x + 1, z);
            }
        }
    }

    private void RaiseHeight(int x, int z, int maxSize, int tileMaxHeight, int iteration, EnableDisable hs)
    {
        if ((hs != EnableDisable.Enabled || IsNeighboringHole(x, z)) && hs != EnableDisable.Disabled)
        {
            return;
        }

        if (_tiles[x, z].Height < tileMaxHeight)
        {
            _tiles[x, z].Height += 1;
        }

        var chanceForAdditionalHeight = 100f;

        var anywayHeights = Mathf.FloorToInt(maxSize / 1.35f);

        if (iteration > anywayHeights)
        {
            chanceForAdditionalHeight = 100f - 100f / (maxSize - anywayHeights) * (iteration - anywayHeights);
        }

        iteration++;

        if (z - 1 > 0 && Random.Range(0, 100) < chanceForAdditionalHeight * Random.Range(0.75f,1f))
        {
            RaiseHeight(x, z - 1, maxSize, tileMaxHeight, iteration, hs);
        }
        if (z + 1 < mapSize && Random.Range(0, 100) < chanceForAdditionalHeight * Random.Range(0.75f, 1f))
        {
            RaiseHeight(x, z + 1, maxSize, tileMaxHeight, iteration, hs);
        }
        if (x - 1 > 0 && Random.Range(0, 100) < chanceForAdditionalHeight * Random.Range(0.75f, 1f))
        {
            RaiseHeight(x - 1, z, maxSize, tileMaxHeight, iteration, hs);
        }
        if (x + 1 < mapSize && Random.Range(0, 100) < chanceForAdditionalHeight * Random.Range(0.75f, 1f))
        {
            RaiseHeight(x + 1, z, maxSize, tileMaxHeight, iteration, hs);
        }
    }

    private void CreateHoles(int x, int z, int maxSize, int iteration)
    {
        if (!_tiles[x, z].HasHole)
        {
            _tiles[x, z].HasHole = true;

            var chanceForAdditionalHole = 100f;

            var anywayHoles = Mathf.FloorToInt(maxSize / 1.25f);

            if (iteration > anywayHoles)
            {
                chanceForAdditionalHole = 100f - 100f / (maxSize - anywayHoles) * (iteration - anywayHoles);
            }

            iteration++;

            if (iteration < maxSize)
            {
                if (z - 1 > 0 && Random.Range(0, 100) < chanceForAdditionalHole * Random.Range(0.75f, 1f))
                {
                    CreateHoles(x, z - 1, maxSize, iteration);
                }
                if (z + 1 < mapSize && Random.Range(0, 100) < chanceForAdditionalHole * Random.Range(0.75f, 1f))
                {
                    CreateHoles(x, z + 1, maxSize, iteration);
                }
                if (x - 1 > 0 && Random.Range(0, 100) < chanceForAdditionalHole * Random.Range(0.75f, 1f))
                {
                    CreateHoles(x - 1, z, maxSize, iteration);
                }
                if (x + 1 < mapSize && Random.Range(0, 100) < chanceForAdditionalHole * Random.Range(0.75f, 1f))
                {
                    CreateHoles(x + 1, z, maxSize, iteration);
                }
            }
        }
    }

    // private bool IsNeighboringHigher(int[,] currentMap, int x, int z)
    // {
    //     var isHigher = false;
    //     var myHeight = currentMap[x, z];
    //     if (z - 1 > 0)
    //     {
    //         if (currentMap[x, z - 1] > myHeight) isHigher = true;
    //     }
    //     if (z + 1 < mapSize)
    //     {
    //         if (currentMap[x, z + 1] > myHeight) isHigher = true;
    //     }
    //     if (x - 1 > 0)
    //     {
    //         if (currentMap[x - 1, z] > myHeight) isHigher = true;
    //     }
    //     if (x + 1 < currentMap.GetLength(0))
    //     {
    //         if (currentMap[x + 1, z] > myHeight) isHigher = true;
    //     }
    //     return isHigher;
    // }

    private bool IsNeighboringHole(int x, int z)
    {
        var isHole = false;

        if (z - 1 > 0)
        {
            if (_tiles[x,z-1].HasHole)
            {
                isHole = true;
            }
        }
        if (z + 1 < mapSize)
        {
            if (_tiles[x, z + 1].HasHole)
            {
                isHole = true;
            }
        }
        if (x - 1 > 0)
        {
            if (_tiles[x - 1, z].HasHole)
            {
                isHole = true;
            }
        }
        if (x + 1 < mapSize)
        {
            if (_tiles[x + 1, z].HasHole)
            {
                isHole = true;
            }
        }

        return isHole;
    }

    private bool IsHole(int x, int z)
    {
        if(x >= 0 && x < mapSize && z >= 0 && z < mapSize)
        {
            return _tiles[x, z].HasHole;
        }
        return false;
    }
}
