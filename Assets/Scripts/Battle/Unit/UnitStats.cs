using UnityEngine;

/// <summary>
/// 유닛이 속한 팀을 정의하는 열거형입니다.
/// 기본적으로 아군과 적군 두 팀이 있습니다.
/// </summary>
public enum UnitTeam
{
    Ally,
    Enemy
}

/// <summary>
/// 유닛의 기본 능력치를 정의합니다.
/// 전투 중 변경되지 않는 값입니다.
/// </summary>
[System.Serializable]
public class UnitStats
{
    [Header("체력")]
    [Tooltip("유닛의 최대 체력입니다.")]
    public float MaxHP = 100f;

    [Header("전투")]
    [Tooltip("기본 공격 1회당 가하는 피해량입니다.")]
    public float Attack = 10f;

    [Tooltip("공격 속도(초당 공격 횟수). 1이면 1초에 1회 공격합니다.")]
    public float AttackSpeed = 1f;

    [Tooltip("공격 가능한 최대 사거리입니다.")]
    public float AttackRange = 2f;

    [Tooltip("초당 이동 속도입니다.")]
    public float MoveSpeed = 3f;
}

/// <summary>
/// 전투 중 변하는 유닛의 상태값을 저장합니다.
/// </summary>
public class UnitState
{
    /// <summary>
    /// 현재 체력. 0 이하가 되면 사망합니다.
    /// </summary>
    public float CurrentHP;

    /// <summary>
    /// 다음 공격까지 남은 시간입니다.
    /// 0 이하가 되면 공격이 가능합니다.
    /// </summary>
    public float AttackCooldown;

    /// <summary>
    /// 사망 여부 확인용 속성
    /// </summary>
    public bool IsDead => CurrentHP <= 0f;
}