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
    public UnitStats baseStats;

    private UnitState state;
    private BattleController battleController;
    private UnitController currentTarget;

    /// <summary>
    /// 유닛이 속한 팀
    /// </summary>
    public UnitTeam Team { get; set; }

    /// <summary>
    /// 현재 상태를 외부에서 읽기 전용으로 접근하기 위한 프로퍼티
    /// </summary>
    public UnitState State => state;

    /// <summary>
    /// BattleController를 설정합니다.
    /// </summary>
    public void SetBattleController(BattleController controller)
    {
        battleController = controller;
    }

    void Awake()
    {
        // 상태 초기화
        state = new UnitState
        {
            CurrentHP = baseStats != null ? baseStats.MaxHP : 100f,
            AttackCooldown = 0f
        };
    }

    void Update()
    {
        if (state.IsDead)
            return;

        if (state.AttackCooldown > 0f)
            state.AttackCooldown -= Time.deltaTime;

        if (currentTarget == null || currentTarget.State.IsDead)
            AcquireTarget();

        if (currentTarget != null && !currentTarget.State.IsDead)
        {
            float distance = Vector3.Distance(transform.position, currentTarget.transform.position);

            if (distance <= baseStats.AttackRange)
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
        if (battleController == null)
            return;

        IList<UnitController> opponents = battleController.GetOpponents(Team);

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

    /// <summary>
    /// 타겟을 향해 이동합니다.
    /// </summary>
    private void MoveTowardsTarget()
    {
        Vector3 direction = (currentTarget.transform.position - transform.position).normalized;
        Vector3 movement = direction * baseStats.MoveSpeed * Time.deltaTime;
        transform.position += movement;
    }

    /// <summary>
    /// 기본 공격을 시도합니다.
    /// </summary>
    private void TryAttack()
    {
        if (state.AttackCooldown > 0f || currentTarget == null)
            return;

        currentTarget.TakeDamage(baseStats.Attack);

        state.AttackCooldown = baseStats.AttackSpeed > 0f
            ? 1f / baseStats.AttackSpeed
            : 0f;
    }

    /// <summary>
    /// 피해를 적용합니다.
    /// 체력이 0 이하가 되면 사망 처리합니다.
    /// </summary>
    public void TakeDamage(float amount)
    {
        if (state.IsDead)
            return;

        state.CurrentHP -= amount;

        if (state.CurrentHP <= 0f)
            Die();
    }

    /// <summary>
    /// 사망 처리
    /// </summary>
    private void Die()
    {
        state.CurrentHP = 0f;

        if (battleController != null)
            battleController.NotifyUnitDied(this);

        gameObject.SetActive(false);
    }
}