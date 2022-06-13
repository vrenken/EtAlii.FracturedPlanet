using System;
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

    [Header("[Ladders on map]")]
    public EnableDisable ladders;
    [Range(0, 100)]
    public int laddersChance;
    public List<GameObject> laddersTiles;

    [Header("Tap this checkbox to generate in play mode")]
    public bool TapToGenerate;

    //Private
    private GameObject _lastMap;
    private bool[,] _lastRoadsMap, _lastLaddersMap;

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

        if (_lastMap != null)
        {
            Destroy(_lastMap);
        }

        yield return new WaitForEndOfFrame();

        GenerateMap(mapPosition);

        if (ScalerSystem.Instance != null)
        {
            var meshes = _lastMap.GetComponentsInChildren<MeshRenderer>();
            foreach (var mr in meshes) mr.enabled = false;
        }

        yield return new WaitForEndOfFrame();

        AdditionalFilling(mapSize);

        if(ScalerSystem.Instance != null)
        {
            ScalerSystem.Instance.StartScaling(_lastMap.transform);

            var meshes = _lastMap.GetComponentsInChildren<MeshRenderer>();
            foreach (var mr in meshes) mr.enabled = true;
        }

        for(var i = 0; i<_lastMap.transform.childCount; i++)
        {
            var thisPos = _lastMap.transform.GetChild(i).localPosition;
            thisPos.x -= ((mapSize * 2f) / 2f)-1f;
            thisPos.z -= ((mapSize * 2f) / 2f)-1f;
            _lastMap.transform.GetChild(i).localPosition = thisPos;
        }
    }
    public void GenerateMap(Vector3 mapPos)
    {
        var map = new GameObject
        {
            name = mapName,
            transform =
            {
                parent = transform,
                position = mapPos
            }
        };

        var xSize = mapSize;
        var zSize = mapSize;

        var roadsMap = new bool[xSize, zSize];
        var heightMap = new int[xSize, zSize];
        var holesMap = new bool[xSize, zSize];
        var laddersMap = new bool[xSize, zSize];

        //ROADS AND POI CREATING ----------------------------------------------
        var pointsOfInterest = new List<int[]>();

        var leftDownCorner = new Vector2(0, 0);
        var rightTopCorner = new Vector2(xSize, zSize);
        var maxMapDistance = Vector2.Distance(leftDownCorner, rightTopCorner);
        if (contentOnMap == EnableDisable.Enabled)
        {
            for (var i = 0; i < poiCount; i++)
            {
                var trys = 0;
                while (true)
                {
                    trys++;
                    var newPoi = new[] { Random.Range(0, xSize), Random.Range(0, zSize) };

                    if (!pointsOfInterest.Contains(newPoi))
                    {
                        var minDistance = -1f;

                        for (var a = 0; a < pointsOfInterest.Count; a++)
                        {
                            var firstPoint = new Vector2(pointsOfInterest[a][0], pointsOfInterest[a][1]);
                            var secondPoint = new Vector2(newPoi[0], newPoi[1]);
                            var distance = Vector2.Distance(firstPoint, secondPoint);
                            if (distance < minDistance || minDistance < 0f) minDistance = distance;
                        }

                        if (minDistance > (maxMapDistance / 4f) || minDistance < 0f || trys > xSize*zSize)
                        {
                            pointsOfInterest.Add(newPoi);
                            roadsMap[newPoi[0], newPoi[1]] = true;
                            break;
                        }
                    }
                }
            }

            if (roads == EnableDisable.Enabled)
            {
                for (var i = 0; i < pointsOfInterest.Count; i++)
                {
                    for (var a = i; a < pointsOfInterest.Count; a++)
                    {
                        if (i != a)
                        {
                            var createThisConnection = true;

                            if(i == 0 && a == 1)
                            {

                            }
                            else
                            {
                                createThisConnection = Random.Range(0, 100) < roadsBetweenPOI;
                            }

                            if (createThisConnection)
                            {
                                var minX = pointsOfInterest[i][0] < pointsOfInterest[a][0] ? pointsOfInterest[i][0] : pointsOfInterest[a][0];
                                var maxX = pointsOfInterest[i][0] < pointsOfInterest[a][0] ? pointsOfInterest[a][0] : pointsOfInterest[i][0];
                                var minZ = pointsOfInterest[i][1] < pointsOfInterest[a][1] ? pointsOfInterest[i][1] : pointsOfInterest[a][1];
                                var maxZ = pointsOfInterest[i][1] < pointsOfInterest[a][1] ? pointsOfInterest[a][1] : pointsOfInterest[i][1];

                                var down = Random.Range(0, 100) < 50;

                                bool left;
                                if (pointsOfInterest[i][1] > pointsOfInterest[a][1])
                                {
                                    if (pointsOfInterest[i][0] < pointsOfInterest[a][0])
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
                                    if (pointsOfInterest[a][0] < pointsOfInterest[i][0])
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
                                        roadsMap[p, minZ] = true;
                                    else
                                        roadsMap[p, maxZ] = true;
                                }
                                for (var p = minZ; p < maxZ; p++)
                                {
                                    if (left)
                                        roadsMap[minX, p] = true;
                                    else
                                        roadsMap[maxX, p] = true;
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

            if (holesCount == GeneratorParametr.Random) holesCount = (GeneratorParametr)Random.Range(1, 6);

            if (holesCount == GeneratorParametr.VeryLow) holesMultiplier = 0.1f;
            else if (holesCount == GeneratorParametr.Low) holesMultiplier = 0.2f;
            else if (holesCount == GeneratorParametr.Medium) holesMultiplier = 0.3f;
            else if (holesCount == GeneratorParametr.High) holesMultiplier = 0.4f;
            else if (holesCount == GeneratorParametr.VeryHigh) holesMultiplier = 0.5f;

            var holesSizesMultiplier = 0f;

            if (holesSizes == GeneratorParametr.Random || holesSizes == GeneratorParametr.None) holesSizes = (GeneratorParametr)Random.Range(1, 6);

            if (holesSizes == GeneratorParametr.VeryLow) holesSizesMultiplier = 0.1f;
            else if (holesSizes == GeneratorParametr.Low) holesSizesMultiplier = 0.25f;
            else if (holesSizes == GeneratorParametr.Medium) holesSizesMultiplier = 0.5f;
            else if (holesSizes == GeneratorParametr.High) holesSizesMultiplier = 0.85f;
            else if (holesSizes == GeneratorParametr.VeryHigh) holesSizesMultiplier = 1f;

            var minSide = zSize < xSize ? zSize : xSize;

            var holesCountToCreate = Mathf.FloorToInt(minSide * holesMultiplier);

            var maxHoleSize = Mathf.FloorToInt((minSide * 0.3f) * holesSizesMultiplier);

            for (var i = 0; i < holesCountToCreate; i++)
            {
                var hX = Random.Range(0, xSize);
                var hZ = Random.Range(0, zSize);
                holesMap = CreateHoles(holesMap, hX, hZ, maxHoleSize, 0);
            }
        }
        //-------------------------------------------------------------

        //HEIGHTS CREATING --------------------------------------------
        if (heightsCount != GeneratorParametr.None)
        {
            var heightsMultiplier = 0f;

            if (heightsCount == GeneratorParametr.Random) heightsCount = (GeneratorParametr)Random.Range(1, 6);

            if (heightsCount == GeneratorParametr.VeryLow) heightsMultiplier = 0.1f;
            else if (heightsCount == GeneratorParametr.Low) heightsMultiplier = 0.2f;
            else if (heightsCount == GeneratorParametr.Medium) heightsMultiplier = 0.3f;
            else if (heightsCount == GeneratorParametr.High) heightsMultiplier = 0.4f;
            else if (heightsCount == GeneratorParametr.VeryHigh) heightsMultiplier = 0.5f;

            var heightsSizesMultiplier = 0f;

            if (heightsSizes == GeneratorParametr.Random || heightsSizes == GeneratorParametr.None) heightsSizes = (GeneratorParametr)Random.Range(1, 6);

            if (heightsSizes == GeneratorParametr.VeryLow) heightsSizesMultiplier = 0.1f;
            else if (heightsSizes == GeneratorParametr.Low) heightsSizesMultiplier = 0.25f;
            else if (heightsSizes == GeneratorParametr.Medium) heightsSizesMultiplier = 0.5f;
            else if (heightsSizes == GeneratorParametr.High) heightsSizesMultiplier = 0.85f;
            else if (heightsSizes == GeneratorParametr.VeryHigh) heightsSizesMultiplier = 1f;

            var minSide = zSize < xSize ? zSize : xSize;

            var heightsCountToCreate = Mathf.FloorToInt(minSide * heightsMultiplier);

            var maxHeightSize = Mathf.FloorToInt((minSide * 0.4f) * heightsSizesMultiplier);

            var maxHeightInTiles = 0;

            if (maxHeight == GeneratorParametr.Random || maxHeight == GeneratorParametr.None) maxHeight = (GeneratorParametr)Random.Range(1, 6);

            if (maxHeight == GeneratorParametr.VeryLow) maxHeightInTiles = 1;
            else if (maxHeight == GeneratorParametr.Low) maxHeightInTiles = 2;
            else if (maxHeight == GeneratorParametr.Medium) maxHeightInTiles = 3;
            else if (maxHeight == GeneratorParametr.High) maxHeightInTiles = 4;
            else if (maxHeight == GeneratorParametr.VeryHigh) maxHeightInTiles = 5;

            for (var i = 0; i < heightsCountToCreate; i++)
            {
                var hX = Random.Range(0, xSize);
                var hZ = Random.Range(0, zSize);
                heightMap = RaiseHeight(heightMap, hX, hZ, maxHeightSize, maxHeightInTiles, holesMap, 0, heightSmoothing);
            }
        }

        //-------------------------------------------------------------

        //HEIGHT SMOOTHING----------------------------------------------
        if (heightSmoothing == EnableDisable.Enabled)
        {
            for (var i = 0; i < xSize; i++)
            {
                for (var a = 0; a < zSize; a++)
                {
                    SmoothHeights(heightMap, i, a);
                }
            }
        }
        //-------------------------------------------------------------

        //ROADS--------------------------------------------------------
        var roadsSumHeights = 0f;
        for(var i = 0; i<xSize; i++)
        {
            for(var a = 0; a<zSize; a++)
            {
                roadsSumHeights += heightMap[i, a];
            }
        }

        var roadsHeight = Mathf.CeilToInt(roadsSumHeights / (xSize * zSize));

        if (contentOnMap == EnableDisable.Enabled && roads == EnableDisable.Enabled)
        {
            for (var i = 0; i < pointsOfInterest.Count; i++)
            {
                var xPos = pointsOfInterest[i][1];
                var zPos = pointsOfInterest[i][0];

                holesMap[xPos, zPos] = false;

                if (xPos + 1 < xSize)
                {
                    heightMap[xPos + 1, zPos] = roadsHeight;
                    if (zPos + 1 < zSize)
                    {
                        heightMap[xPos + 1, zPos + 1] = roadsHeight;
                    }
                    if (zPos - 1 > 0)
                    {
                        heightMap[xPos + 1, zPos - 1] = roadsHeight;
                    }
                }
                if (zPos + 1 < zSize)
                {
                    heightMap[xPos, zPos + 1] = roadsHeight;
                }
                if (xPos - 1 > 0)
                {
                    heightMap[xPos - 1, zPos] = roadsHeight;
                    if (zPos + 1 < zSize)
                    {
                        heightMap[xPos - 1, zPos + 1] = roadsHeight;
                    }
                    if (zPos - 1 > 0)
                    {
                        heightMap[xPos - 1, zPos - 1] = roadsHeight;
                    }
                }
                if (zPos - 1 > 0)
                {
                    heightMap[xPos, zPos - 1] = roadsHeight;
                }
            }

            for (var i = 0; i < xSize; i++)
            {
                for (var a = 0; a < zSize; a++)
                {
                    if (roadsMap[i, a])
                    {
                        heightMap[a, i] = roadsHeight;
                        SmoothHeightDown(heightMap, a, i);
                    }
                }
            }
        }
        //-------------------------------------------------------------

        //LADDERS------------------------------------------------------
        if (ladders == EnableDisable.Enabled)
        {
            for (var i = 0; i < xSize; i++)
            {
                for (var a = 0; a < zSize; a++)
                {
                    if (!holesMap[i, a])
                    {
                        var myHeight = heightMap[a, i];

                        var right = false;
                        var left = false;
                        var up = false;
                        var down = false;

                        if (i + 1 < xSize)
                        {
                            if (!holesMap[i + 1, a] && !laddersMap[i + 1, a])
                            {
                                if (heightMap[a, i + 1] == (myHeight + 1)) right = true;
                            }
                        }
                        if (i - 1 >= 0)
                        {
                            if (!holesMap[i - 1, a] && !laddersMap[i - 1, a])
                            {
                                if (heightMap[a, i - 1] == (myHeight + 1)) left = true;
                            }
                        }
                        if (a + 1 < zSize)
                        {
                            if (!holesMap[i, a + 1] && !laddersMap[i, a + 1])
                            {
                                if (heightMap[a + 1, i] == (myHeight + 1)) up = true;
                            }
                        }
                        if (a - 1 >= 0)
                        {
                            if (!holesMap[i, a - 1] && !laddersMap[i, a - 1])
                            {
                                if (heightMap[a - 1, i] == (myHeight + 1)) down = true;
                            }
                        }

                        float y = 0;
                        var needSpawn = false;
                        if (right && !left && !down && !up) //Ladder to right
                        {
                            if (i - 1 >= 0)
                            {
                                if (heightMap[a, i - 1] == myHeight)
                                {
                                    if (!IsHole(i - 1, a, holesMap) && !IsHole(i + 1, a, holesMap) && !IsHole(i, a, holesMap))
                                    {
                                        y = 0;
                                        needSpawn = true;
                                    }
                                }
                            }
                        }
                        if (!right && left && !down && !up) //Ladder to left
                        {
                            if (i + 1 < xSize)
                            {
                                if (heightMap[a, i + 1] == myHeight)
                                {
                                    if (!IsHole(i - 1, a, holesMap) && !IsHole(i + 1, a, holesMap) && !IsHole(i, a, holesMap))
                                    {
                                        y = 180f;
                                        needSpawn = true;
                                    }
                                }
                            }
                        }
                        if (!right && !left && down && !up) //Ladder to down
                        {
                            if (a + 1 < zSize)
                            {
                                if (heightMap[a + 1, i] == myHeight)
                                {
                                    if (!IsHole(i, a - 1, holesMap) && !IsHole(i, a + 1, holesMap) && !IsHole(i, a, holesMap))
                                    {
                                        y = 90f;
                                        needSpawn = true;
                                    }
                                }
                            }
                        }
                        if (!right && !left && !down && up) //Ladder to up
                        {
                            if (a - 1 >= 0)
                            {
                                if (heightMap[a, a - 1] == myHeight)
                                {
                                    if (!IsHole(i, a - 1, holesMap) && !IsHole(i, a + 1, holesMap) && !IsHole(i, a, holesMap))
                                    {
                                        y = -90f;
                                        needSpawn = true;
                                    }
                                }
                            }
                        }

                        if (needSpawn)
                        {
                            if (Random.Range(0, 100) < laddersChance)
                            {
                                laddersMap[i, a] = true;
                                var ladder = Instantiate(laddersTiles[Random.Range(0, laddersTiles.Count)], map.transform, true);
                                ladder.transform.position = new Vector3(i * 2f, mapPos.y + (heightMap[a, i] * 2f), a * 2f);
                                ladder.transform.localEulerAngles = new Vector3(0f, y, 0f);
                            }
                        }
                    }
                }
            }
        }
        //-------------------------------------------------------------

        //SPAWNING MAP-------------------------------------------------
        var x = 0f;
        var z = 0f;

        for (var i = 0; i < xSize; i++)
        {
            for (var a = 0; a < zSize; a++)
            {
                if (!holesMap[i, a])
                {
                    SpawnTile(i, a, x, z, mapPosition, map.transform, roadsMap[a, i], heightMap[i, a]);
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
                if (i == 0) poiObj = Instantiate(startTile);
                else if (i == 1) poiObj = Instantiate(endTile);
                else poiObj = Instantiate(interestPointTiles[Random.Range(0, interestPointTiles.Count)]);
                poiObj.transform.position = new Vector3(zPos * 2f, mapPos.y + (heightMap[xPos, zPos] * 2f), xPos * 2f);
                poiObj.transform.parent = map.transform;

                xPos = pointsOfInterest[i][0];
                zPos = pointsOfInterest[i][1];

                if (xPos + 1 < xSize)
                {
                    if (roadsMap[xPos + 1, zPos])
                    {
                        poiObj.transform.localEulerAngles = new Vector3(0f, -90f, 0f);
                    }
                }
                if (zPos + 1 < zSize)
                {
                    if (roadsMap[xPos, zPos + 1])
                    {
                        poiObj.transform.localEulerAngles = new Vector3(0f, -180f, 0f);
                    }
                }
                if (xPos - 1 > 0)
                {
                    if (roadsMap[xPos - 1, zPos])
                    {
                        poiObj.transform.localEulerAngles = new Vector3(0f, -270f, 0f);
                    }
                }
                if (zPos - 1 > 0)
                {
                    if (roadsMap[xPos, zPos - 1])
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
            if (roadBridges.Count > 0) bridgeNumber = Random.Range(0, roadBridges.Count);
            for (var i = 0; i < xSize; i++)
            {
                for (var a = 0; a < zSize; a++)
                {
                    if (roadsMap[i, a])
                    {
                        var xPos = i;
                        var zPos = a;

                        var right = false;
                        var left = false;
                        var up = false;
                        var down = false;

                        if (xPos + 1 < xSize) //RightTile checking
                        {
                            if (roadsMap[xPos + 1, zPos] && !holesMap[a, i + 1]) //RightTile road +
                            {
                                right = true;
                            }
                        }
                        if (xPos - 1 >= 0)
                        {
                            if (roadsMap[xPos - 1, zPos] && !holesMap[a, i - 1])
                            {
                                left = true;
                            }
                        }
                        if (zPos + 1 < zSize)
                        {
                            if (roadsMap[xPos, zPos + 1] && !holesMap[a + 1, i])
                            {
                                up = true;
                            }
                        }
                        if (zPos - 1 >= 0)
                        {
                            if (roadsMap[xPos, zPos - 1] && !holesMap[a - 1, i])
                            {
                                down = true;
                            }
                        }

                        if (up && down && !IsHole(i, a, holesMap))
                        {
                            if (Random.Range(0, 100) < (100 - roadsFilling))
                                roadsMap[i, a] = false;
                        }
                        else if (left && right && !IsHole(i,a,holesMap))
                        {
                            if (Random.Range(0, 100) < (100 - roadsFilling))
                                roadsMap[i, a] = false;
                        }
                    }
                }
            }

            for (var i = 0; i < xSize; i++)
            {
                for (var a = 0; a < zSize; a++)
                {
                    if (roadsMap[i, a] && RoadIsNotPOI(i, a, pointsOfInterest))
                    {
                        var xPos = i;
                        var zPos = a;
                        var yEulers = 0f;
                        var spawned = true;

                        GameObject road = null;

                        var right = false;
                        var left = false;
                        var up = false;
                        var down = false;

                        if (xPos + 1 < xSize) //RightTile checking
                        {
                            if (roadsMap[xPos + 1, zPos]) //RightTile road +
                            {
                                right = true;
                            }
                        }
                        if (xPos - 1 >= 0)
                        {
                            if (roadsMap[xPos - 1, zPos])
                            {
                                left = true;
                            }
                        }
                        if (zPos + 1 < zSize)
                        {
                            if (roadsMap[xPos, zPos + 1])
                            {
                                up = true;
                            }
                        }
                        if (zPos - 1 >= 0)
                        {
                            if (roadsMap[xPos, zPos - 1])
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
                            if (holesMap[a, i])
                            {
                                road = Instantiate(roadBridges[bridgeNumber]);
                                yEulers = 90f;
                            }
                            else
                            {
                                if (RoadIsNotPOI(i + 1, a, pointsOfInterest) && RoadIsNotPOI(i - 1, a, pointsOfInterest))
                                {
                                    road = Instantiate(roadStraight);
                                    if (Random.Range(0, 100) < roadsFenceChance)
                                        road.transform.GetChild(1).gameObject.SetActive(true);
                                    if (Random.Range(0, 100) < roadsFenceChance)
                                        road.transform.GetChild(2).gameObject.SetActive(true);

                                    yEulers = 90f;
                                }
                                else
                                {
                                    if (holesMap[a, i])
                                        road = Instantiate(roadBridges[bridgeNumber]);
                                    else
                                        road = Instantiate(roadEnd);

                                    if (RoadIsNotPOI(i + 1, a, pointsOfInterest)) yEulers = -90f;
                                    else yEulers = 90f;
                                }
                            }
                        }
                        else if (up && down)
                        {
                            if (holesMap[a, i])
                            {
                                road = Instantiate(roadBridges[bridgeNumber]);
                                yEulers = 0f;
                            }
                            else
                            {
                                if (RoadIsNotPOI(i + 1, a, pointsOfInterest) && RoadIsNotPOI(i - 1, a, pointsOfInterest))
                                {
                                    road = Instantiate(roadStraight);
                                    if (Random.Range(0, 100) < roadsFenceChance)
                                        road.transform.GetChild(1).gameObject.SetActive(true);
                                    if (Random.Range(0, 100) < roadsFenceChance)
                                        road.transform.GetChild(2).gameObject.SetActive(true);

                                    yEulers = 0f;
                                }
                                else
                                {
                                    if (holesMap[a, i])
                                        road = Instantiate(roadBridges[bridgeNumber]);
                                    else
                                        road = Instantiate(roadEnd);

                                    if (RoadIsNotPOI(i + 1, a, pointsOfInterest)) yEulers = -90f;
                                    else yEulers = 90f;
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
                            if (holesMap[a, i])
                                road = Instantiate(roadBridges[bridgeNumber]);
                            else
                                road = Instantiate(roadEnd);
                            yEulers = 180f;
                        }
                        else if (down)
                        {
                            if (holesMap[a, i])
                                road = Instantiate(roadBridges[bridgeNumber]);
                            else
                                road = Instantiate(roadEnd);
                            yEulers = 0f;
                        }
                        else if (right)
                        {
                            if (holesMap[a, i])
                                road = Instantiate(roadBridges[bridgeNumber]);
                            else
                                road = Instantiate(roadEnd);
                            yEulers = -90f;
                        }
                        else if (left)
                        {
                            if (holesMap[a, i])
                                road = Instantiate(roadBridges[bridgeNumber]);
                            else
                                road = Instantiate(roadEnd);
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
                            if (holesMap[a, i])
                            {
                                SpawnTile(i, a, (i * 2), (a * 2), mapPosition, map.transform, true, heightMap[a, i]);
                            }
                        }

                        if (spawned)
                        {
                            road.transform.position = new Vector3(i * 2f, mapPos.y + (heightMap[a, i] * 2f), a * 2f);
                            road.transform.parent = map.transform;
                            road.transform.localEulerAngles = new Vector3(0f, yEulers, 0f);
                        }
                    }
                }
            }
        }
        //-------------------------------------------------------------

        _lastRoadsMap = roadsMap;
        _lastLaddersMap = laddersMap;
        _lastMap = map;
    }

    private void AdditionalFilling(int sizeOfMap)
    {

        if (additionalFilling == GeneratorParametr.Random) additionalFilling = (GeneratorParametr)Random.Range(1, 6);
        if (additionalFilling != GeneratorParametr.None)
        {
            var countsCycle = (int)(sizeOfMap/5f) * (int)additionalFilling;

            var circlesRange = sizeOfMap/6f + sizeOfMap / 30f * (int)additionalFilling;

            var objectsCounts = sizeOfMap/2.5f + sizeOfMap/6 * (int)additionalFilling;

            Debug.Log(countsCycle + " " + circlesRange + " " + objectsCounts);

            var treesPoints = new List<Vector3>();
            var bushsPoints = new List<Vector3>();
            var bigStonesPoints = new List<Vector3>();
            var grassPoint = new List<Vector3>();
            var branchsPoints = new List<Vector3>();
            var logsPoints = new List<Vector3>();

            for (var a = 0; a < countsCycle; a++)
            {
                var circleTreesPos = new Vector3(Random.Range(0f, sizeOfMap * 2f), 15f, Random.Range(0f, sizeOfMap * 2f));

                for (var i = 0; i < objectsCounts; i++) //Trees
                {
                    RaycastHit hit;
                    var rayPos = circleTreesPos;
                    rayPos.x += Random.Range(-circlesRange, circlesRange);
                    rayPos.z += Random.Range(-circlesRange, circlesRange);
                    if (Physics.Raycast(rayPos, Vector3.down, out hit, Mathf.Infinity))
                    {
                        if (hit.transform.name.Contains("Tile") && IsPosAvailableByDistance(hit.point,treesPoints,1.5f) && isPosNotInPOI(hit.point,_lastRoadsMap,_lastLaddersMap))
                        {
                            var tree = Instantiate(treesPrefabs[Random.Range(0, treesPrefabs.Count)], hit.point, Quaternion.identity, _lastMap.transform);
                            tree.transform.eulerAngles = new Vector3(Random.Range(-7.5f, 7.5f), Random.Range(0f, 360f), Random.Range(-7.5f, 7.5f));
                            treesPoints.Add(hit.point);
                        }
                    }
                }

                for (var i = 0; i < objectsCounts/3; i++) //Bushs
                {
                    RaycastHit hit;
                    var rayPos = circleTreesPos;
                    rayPos.x += Random.Range(-circlesRange, circlesRange);
                    rayPos.z += Random.Range(-circlesRange, circlesRange);
                    if (Physics.Raycast(rayPos, Vector3.down, out hit, Mathf.Infinity))
                    {
                        if (hit.transform.name.Contains("Tile") && IsPosAvailableByDistance(hit.point, bushsPoints, 2f) && isPosInRangeOf(hit.point,treesPoints,4f) && isPosNotInPOI(hit.point, _lastRoadsMap, _lastLaddersMap))
                        {
                            var tree = Instantiate(bushsPrefabs[Random.Range(0, bushsPrefabs.Count)], hit.point, Quaternion.identity, _lastMap.transform);
                            tree.transform.eulerAngles = new Vector3(0f, Random.Range(0f, 360f), 0f);
                            bushsPoints.Add(hit.point);
                        }
                    }
                }

                for (var i = 0; i < objectsCounts * 4; i++) //Grass
                {
                    RaycastHit hit;
                    var rayPos = circleTreesPos;
                    rayPos.x += Random.Range(-circlesRange, circlesRange);
                    rayPos.z += Random.Range(-circlesRange, circlesRange);
                    if (Physics.Raycast(rayPos, Vector3.down, out hit, Mathf.Infinity))
                    {
                        if (hit.transform.name.Contains("Tile") && IsPosAvailableByDistance(hit.point,grassPoint, 0.25f) && isPosNotInPOI(hit.point, _lastRoadsMap, _lastLaddersMap))
                        {
                            var tree = Instantiate(grassPrefabs[Random.Range(0, grassPrefabs.Count)], hit.point, Quaternion.identity, _lastMap.transform);
                            tree.transform.eulerAngles = new Vector3(0f, Random.Range(0f, 360f), 0f);
                            grassPoint.Add(hit.point);
                        }
                    }
                }

                for (var i = 0; i < objectsCounts; i++) //Little stones
                {
                    RaycastHit hit;
                    var rayPos = circleTreesPos;
                    rayPos.x += Random.Range(-circlesRange, circlesRange);
                    rayPos.z += Random.Range(-circlesRange, circlesRange);
                    if (Physics.Raycast(rayPos, Vector3.down, out hit, Mathf.Infinity))
                    {
                        if (hit.transform.name.Contains("Tile") && isPosNotInPOI(hit.point, _lastRoadsMap, _lastLaddersMap))
                        {
                            var tree = Instantiate(littleStonesPrefabs[Random.Range(0, littleStonesPrefabs.Count)], hit.point, Quaternion.identity, _lastMap.transform);
                            tree.transform.eulerAngles = new Vector3(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
                        }
                    }
                }
            }


            for (var i = 0; i < ((objectsCounts * countsCycle)*(int)additionalFilling); i++) //Grass
            {
                RaycastHit hit;
                var rayPos = new Vector3(Random.Range(0f, sizeOfMap * 2f), 15f, Random.Range(0f, sizeOfMap * 2f));
                if (Physics.Raycast(rayPos, Vector3.down, out hit, Mathf.Infinity))
                {
                    if (hit.transform.name.Contains("Tile") && IsPosAvailableByDistance(hit.point, grassPoint, 0.25f) && isPosNotInPOI(hit.point, _lastRoadsMap, _lastLaddersMap))
                    {
                        var tree = Instantiate(grassPrefabs[Random.Range(0, grassPrefabs.Count)], hit.point, Quaternion.identity, _lastMap.transform);
                        tree.transform.eulerAngles = new Vector3(0f, Random.Range(0f, 360f), 0f);
                        grassPoint.Add(hit.point);
                    }
                }
            }

            for (var i = 0; i < objectsCounts / 2; i++) //big stones
            {
                RaycastHit hit;
                var rayPos = new Vector3(Random.Range(0f, sizeOfMap * 2f), 15f, Random.Range(0f, sizeOfMap * 2f));
                if (Physics.Raycast(rayPos, Vector3.down, out hit, Mathf.Infinity))
                {
                    if (hit.transform.name.Contains("Tile") && IsPosAvailableByDistance(hit.point, bigStonesPoints, 10f) && isPosInRangeOf(hit.point, treesPoints, 8f) && isPosNotInPOI(hit.point, _lastRoadsMap, _lastLaddersMap))
                    {
                        var tree = Instantiate(bigStonesPrefabs[Random.Range(0, bigStonesPrefabs.Count)], hit.point, Quaternion.identity, _lastMap.transform);
                        tree.transform.eulerAngles = new Vector3(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
                        bigStonesPoints.Add(hit.point);
                    }
                }
            }

            for (var i = 0; i < objectsCounts / 2; i++) //branchs
            {
                RaycastHit hit;
                var rayPos = new Vector3(Random.Range(0f, sizeOfMap * 2f), 15f, Random.Range(0f, sizeOfMap * 2f));
                if (Physics.Raycast(rayPos, Vector3.down, out hit, Mathf.Infinity))
                {
                    if (hit.transform.name.Contains("Tile") && IsPosAvailableByDistance(hit.point, bigStonesPoints, 10f) && isPosInRangeOf(hit.point, treesPoints, 8f) && isPosNotInPOI(hit.point, _lastRoadsMap, _lastLaddersMap))
                    {
                        var tree = Instantiate(branchsPrefabs[Random.Range(0, branchsPrefabs.Count)], hit.point, Quaternion.identity, _lastMap.transform);
                        tree.transform.eulerAngles = new Vector3(0f, Random.Range(0f, 360f), 0f);
                        branchsPoints.Add(hit.point);
                    }
                }
            }

            for (var i = 0; i < objectsCounts / 2; i++) //logs
            {
                RaycastHit hit;
                var rayPos = new Vector3(Random.Range(0f, sizeOfMap * 2f), 15f, Random.Range(0f, sizeOfMap * 2f));
                if (Physics.Raycast(rayPos, Vector3.down, out hit, Mathf.Infinity))
                {
                    if (hit.transform.name.Contains("Tile") && IsPosAvailableByDistance(hit.point, bigStonesPoints, 10f) && isPosInRangeOf(hit.point, treesPoints, 8f) && isPosNotInPOI(hit.point, _lastRoadsMap, _lastLaddersMap))
                    {
                        var tree = Instantiate(logsPrefabs[Random.Range(0, logsPrefabs.Count)], hit.point, Quaternion.identity, _lastMap.transform);
                        tree.transform.eulerAngles = new Vector3(0f, Random.Range(0f, 360f),0f);
                        logsPoints.Add(hit.point);
                    }
                }
            }
        }
    }

    private bool isPosNotInPOI(Vector3 posToCheck, bool[,] roadsMap, bool[,] laddersMap)
    {
        var x = Mathf.CeilToInt(posToCheck.x / 2f);
        var z = Mathf.CeilToInt(posToCheck.z / 2f);

        if (x < roadsMap.GetLength(0) && z < roadsMap.GetLength(1))
        {
            try
            {
                return !roadsMap[z, x] && !laddersMap[x, z];
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return false;
            }
        }

        return false;
    }

    private bool IsPosAvailableByDistance(Vector3 posToCheck, List<Vector3> otherPoses, float minDistance)
    {
        var minDistanceWeHas = -1f;

        foreach (var otherPose in otherPoses)
        {
            var distance = Vector3.Distance(posToCheck, otherPose);
            if (distance < minDistanceWeHas || minDistanceWeHas < 0f) minDistanceWeHas = distance;
        }

        return minDistanceWeHas > minDistance || minDistanceWeHas < 0f;
    }

    private bool isPosInRangeOf(Vector3 posToCheck, List<Vector3> otherPoses, float needDistance)
    {
        var minDistanceWeHas = -1f;

        foreach (var otherPose in otherPoses)
        {
            var distance = Vector3.Distance(posToCheck, otherPose);
            if (distance < minDistanceWeHas || minDistanceWeHas < 0f) minDistanceWeHas = distance;
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

    private void SpawnTile(int i, int a, float x, float z, Vector3 mapPos, Transform parent, bool isRoad, int height)
    {
        var newPart = Instantiate(tiles[Random.Range(0,tiles.Count)], parent, true);
        newPart.name = string.Concat(newPart.name.TakeWhile(c => c != '(')) + $" ({i:+00;-00} x {a:+00;-00})";
        var posY = mapPos.y;

        var heightForThisTile = height;

        if (heightForThisTile > 0)
        {
            posY += (heightForThisTile * 2f);
        }
        newPart.transform.position = new Vector3(mapPos.x + x, posY, mapPos.z + z);

        if (heightForThisTile > 0)
        {
            newPart.transform.GetChild(1).localScale = new Vector3(1f, 1f + (heightForThisTile * 1.2f), 1f);
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

    private int[,] SmoothHeights(int[,] currentMap, int x, int z)
    {
        var myHeight = currentMap[x, z];
        var heightDifference = 0;

        if (z - 1 > 0)
        {
            var difference = currentMap[x, z - 1] - myHeight;
            if (difference > heightDifference) heightDifference = difference;
        }
        if (z + 1 < currentMap.GetLength(1))
        {
            var difference = currentMap[x, z + 1] - myHeight;
            if (difference > heightDifference) heightDifference = difference;
        }
        if (x - 1 > 0)
        {
            var difference = currentMap[x - 1, z] - myHeight;
            if (difference > heightDifference) heightDifference = difference;
        }
        if (x + 1 < currentMap.GetLength(0))
        {
            var difference = currentMap[x + 1, z] - myHeight;
            if (difference > heightDifference) heightDifference = difference;
        }

        if (heightDifference > 1)
        {
            currentMap[x, z] = myHeight + 1;
        }

        return currentMap;
    }

    private int[,] SmoothHeightDown(int[,] currentMap, int x, int z)
    {
        var myHeight = currentMap[x, z];

        if (z - 1 > 0)
        {
            if (currentMap[x, z - 1] > myHeight)
            {
                currentMap[x, z - 1] = Random.Range(myHeight, myHeight + 2);
                currentMap = SmoothHeightDown(currentMap, x, z - 1);
            }
        }
        if (z + 1 < currentMap.GetLength(1))
        {
            if (currentMap[x, z + 1] > myHeight)
            {
                currentMap[x, z + 1] = Random.Range(myHeight, myHeight + 2);
                currentMap = SmoothHeightDown(currentMap, x, z + 1);
            }
        }
        if (x - 1 > 0)
        {
            if (currentMap[x-1, z] > myHeight)
            {
                currentMap[x-1, z] = Random.Range(myHeight, myHeight + 2);
                currentMap = SmoothHeightDown(currentMap, x - 1, z);
            }
        }
        if (x + 1 < currentMap.GetLength(0))
        {
            if (currentMap[x + 1, z] > myHeight)
            {
                currentMap[x + 1, z] = Random.Range(myHeight, myHeight + 2);
                currentMap = SmoothHeightDown(currentMap, x + 1, z);
            }
        }

        return currentMap;
    }

    private int[,] RaiseHeight(int[,] currentMap, int x, int z, int maxSize, int maxHeight, bool[,] holesMap, int iteration, EnableDisable hs)
    {
        if ((hs == EnableDisable.Enabled && !IsNeighboringHole(x, z, holesMap)) || hs == EnableDisable.Disabled)
        {
            if (currentMap[x, z] < maxHeight)
            {
                currentMap[x, z] += 1;
            }

            var chanceForAdditionalHeight = 100f;

            var anywayHeights = Mathf.FloorToInt(maxSize / 1.35f);

            if (iteration > anywayHeights)
            {
                chanceForAdditionalHeight = 100f - ((100f / (maxSize - anywayHeights)) * (iteration - anywayHeights));
            }

            iteration++;

            if (z - 1 > 0 && Random.Range(0, 100) < (chanceForAdditionalHeight * Random.Range(0.75f,1f)))
            {
                currentMap = RaiseHeight(currentMap, x, z - 1, maxSize, maxHeight, holesMap, iteration, hs);
            }
            if (z + 1 < currentMap.GetLength(1) && Random.Range(0, 100) < (chanceForAdditionalHeight * Random.Range(0.75f, 1f)))
            {
                currentMap = RaiseHeight(currentMap, x, z + 1, maxSize, maxHeight, holesMap, iteration, hs);
            }
            if (x - 1 > 0 && Random.Range(0, 100) < (chanceForAdditionalHeight * Random.Range(0.75f, 1f)))
            {
                currentMap = RaiseHeight(currentMap, x - 1, z, maxSize, maxHeight, holesMap, iteration, hs);
            }
            if (x + 1 < currentMap.GetLength(0) && Random.Range(0, 100) < (chanceForAdditionalHeight * Random.Range(0.75f, 1f)))
            {
                currentMap = RaiseHeight(currentMap, x + 1, z, maxSize, maxHeight, holesMap, iteration, hs);
            }
        }
        return currentMap;
    }

    private bool[,] CreateHoles(bool[,] currentMap, int x, int z, int maxSize, int iteration)
    {
        if (!currentMap[x, z])
        {
            currentMap[x, z] = true;

            var chanceForAdditionalHole = 100f;

            var anywayHoles = Mathf.FloorToInt(maxSize / 1.25f);

            if (iteration > anywayHoles)
            {
                chanceForAdditionalHole = 100f - ((100f / (maxSize - anywayHoles)) * (iteration - anywayHoles));
            }

            iteration++;

            if (iteration < maxSize)
            {
                if (z - 1 > 0 && Random.Range(0, 100) < (chanceForAdditionalHole * Random.Range(0.75f, 1f)))
                {
                    currentMap = CreateHoles(currentMap, x, z - 1, maxSize, iteration);
                }
                if (z + 1 < currentMap.GetLength(1) && Random.Range(0, 100) < (chanceForAdditionalHole * Random.Range(0.75f, 1f)))
                {
                    currentMap = CreateHoles(currentMap, x, z + 1, maxSize, iteration);
                }
                if (x - 1 > 0 && Random.Range(0, 100) < (chanceForAdditionalHole * Random.Range(0.75f, 1f)))
                {
                    currentMap = CreateHoles(currentMap, x - 1, z, maxSize, iteration);
                }
                if (x + 1 < currentMap.GetLength(0) && Random.Range(0, 100) < (chanceForAdditionalHole * Random.Range(0.75f, 1f)))
                {
                    currentMap = CreateHoles(currentMap, x + 1, z, maxSize, iteration);
                }
            }
        }

        return currentMap;
    }

    private bool IsNeighboringHigher(int[,] currentMap, int x, int z)
    {
        var isHigher = false;
        var myHeight = currentMap[x, z];
        if (z - 1 > 0)
        {
            if (currentMap[x, z - 1] > myHeight) isHigher = true;
        }
        if (z + 1 < currentMap.GetLength(1))
        {
            if (currentMap[x, z + 1] > myHeight) isHigher = true;
        }
        if (x - 1 > 0)
        {
            if (currentMap[x - 1, z] > myHeight) isHigher = true;
        }
        if (x + 1 < currentMap.GetLength(0))
        {
            if (currentMap[x + 1, z] > myHeight) isHigher = true;
        }
        return isHigher;
    }

    private bool IsNeighboringHole(int x, int z, bool[,] currentHoles)
    {
        var isHole = false;

        if (z - 1 > 0)
        {
            if (currentHoles[x,z-1]) isHole = true;
        }
        if (z + 1 < currentHoles.GetLength(1))
        {
            if (currentHoles[x, z + 1]) isHole = true;
        }
        if (x - 1 > 0)
        {
            if (currentHoles[x - 1, z]) isHole = true;
        }
        if (x + 1 < currentHoles.GetLength(0))
        {
            if (currentHoles[x + 1, z]) isHole = true;
        }

        return isHole;
    }

    private bool IsHole(int x, int z, bool[,] holes)
    {
        if(z >= 0 && z < holes.GetLength(0) && x >= 0 && x < holes.GetLength(1))
        {
            return holes[z, x];
        }
        return false;
    }
}
