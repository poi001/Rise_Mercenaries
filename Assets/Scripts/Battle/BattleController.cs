using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// BattleController는 단일 전투를 관리합니다.
/// 각 팀의 유닛 리스트를 추적하고,
/// 전투 종료 조건을 판단합니다.
/// </summary>
public class BattleController : MonoBehaviour
{
    private List<UnitController> _allies;
    private List<UnitController> _enemies;

    private bool _battleActive;

    /// <summary>
    /// 아군과 적군 리스트로 전투를 초기화합니다.
    /// 유닛은 이 컨트롤러를 통해 적 목록을 조회하고,
    /// 사망 시 알림을 보냅니다.
    /// </summary>
    public void InitializeBattle(List<UnitController> allies, List<UnitController> enemies)
    {
        _allies = allies;
        _enemies = enemies;
        _battleActive = true;

        Debug.Log("전투 시작 - 아군: " + allies.Count + " / 적군: " + enemies.Count);
    }

    /// <summary>
    /// 특정 팀 기준으로 상대 유닛 목록을 반환합니다.
    /// </summary>
    public IList<UnitController> GetOpponents(EUnitTeam team)
    {
        return team == EUnitTeam.Ally ? _enemies : _allies;
    }

    /// <summary>
    /// 유닛 사망 시 호출됩니다.
    /// 해당 팀 리스트에서 제거하고,
    /// 전투 종료 여부를 확인합니다.
    /// </summary>
    public void NotifyUnitDied(UnitController unit)
    {
        if (unit.Team == EUnitTeam.Ally)
            _allies.Remove(unit);
        else
            _enemies.Remove(unit);

        CheckBattleEnd();
    }

    /// <summary>
    /// 한 팀이 전멸했는지 확인하고,
    /// 전투 종료를 처리합니다.
    /// </summary>
    private void CheckBattleEnd()
    {
        if (!_battleActive)
            return;

        bool alliesAlive = _allies != null && _allies.Count > 0;
        bool enemiesAlive = _enemies != null && _enemies.Count > 0;

        if (!alliesAlive || !enemiesAlive)
        {
            _battleActive = false;
            string winner = alliesAlive ? "아군 승리" : "적군 승리";
            Debug.Log("전투 종료 - 결과: " + winner);
        }
    }
}