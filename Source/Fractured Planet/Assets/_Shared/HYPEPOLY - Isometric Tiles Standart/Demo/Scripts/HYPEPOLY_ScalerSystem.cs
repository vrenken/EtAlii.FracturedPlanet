// ReSharper disable All
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HYPEPOLY_ScalerSystem : MonoBehaviour
{
    public static HYPEPOLY_ScalerSystem Instance;
    [Range(0f,100f)]
    public float scalingGroupsSize = 50f;
    [Range(0f, 5f)]
    public float scalingTime = 0.2f;

    private List<Transform> scalers;
    private List<Vector3> targetScales;
    private int objectsScaled = 0;
    private bool startScaling = false;
    private float a = 0f;
    private int detailsInPair = 15;
    private int detailsForNoTile = 0;
    private int detailsNow = 0;
    private int scalingTilesCount = 0;

    [HideInInspector]
    public bool reversed = true;
    [HideInInspector]
    public bool mapReady = true;

    private bool reverseScaling = false;

    private void Awake()
    {
        Instance = this;
    }
    public void StartScaling(Transform _parent)
    {
        objectsScaled = 0;

        scalers = new List<Transform>();
        var tilesTransforms = new List<Transform>();
        var otherTransforms = new List<Transform>();
        for (var i = 0; i < _parent.childCount; i++)
        {
            if (_parent.GetChild(i).name.Contains("Tile"))
            {
                tilesTransforms.Add(_parent.GetChild(i));
            }
            else
            {
                otherTransforms.Add(_parent.GetChild(i));
            }
        }

        var animType = Random.Range(0, 2);

        if(animType == 0)
        {
            var subAnimType = Random.Range(0, 4);
            if(subAnimType == 0)
            {
                tilesTransforms = tilesTransforms.OrderBy(obj => obj.transform.position.x + obj.transform.position.z + obj.transform.position.y).ToList();
                otherTransforms = otherTransforms.OrderBy(obj => obj.transform.position.x + obj.transform.position.z + obj.transform.position.y).ToList();
            }
            else if (subAnimType == 1)
            {
                tilesTransforms = tilesTransforms.OrderBy(obj => 0-obj.transform.position.x - obj.transform.position.z - obj.transform.position.y).ToList();
                otherTransforms = otherTransforms.OrderBy(obj => 0-obj.transform.position.x - obj.transform.position.z - obj.transform.position.y).ToList();
            }
            else if (subAnimType == 2)
            {
                tilesTransforms = tilesTransforms.OrderBy(obj => obj.transform.position.x).ToList();
                otherTransforms = otherTransforms.OrderBy(obj => obj.transform.position.x).ToList();
            }
            else if (subAnimType == 3)
            {
                tilesTransforms = tilesTransforms.OrderBy(obj => obj.transform.position.y).ToList();
                otherTransforms = otherTransforms.OrderBy(obj => obj.transform.position.y).ToList();
            }
            detailsInPair = (int)((float)tilesTransforms.Count * (scalingGroupsSize / 100f));
            detailsForNoTile = (int)((float)otherTransforms.Count * (scalingGroupsSize / 100f));
            if (detailsForNoTile <= 0) detailsForNoTile = 1;
            detailsNow = detailsInPair;
            scalingTilesCount = tilesTransforms.Count;

            scalers.AddRange(tilesTransforms);
            scalers.AddRange(otherTransforms);
        }
        else if (animType == 1)
        {
            tilesTransforms = tilesTransforms.OrderBy(obj => Random.value).ToList();
            otherTransforms = otherTransforms.OrderBy(obj => Random.value).ToList();
            detailsInPair = (int)((float)tilesTransforms.Count * (scalingGroupsSize / 100f));
            detailsForNoTile = (int)((float)otherTransforms.Count * (scalingGroupsSize / 100f));
            if (detailsForNoTile <= 0) detailsForNoTile = 1;
            detailsNow = detailsInPair;
            scalingTilesCount = tilesTransforms.Count;

            scalers.AddRange(tilesTransforms);
            scalers.AddRange(otherTransforms);
        }

        targetScales = new List<Vector3>();
        for (var i = 0; i < scalers.Count; i++)
        {
            targetScales.Add(scalers[i].transform.localScale);
            scalers[i].transform.localScale = Vector3.zero;
        }

        a = 0f;
        startScaling = true;
        mapReady = false;
    }
    public void ReverseScaling()
    {
        objectsScaled = 0;

        detailsNow = detailsForNoTile*2;

        if (scalers != null)
            scalers.Reverse();

        reversed = false;
        reverseScaling = true;
    }

    private void FixedUpdate()
    {
        if (startScaling)
        {
            if (scalers[0] != null)
            {
                a += (1f / (scalingTime / Time.fixedDeltaTime));

                for (var i = 0; i < detailsNow; i++)
                {
                    if (objectsScaled + i < scalers.Count)
                        scalers[objectsScaled + i].transform.localScale = Vector3.Lerp(Vector3.zero, targetScales[objectsScaled + i], a);
                }

                if (a >= 1f)
                {
                    objectsScaled += detailsNow;
                    a = 0f;

                    if (objectsScaled >= scalers.Count)
                    {
                        mapReady = true;
                        startScaling = false;
                    }
                    if(objectsScaled > scalingTilesCount)
                    {
                        detailsNow = detailsForNoTile;
                    }
                }
            }
            else
            {
                mapReady = true;
                startScaling = false;
            }
        }
        else if(reverseScaling)
        {
            if (scalers != null && scalers[0] != null)
            {
                a += (1f / (scalingTime / Time.fixedDeltaTime))*2f;

                for (var i = 0; i < detailsNow; i++)
                {
                    if (objectsScaled + i < scalers.Count)
                        scalers[objectsScaled + i].transform.localScale = Vector3.Lerp(targetScales[objectsScaled + i], Vector3.zero, a);
                }

                if (a >= 1f)
                {
                    objectsScaled += detailsNow;
                    a = 0f;

                    if (objectsScaled >= scalers.Count)
                    {
                        reverseScaling = false;
                        reversed = true;
                    }
                    if (objectsScaled > scalers.Count - scalingTilesCount)
                    {
                        detailsNow = detailsInPair*2;
                    }
                }
            }
            else
            {
                reverseScaling = false;
                reversed = true;
            }
        }
    }
}
