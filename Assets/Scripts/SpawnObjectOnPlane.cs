using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class SpawnObjectOnPlane : MonoBehaviour
{
    private ARRaycastManager _raycastManager;
    private GameObject _spawnedObject;
    private List<GameObject> placedPreafabList = new List<GameObject>();
    private Vector3 _spawnPosition;
    private bool _selectableObjectHit;


    [SerializeField] private Camera _ARCamera;
    [SerializeField] private int _maxPrefabSpawnCount = 100;
    private int _placedPrefabCount;

    [SerializeField] private GameObject placeablePrefab;

    private static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    private void Awake()
    {
        _raycastManager = GetComponent<ARRaycastManager>();
    }

    bool TryGetTouchPosition(out Vector2 touchPosition)
    {
        if (Input.GetTouch(0).phase == TouchPhase.Began)
        {
            touchPosition = Input.GetTouch(0).position;
            return true;
        }

        touchPosition = default;
        return false;
    }

    private void Update()
    {
        if (!TryGetTouchPosition(out Vector2 touchPosition))
        {
            return;
        }

        if (_raycastManager.Raycast(touchPosition,s_Hits,TrackableType.PlaneWithinPolygon))
        {
            var hitPose = s_Hits[0].pose;
            _spawnPosition = hitPose.position + new Vector3(0, placeablePrefab.transform.localScale.y / 2f, 0); 
            if (_placedPrefabCount < _maxPrefabSpawnCount/* && !_selectableObjectHit*/)
            {
                SpawnPrefab(hitPose);
            }
        }

       /* Ray ray = new Ray(touchPosition,_ARCamera.transform.forward);
        int layerMask = 1 << 8;
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit) && hit.collider.gameObject.layer == LayerMask.NameToLayer("Selectable"))
        {
            _selectableObjectHit = true;
        } */
    }
    
    public void SetPrefabType(GameObject prefabType)
    {
        placeablePrefab = prefabType;
    }

    private void SpawnPrefab(Pose hitPose)
    {
        _spawnedObject = Instantiate(placeablePrefab, _spawnPosition, hitPose.rotation);
        placedPreafabList.Add(_spawnedObject);
        _placedPrefabCount++;
    }

    
}
