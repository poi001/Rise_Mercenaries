// 유닛이 속한 팀을 정의하는 열거형, 기본적으로 아군과 적군 두 팀
public enum EUnitTeam
{
    Ally,
    Enemy
}

// 스탯 적용 열거형
public enum EStatModType
{
    Flat,        // +10
    AddPercent,  // +(10%) : 합산 후 1회 적용
    MulPercent,  // *(1.1) : 곱연산 누적
    Override     // 값을 강제로 고정(우선순위 필요)
}