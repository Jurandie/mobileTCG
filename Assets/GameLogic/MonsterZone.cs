using UnityEngine;

public class MonsterZone : MonoBehaviour
{
    public MonsterZoneSlot[] slots;

    void Awake()
    {
        slots = GetComponentsInChildren<MonsterZoneSlot>();
    }
}
