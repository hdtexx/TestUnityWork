using AxGrid;
using AxGrid.Base;
using AxGrid.FSM;
using TASK3.Scripts.States;
using UnityEngine;

namespace TASK3.Scripts
{
    public class Bootstrap : MonoBehaviourExtBind
    {
        [SerializeField] private SlotConfig _config;

        [OnAwake]
        private void Initialize()
        {
            PublishConfig();
            CreateFsm();
        }

        [OnStart]
        private void Begin()
        {
            Settings.Fsm.Start("Idle");
        }

        [OnUpdate]
        private void Tick()
        {
            Settings.Fsm.Update(Time.deltaTime);
        }

        [OnDestroy]
        private void Cleanup()
        {
            Settings.Fsm = null;
        }

        private void PublishConfig()
        {
            Model.Set("CollectionName", _config.CollectionName);
            Model.Set("ItemHeight", _config.ItemHeight);
            Model.Set("MaxSpeed", _config.MaxSpeed);
            Model.Set("AccelerationTime", _config.AccelerationTime);
            Model.Set("DecelerationTime", _config.DecelerationTime);
            Model.Set("MinStopSlots", _config.MinStopSlots);
            Model.Set("StopUnlockDelay", _config.StopUnlockDelay);
            Model.Set("PulseScale", _config.PulseScale);
            Model.Set("PulseDuration", _config.PulseDuration);
        }

        private void CreateFsm()
        {
            Settings.Fsm = new FSM();
            Settings.Fsm.Add(new IdleState());
            Settings.Fsm.Add(new SpinningState());
            Settings.Fsm.Add(new StoppingState());
        }
    }
}