using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// BattleBootstrap은 단일 전투 인스턴스를 설정하는 역할을 합니다.
/// 미리 정의된 위치에 아군과 적군을 스폰하고,
/// 팀을 지정한 뒤 BattleController에 연결합니다.
/// 시작 씬의 빈 GameObject에 이 스크립트를 붙이고
/// 유닛 프리팹을 인스펙터에서 할당하세요.
/// </summary>
public class BattleBootstrap : MonoBehaviour
{
    [Header("유닛 프리팹")]
    [Tooltip("아군 유닛에 사용할 프리팹입니다. UnitController 컴포넌트가 반드시 포함되어 있어야 합니다.")]
    public GameObject allyPrefab;

    [Tooltip("적 유닛에 사용할 프리팹입니다. UnitController 컴포넌트가 반드시 포함되어 있어야 합니다.")]
    public GameObject enemyPrefab;

    [Header("스폰 개수")]
    [Tooltip("전투 시작 시 생성할 아군 유닛 수입니다.")]
    public int allyCount = 3;

    [Tooltip("전투 시작 시 생성할 적 유닛 수입니다.")]
    public int enemyCount = 3;

    [Header("스폰 배치")]
    [Tooltip("유닛을 배치할 때 각 유닛 사이의 간격입니다.")]
    public float spacing = 2.0f;

    [Tooltip("아군이 시작할 X 좌표입니다.")]
    public float allyStartX = -5.0f;

    [Tooltip("적군이 시작할 X 좌표입니다.")]
    public float enemyStartX = 5.0f;

    private BattleController battleController;

    void Start()
    {
        // 이 오브젝트에 BattleController를 추가하여 전투를 관리합니다.
        battleController = gameObject.AddComponent<BattleController>();

        var allies = new List<UnitController>();
        var enemies = new List<UnitController>();

        // 아군 생성 및 팀 설정
        for (int i = 0; i < allyCount; i++)
        {
            Vector3 position = new Vector3(allyStartX, 0f, i * spacing);
            GameObject allyGO = Instantiate(allyPrefab, position, Quaternion.identity);
            UnitController unit = allyGO.GetComponent<UnitController>();

            if (unit == null)
            {
                Debug.LogError("아군 프리팹에 UnitController가 없습니다.");
                continue;
            }

            unit.Team = UnitTeam.Ally;
            unit.SetBattleController(battleController);
            allies.Add(unit);
        }

        // 적군 생성 및 팀 설정
        for (int i = 0; i < enemyCount; i++)
        {
            Vector3 position = new Vector3(enemyStartX, 0f, i * spacing);
            GameObject enemyGO = Instantiate(enemyPrefab, position, Quaternion.identity);
            UnitController unit = enemyGO.GetComponent<UnitController>();

            if (unit == null)
            {
                Debug.LogError("적군 프리팹에 UnitController가 없습니다.");
                continue;
            }

            unit.Team = UnitTeam.Enemy;
            unit.SetBattleController(battleController);
            enemies.Add(unit);
        }

        // 전투 시작
        battleController.InitializeBattle(allies, enemies);
    }
}