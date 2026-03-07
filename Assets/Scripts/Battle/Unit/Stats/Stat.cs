using System;
using System.Collections.Generic;
using UnityEngine;

public readonly struct StatModifier
{
    public readonly int Key;        // 버프 인스턴스 내부에서 유니크(또는 전역 유니크)해야 함
    public readonly int SourceID;   // “누가 걸었는지/어떤 버프인지” 출처
    public readonly float Value;
    public readonly EStatModType Type;
    public readonly int Priority;   // Override 우선순위 등

    public StatModifier(int key, int sourceID, float value, EStatModType type, int priority = 0)
    {
        Key = key;
        SourceID = sourceID;
        Value = value;
        Type = type;
        Priority = priority;
    }
}

public sealed class Stat
{
    private readonly List<StatModifier> _modifiers = new();
    private readonly Dictionary<int, int> _modifierIndexByKey = new(); // Key -> index

    private float _baseValue;
    private bool _isDirty = true;
    private float _cachedFinalValue;

    public Stat(float baseValue)
    {
        _baseValue = baseValue;
    }

    public float BaseValue
    {
        get => _baseValue;
        set
        {
            if (Mathf.Approximately(_baseValue, value)) return; //두 개의 float 값이 거의 같은지 비교( 미세한 오차때문 )
            _baseValue = value;
            _isDirty = true;
        }
    }

    public float FinalValue
    {
        get
        {
            if (_isDirty)
            {
                _cachedFinalValue = CalculateFinalValue();
                _isDirty = false;
            }
            return _cachedFinalValue;
        }
    }

    public IReadOnlyList<StatModifier> Modifiers => _modifiers; //값을 조회만 할 수 있고 수정은 할 수 없는 리스트 형태의 컬렉션

    /// <summary>
    /// 동일 Key가 있으면 업데이트, 없으면 추가
    /// </summary>
    public void AddOrUpdateModifier(in StatModifier modifier)
    {
        if (_modifierIndexByKey.TryGetValue(modifier.Key, out int index))
        {
            _modifiers[index] = modifier;
        }
        else
        {
            _modifierIndexByKey.Add(modifier.Key, _modifiers.Count);
            _modifiers.Add(modifier);
        }

        _isDirty = true;
    }

    public void RemoveModifierByKey(int key)
    {
        if (!_modifierIndexByKey.TryGetValue(key, out int index))
            return;

        RemoveAtSwapBack(index);
        _isDirty = true;
    }

    public void RemoveModifiersBySource(int sourceId)
    {
        // 최소 구현: 리스트에서 역순으로 훑으며 제거
        for (int i = _modifiers.Count - 1; i >= 0; i--)
        {
            if (_modifiers[i].SourceID == sourceId)
            {
                RemoveAtSwapBack(i);
                _isDirty = true;
            }
        }
    }

    public void ClearModifiers()
    {
        if (_modifiers.Count == 0) return;
        _modifiers.Clear();
        _modifierIndexByKey.Clear();
        _isDirty = true;
    }

    // RemoveAt은 해당 원소를 삭제 후 그 뒤 원소들을 앞으로 땡겨오기 때문에 시간 복잡도가 O(n)이다.
    // RemoveAtSwapBack은 해당 위치 원소 자리에 맨 뒤에 있는 원소를 넣고 맨 뒤 원소를 지우기 때문에 시간 복잡도가 O(1)이다.
    private void RemoveAtSwapBack(int index)
    {
        int lastIndex = _modifiers.Count - 1;
        StatModifier removed = _modifiers[index];

        if (index != lastIndex)
        {
            StatModifier last = _modifiers[lastIndex];
            _modifiers[index] = last;
            _modifierIndexByKey[last.Key] = index;
        }

        _modifiers.RemoveAt(lastIndex);
        _modifierIndexByKey.Remove(removed.Key);
    }

    private float CalculateFinalValue()
    {
        // Override(고정값) 처리: 우선순위 높은 것, 동률이면 “마지막 적용(리스트 뒤)”을 선택
        bool hasOverride = false;
        float overrideValue = 0f;
        int bestPriority = int.MinValue;
        int bestOrder = -1;

        float flatSum = 0f;
        float addPercentSum = 0f;
        float mulPercent = 1f;

        for (int i = 0; i < _modifiers.Count; i++)
        {
            var mod = _modifiers[i];
            switch (mod.Type)
            {
                case EStatModType.Override:
                    if (!hasOverride || mod.Priority > bestPriority || (mod.Priority == bestPriority && i > bestOrder))
                    {
                        hasOverride = true;
                        bestPriority = mod.Priority;
                        bestOrder = i;
                        overrideValue = mod.Value;
                    }
                    break;

                case EStatModType.Flat:
                    flatSum += mod.Value;
                    break;

                case EStatModType.AddPercent:
                    addPercentSum += mod.Value;
                    break;

                case EStatModType.MulPercent:
                    mulPercent *= 1f + (mod.Value / 100f);
                    break;
            }
        }

        if (hasOverride) return overrideValue;

        float value = _baseValue + flatSum;
        value *= 1f + (addPercentSum / 100f);
        value *= mulPercent;
        return value;
    }
}