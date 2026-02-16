using UnityEngine;

namespace TASK3.Scripts
{
    [CreateAssetMenu(fileName = "SlotConfig", menuName = "TASK3/SlotConfig")]
    public class SlotConfig : ScriptableObject
    {
        [Header("Спрайты")]
        public string CollectionName = "Slots";

        [Header("Визуал слота")]
        public float ItemHeight = 300f;

        [Header("Разгон")]
        public float MaxSpeed = 5000f;
        public float AccelerationTime = 1f;

        [Header("Торможение")]
        public float DecelerationTime = 2f;
        public int MinStopSlots = 3;

        [Header("Задержка разблокировки Stop")]
        public float StopUnlockDelay = 3f;

        [Header("Пульс центральной картинки")]
        public float PulseScale = 1.05f;
        public float PulseDuration = 0.45f;
    }
}
