using System.Collections.Generic;
using UnityEngine;

public class UnitTargeting : MonoBehaviour
{


    public void AcquireTarget()
    {
        if (_battleController == null)
            return;

        IList<UnitController> opponents = _battleController.GetOpponents(Team);

        float closestDistance = float.MaxValue;
        UnitController closestUnit = null;

        foreach (UnitController unit in opponents)
        {
            if (unit == null || unit.State.IsDead)
                continue;

            float dist = Vector3.Distance(transform.position, unit.transform.position);

            if (dist < closestDistance)
            {
                closestDistance = dist;
                closestUnit = unit;
            }
        }

        _currentTarget = closestUnit;
    }
}
