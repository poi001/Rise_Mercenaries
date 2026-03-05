using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// UnitController는 각 유닛에 부착됩니다.
/// 이동, 타겟팅, 공격, 피해 처리, 사망을 담당합니다.
/// </summary>
public class UnitController : MonoBehaviour
{
    [Header("기본 능력치")]
    [Tooltip("체력, 공격력, 속도 등의 기본 능력치입니다.")]
    public UnitStats BaseStats;

    private UnitState _state;
    private BattleController _battleController;
    private UnitController _currentTarget;

    /// <summary>
    /// 유닛이 속한 팀
    /// </summary>
    public UnitTeam Team { get; set; }

    /// <summary>
    /// 현재 상태를 외부에서 읽기 전용으로 접근하기 위한 프로퍼티
    /// </summary>
    public UnitState State => _state;

    /// <summary>
    /// BattleController를 설정합니다.
    /// </summary>
    public void SetBattleController(BattleController controller)
    {
        _battleController = controller;
    }

    // 나중에 Init이라는 함수로 초기설정 대체할 예정
    void Awake()
    {
        // 상태 초기화
        _state = new UnitState
        {
            CurrentHP = BaseStats != null ? BaseStats.MaxHP : 100f,
            AttackCooldown = 0f
        };
    }

    void Update()
    {
        if (_state.IsDead)
            return;

        if (_state.AttackCooldown > 0f)
            _state.AttackCooldown -= Time.deltaTime;

        if (_currentTarget == null || _currentTarget.State.IsDead)
            AcquireTarget();

        if (_currentTarget != null && !_currentTarget.State.IsDead)
        {
            float distance = Vector3.Distance(transform.position, _currentTarget.transform.position);

            if (distance <= BaseStats.AttackRange)
                TryAttack();
            else
                MoveTowardsTarget();
        }
    }

    /// <summary>
    /// 가장 가까운 적을 탐색하여 타겟으로 설정합니다.
    /// </summary>
    private void AcquireTarget()
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

    /// <summary>
    /// 타겟을 향해 이동합니다.
    /// </summary>
    private void MoveTowardsTarget()
    {
        Vector3 direction = (_currentTarget.transform.position - transform.position).normalized;
        Vector3 movement = direction * BaseStats.MoveSpeed * Time.deltaTime;
        transform.position += movement;
    }

    /// <summary>
    /// 기본 공격을 시도합니다.
    /// </summary>
    private void TryAttack()
    {
        if (_state.AttackCooldown > 0f || _currentTarget == null)
            return;

        _currentTarget.TakeDamage(BaseStats.Attack);

        _state.AttackCooldown = BaseStats.AttackSpeed > 0f
            ? 1f / BaseStats.AttackSpeed
            : 0f;
    }

    /// <summary>
    /// 피해를 적용합니다.
    /// 체력이 0 이하가 되면 사망 처리합니다.
    /// </summary>
    public void TakeDamage(float amount)
    {
        if (_state.IsDead)
            return;

        _state.CurrentHP -= amount;

        if (_state.CurrentHP <= 0f)
            Die();
    }

    /// <summary>
    /// 사망 처리
    /// </summary>
    private void Die()
    {
        _state.CurrentHP = 0f;

        if (_battleController != null)
            _battleController.NotifyUnitDied(this);

        gameObject.SetActive(false);
    }
}