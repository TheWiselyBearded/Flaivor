using Oculus.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformHelper : MonoBehaviour {
    public GameObject PanelObject;

    private void Start() {
        if (PanelObject == null) PanelObject = gameObject.GetComponentInChildren<Grabbable>().gameObject;
    }

    public void SetTransform() {
        UITransformManager.Instance.SetVector3(PanelObject.transform);
    }

    private void OnEnable() {
        if (PanelObject == null) PanelObject = gameObject.GetComponentInChildren<Grabbable>().gameObject;
        if (UITransformManager.Instance != null) SetPoseFromTransform(UITransformManager.Instance.GetLatestTransform());
    }


    public void SetPoseFromTransform(Transform target)
    {
        if (target != null) {
            Debug.Log($"Updating transform to {target.transform.position} rot {target.transform.rotation}");
            PanelObject.transform.position = target.position;
            PanelObject.transform.rotation = target.rotation;
        }
    }
}
