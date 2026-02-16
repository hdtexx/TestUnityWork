using AxGrid;
using AxGrid.FSM;

namespace TASK3.Scripts.States
{
    [State("Stopping")]
    public class StoppingState : FSMState
    {
        [Enter]
        private void OnEnter()
        {
            Log.Debug($">>> STATE: {Parent.CurrentStateName}");
            Settings.Model.Set("BtnStartEnable", false);
            Settings.Model.Set("BtnStopEnable", false);
            Settings.Model.Set("AnimationDone", false);

            var itemHeight = Settings.Model.GetFloat("ItemHeight");
            var minStopSlots = Settings.Model.GetInt("MinStopSlots");

            Settings.Model.Set("SnapDistance", minStopSlots * itemHeight);
            Settings.Model.Set("IsStopping", true);
        }

        [Loop(0f)]
        private void Tick(float deltaTime)
        {
            if (false == Settings.Model.GetBool("AnimationDone"))
                return;

            Log.Debug($"[{Parent.CurrentStateName}] Animation done, going Idle");
            Parent.Change("Idle");
        }

        [Exit]
        private void OnExit()
        {
            Settings.Model.Set("IsStopping", false);
            Log.Debug($"<<< STATE: {Parent.CurrentStateName}");
        }
    }
}