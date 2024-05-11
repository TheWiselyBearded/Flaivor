using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class TransformConstraint : MonoBehaviour
{
    [SerializeField] private Space space;

    [SerializeField] private bool freezeRotation;

    [SerializeField] private Axis frozenAxes;

    private void LateUpdate()
    {
        if (freezeRotation)
        {
            switch (space)
            {
                case Space.World:
                    transform.rotation = Quaternion.Euler(
                        frozenAxes.HasFlag(Axis.X) ? 0 : transform.rotation.eulerAngles.x,
                        frozenAxes.HasFlag(Axis.Y) ? 0 : transform.rotation.eulerAngles.y,
                        frozenAxes.HasFlag(Axis.Z) ? 0 : transform.rotation.eulerAngles.z
                    );
                    break;
                case Space.Self:
                    transform.localRotation = Quaternion.Euler(
                        frozenAxes.HasFlag(Axis.X) ? 0 : transform.localRotation.eulerAngles.x,
                        frozenAxes.HasFlag(Axis.Y) ? 0 : transform.localRotation.eulerAngles.y,
                        frozenAxes.HasFlag(Axis.Z) ? 0 : transform.localRotation.eulerAngles.z
                    );
                    break;
            }
        }
    }

}
