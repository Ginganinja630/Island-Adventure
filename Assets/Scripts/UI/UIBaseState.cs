namespace RPG.UI
{
    public abstract class UIBaseState
    {
        public UIController controller;

        public UIBaseState(UIController ui)
        {
            controller = ui;
        }

        public abstract void EnterState();

        public abstract void SelectButton();
    }
}