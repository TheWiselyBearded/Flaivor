using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Temp : MonoBehaviour
{
    public GameObject root;
    public void SetParent()
    {
        root.transform.parent = null;
    }
}
