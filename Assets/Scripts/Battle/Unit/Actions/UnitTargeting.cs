using System.Collections.Generic;
using UnityEngine;

public class UnitTargeting : MonoBehaviour//MonoBehaviour삭제해야함
{

    /// <summary>
    /// 가장 가까운 적을 탐색하여 타겟으로 설정합니다.
    /// battleController: 배틀컨트롤러, currentTarget: 타겟의 유닛컨트롤러, team: 자신의 팀 정보
    /// </summary>
    public void AcquireTarget(BattleController battleController, out UnitController currentTarget, EUnitTeam team)
    {
        if (battleController == null)
        {
            currentTarget = null;
            return;
        }

        IList<UnitController> opponents = battleController.GetOpponents(team);

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

        currentTarget = closestUnit;
    }
}
