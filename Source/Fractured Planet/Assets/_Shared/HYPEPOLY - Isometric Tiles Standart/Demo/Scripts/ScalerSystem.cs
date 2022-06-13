using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// ReSharper disable once CheckNamespace
public class ScalerSystem : MonoBehaviour
{
    public static ScalerSystem Instance;
    [Range(0f,100f)]
    public float scalingGroupsSize = 50f;
    [Range(0f, 5f)]
    public float scalingTime = 0.2f;

    private List<Transform> _scalers;
    private List<Vector3> _targetScales;
    private int _objectsScaled = 0;
    private bool _startScaling = false;
    private float _a = 0f;
    private int _detailsInPair = 15;
    private int _detailsForNoTile = 0;
    private int _detailsNow = 0;
    private int _scalingTilesCount = 0;
    private bool _reverseScaling = false;

    [HideInInspector]
    public bool reversed = true;
    [HideInInspector]
    public bool mapReady = true;

    private void Awake()
    {
        Instance = this;
    }
    public void StartScaling(Transform parent)
    {
        _objectsScaled = 0;

        _scalers = new List<Transform>();
        var tilesTransforms = new List<Transform>();
        var otherTransforms = new List<Transform>();
        for (var i = 0; i < parent.childCount; i++)
        {
            if (parent.GetChild(i).name.Contains("Tile"))
            {
                tilesTransforms.Add(parent.GetChild(i));
            }
            else
            {
                otherTransforms.Add(parent.GetChild(i));
            }
        }

        var animType = Random.Range(0, 2);

        if(animType == 0)
        {
            var subAnimType = Random.Range(0, 4);
            if(subAnimType == 0)
            {
                tilesTransforms = tilesTransforms.OrderBy(obj => { var objPosition = obj.transform.position; return objPosition.x + objPosition.z + objPosition.y; }).ToList();
                otherTransforms = otherTransforms.OrderBy(obj => { var objPosition = obj.transform.position; return objPosition.x + objPosition.z + objPosition.y; }).ToList();
            }
            else if (subAnimType == 1)
            {
                tilesTransforms = tilesTransforms.OrderBy(obj => { var objPosition = obj.transform.position; return 0 - objPosition.x - objPosition.z - objPosition.y; }).ToList();
                otherTransforms = otherTransforms.OrderBy(obj => { var objPosition = obj.transform.position; return 0 - objPosition.x - objPosition.z - objPosition.y; }).ToList();
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
            _detailsInPair = (int)(tilesTransforms.Count * (scalingGroupsSize / 100f));
            _detailsForNoTile = (int)(otherTransforms.Count * (scalingGroupsSize / 100f));
            if (_detailsForNoTile <= 0) _detailsForNoTile = 1;
            _detailsNow = _detailsInPair;
            _scalingTilesCount = tilesTransforms.Count;

            _scalers.AddRange(tilesTransforms);
            _scalers.AddRange(otherTransforms);
        }
        else if (animType == 1)
        {
            tilesTransforms = tilesTransforms.OrderBy(_ => Random.value).ToList();
            otherTransforms = otherTransforms.OrderBy(_ => Random.value).ToList();
            _detailsInPair = (int)(tilesTransforms.Count * (scalingGroupsSize / 100f));
            _detailsForNoTile = (int)(otherTransforms.Count * (scalingGroupsSize / 100f));
            if (_detailsForNoTile <= 0) _detailsForNoTile = 1;
            _detailsNow = _detailsInPair;
            _scalingTilesCount = tilesTransforms.Count;

            _scalers.AddRange(tilesTransforms);
            _scalers.AddRange(otherTransforms);
        }

        _targetScales = new List<Vector3>();
        for (var i = 0; i < _scalers.Count; i++)
        {
            _targetScales.Add(_scalers[i].transform.localScale);
            _scalers[i].transform.localScale = Vector3.zero;
        }

        _a = 0f;
        _startScaling = true;
        mapReady = false;
    }
    public void ReverseScaling()
    {
        _objectsScaled = 0;

        _detailsNow = _detailsForNoTile*2;

        if (_scalers != null)
            _scalers.Reverse();

        reversed = false;
        _reverseScaling = true;
    }

    private void FixedUpdate()
    {
        if (_startScaling)
        {
            if (_scalers[0] != null)
            {
                _a += (1f / (scalingTime / Time.fixedDeltaTime));

                for (var i = 0; i < _detailsNow; i++)
                {
                    if (_objectsScaled + i < _scalers.Count)
                    {
                        _scalers[_objectsScaled + i].transform.localScale = Vector3.Lerp(Vector3.zero, _targetScales[_objectsScaled + i], _a);
                    }
                }

                if (_a >= 1f)
                {
                    _objectsScaled += _detailsNow;
                    _a = 0f;

                    if (_objectsScaled >= _scalers.Count)
                    {
                        mapReady = true;
                        _startScaling = false;
                    }
                    if(_objectsScaled > _scalingTilesCount)
                    {
                        _detailsNow = _detailsForNoTile;
                    }
                }
            }
            else
            {
                mapReady = true;
                _startScaling = false;
            }
        }
        else if(_reverseScaling)
        {
            if (_scalers != null && _scalers[0] != null)
            {
                _a += (1f / (scalingTime / Time.fixedDeltaTime))*2f;

                for (var i = 0; i < _detailsNow; i++)
                {
                    if (_objectsScaled + i < _scalers.Count)
                    {
                        _scalers[_objectsScaled + i].transform.localScale = Vector3.Lerp(_targetScales[_objectsScaled + i], Vector3.zero, _a);
                    }
                }

                if (_a >= 1f)
                {
                    _objectsScaled += _detailsNow;
                    _a = 0f;

                    if (_objectsScaled >= _scalers.Count)
                    {
                        _reverseScaling = false;
                        reversed = true;
                    }
                    if (_objectsScaled > _scalers.Count - _scalingTilesCount)
                    {
                        _detailsNow = _detailsInPair*2;
                    }
                }
            }
            else
            {
                _reverseScaling = false;
                reversed = true;
            }
        }
    }
}
