using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 유닛의 기본 능력치를 정의합니다.
/// 전투 중 변경되지 않는 값입니다.
/// </summary>
[System.Serializable]
public class UnitStats
{
    //[Header("체력")]
    //[Tooltip("유닛의 최대 체력입니다.")]
    //public float MaxHP = 100f;

    //[Header("전투")]
    //[Tooltip("기본 공격 1회당 가하는 피해량입니다.")]
    //public float Attack = 10f;

    //[Tooltip("공격 속도(초당 공격 횟수). 1이면 1초에 1회 공격합니다.")]
    //public float AttackSpeed = 1f;

    //[Tooltip("공격 가능한 최대 사거리입니다.")]
    //public float AttackRange = 2f;

    //[Tooltip("초당 이동 속도입니다.")]
    //public float MoveSpeed = 3f;


    // 방어 관련
    public Stat MaxHP;
    public Stat Armor;
    public Stat MR;

    // 공격 관련
    public Stat AD;
    public Stat AP;
    public Stat ArPen;
    public Stat MrPen;
    public Stat AS;
    public Stat Crit;
    public Stat Range;

    // 유틸 관련
    public Stat MaxMana;
    public Stat MS;
    public Stat Lifesteal;
}

/// <summary>
/// 전투 중 변하는 유닛의 상태값을 저장합니다.
/// </summary>
public class UnitState
{
    // 현재 체력. 0 이하가 되면 사망합니다.
    public float CurrentHP;

    // 다음 공격까지 남은 시간입니다. 0 이하가 되면 공격이 가능합니다.
    public float AttackCooldown;

    // 사망 여부 확인용 속성
    public bool IsDead => CurrentHP <= 0.0f;
}