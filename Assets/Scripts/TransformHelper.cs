using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformHelper : MonoBehaviour
{
    public void SetPoseFromTransform(Transform target)
    {
        transform.position = target.position;
        transform.rotation = target.rotation;
    }
}
