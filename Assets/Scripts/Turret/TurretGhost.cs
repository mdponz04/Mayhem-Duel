using Assets.Scripts.TheCastle;
using System;
using System.Collections.Generic;
using TheCastle;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TurretGhost : NetworkBehaviour
{

    [Header("Indicate ray")]
    [SerializeField] private bool isRayActive;
    [SerializeField] private Transform rayOrigin;
    [SerializeField] private float rayMaxDistance;
    [SerializeField] private LayerMask layersRayHit;
    [SerializeField] private PlacedObjectTypeSO objectToPlace;
    [SerializeField] private TurretType turretTypeToPlace;

    [Header("Ghost")]
    [SerializeField] private Vector3 ghostScalling = new Vector3(0.3f, 0.3f, 0.3f);
    [SerializeField] private int layerToGhost;

    [Header("Item")]
    [SerializeField] private bool isDestroyAfterUse = false;


    private Vector3 direction;
    private LineRenderer lineRender;
    private Transform visual;

    private void Start()
    {
        lineRender = GetComponent<LineRenderer>();
    }

    private void Update()
    {

        if (isRayActive)
        {

            RaycastHit rayHit = FireRay();
            if (rayHit.collider != null)
            {
                Vector3 ghostPosition = GridSystem.Instance.GetWorldSnappedPosition(rayHit.point, out bool validPosition);
                if (validPosition)
                {
                    TurnOnVisual(ghostPosition);
                    MoveVisual(ghostPosition);

                }
            }
            else
            {
                TurnOffVisual();
            }
        }
        else
        {
            TurnOffVisual();
        }
        //Getting position in grid
        // position != null
        //do logic
        //place "animation"
    }

    public void PlaceTurretOnGrid()
    {
        Debug.Log("Placing Turret");
        RaycastHit rayHit = FireRay();
        if (rayHit.collider != null && objectToPlace != null)
        {
            Vector3 snappedPosition = GridSystem.Instance.GetWorldSnappedPosition(rayHit.point, out bool validPosition);
            if (validPosition)
            {
                GridSystem.Instance.GridObjectPlace(snappedPosition, objectToPlace);

                if (isDestroyAfterUse)
                {
                    DestroyItem();
                }
            }
        }
    }
    public void PlaceTurretOnGridServer()
    {
        Debug.Log("Placing Turret");
        RaycastHit rayHit = FireRay();
        if (rayHit.collider != null)
        {
            Vector3 snappedPosition = GridSystem.Instance.GetWorldSnappedPosition(rayHit.point, out bool validPosition);
            if (validPosition)
            {
                GridSystem.Instance.GridObjectPlaceServerRpc(snappedPosition, turretTypeToPlace);

                if (isDestroyAfterUse)
                {
                    DestroyItem();
                }
            }
        }
    }

    public void DestroyItem()
    {
        if (gameObject != null)
        {
            Destroy(gameObject, Time.deltaTime);
        }
    }

    #region Handle Visual
    private void TurnOnVisual(Vector3 visualPosition)
    {
        if (visualPosition == Vector3.positiveInfinity)
        {
            return;
        }

        if (objectToPlace != null && visual == null)
        {
            visual = Instantiate(objectToPlace.visual, visualPosition, Quaternion.identity);
            visual.localScale = ghostScalling;
            //visual.parent = transform;
            //visual.localPosition = Vector3.zero;
            //visual.localEulerAngles = Vector3.zero;
            SetLayerRecursive(visual.gameObject, layerToGhost);
        }
    }

    private void TurnOffVisual()
    {
        if (visual != null)
        {
            Destroy(visual.gameObject);
            visual = null;
        }
    }

    private void MoveVisual(Vector3 targetPosition)
    {
        if (targetPosition == Vector3.positiveInfinity)
        {
            return;
        }

        if (visual != null)
        {
            targetPosition.y += 0.1f;
            visual.position = Vector3.Lerp(visual.position, targetPosition, Time.deltaTime * 15f);
        }
    }
    #endregion

    #region Handle Ray

    public void TurnOnRay()
    {
        isRayActive = true;
        lineRender.enabled = true;
    }
    public void TurnOffRay()
    {
        isRayActive = false;
        lineRender.enabled = false;
    }
    private RaycastHit FireRay()
    {
        //TODO: add activate/deactivate ray
        RaycastHit hit;
        Debug.DrawRay(rayOrigin.position, transform.rotation * Vector3.forward);
        if (Physics.Raycast(rayOrigin.position, transform.rotation * Vector3.forward, out hit, rayMaxDistance, layersRayHit))
        {
            DrawRayLine(hit.point);
        }
        else
        {
            DrawRayLine(rayOrigin.position + rayOrigin.forward * rayMaxDistance);
        }
        //Debug.Log(hit.collider);
        return hit;
    }

    private void DrawRayLine(Vector3 endPosition)
    {
        lineRender.SetPosition(0, rayOrigin.position);
        lineRender.SetPosition(1, endPosition);
    }
    #endregion

    #region Utils Method
    private void SetLayerRecursive(GameObject targetGameObject, int layer)
    {
        targetGameObject.layer = layer;
        foreach (Transform child in targetGameObject.transform)
        {
            SetLayerRecursive(child.gameObject, layer);
        }
    }
    #endregion
}
