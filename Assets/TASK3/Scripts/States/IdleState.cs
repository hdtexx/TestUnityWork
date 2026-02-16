using AxGrid;
using AxGrid.FSM;
using AxGrid.Model;

namespace TASK3.Scripts.States
{
    [State("Idle")]
    public class IdleState : FSMState
    {
        [Enter]
        private void OnEnter()
        {
            Log.Debug($">>> STATE: {Parent.CurrentStateName}");
            Settings.Model.Set("BtnStartEnable", true);
            Settings.Model.Set("BtnStopEnable", false);
            Settings.Model.Set("Speed", 0f);
        }

        [Bind("OnBtn")]
        private void OnButton(string btn)
        {
            if (btn == "Start") 
                Parent.Change("Spinning");
        }

        [Exit]
        private void OnExit()
        {
            Log.Debug($"<<< STATE: {Parent.CurrentStateName}");
        }
    }
}