using AxGrid;
using AxGrid.FSM;
using AxGrid.Model;
using AxGrid.Path;

namespace TASK3.Scripts.States
{
    [State("Spinning")]
    public class SpinningState : FSMState
    {
        private CPath _path;
        private float _stopUnlockTimer;
        private bool _stopUnlocked;

        [Enter]
        private void OnEnter()
        {
            Log.Debug($">>> STATE: {Parent.CurrentStateName}");
            Settings.Model.Set("BtnStartEnable", false);
            Settings.Model.Set("BtnStopEnable", false);

            _stopUnlockTimer = 0f;
            _stopUnlocked = false;

            var startSpeed = Settings.Model.GetFloat("Speed");
            var maxSpeed = Settings.Model.GetFloat("MaxSpeed");
            var accelTime = Settings.Model.GetFloat("AccelerationTime");

            _path = new CPath()
                .EasingCircEaseIn(accelTime, startSpeed, maxSpeed, speed =>
                {
                    Settings.Model.Set("Speed", speed);
                });
        }

        [Loop(0f)]
        private void Tick(float deltaTime)
        {
            _path?.Update(deltaTime);

            if (_stopUnlocked)
                return;

            _stopUnlockTimer += deltaTime;

            var unlockDelay = Settings.Model.GetFloat("StopUnlockDelay");
            if (_stopUnlockTimer < unlockDelay)
                return;

            _stopUnlocked = true;
            Log.Debug($"[{Parent.CurrentStateName}] Stop unlocked!");
            Settings.Model.Set("BtnStopEnable", true);
        }

        [Bind("OnBtn")]
        private void OnButton(string btn)
        {
            if (btn == "Stop")
                Parent.Change("Stopping");
        }

        [Exit]
        private void OnExit()
        {
            _path?.StopPath();
            _path = null;
            Log.Debug($"<<< STATE: {Parent.CurrentStateName}");
        }
    }
}