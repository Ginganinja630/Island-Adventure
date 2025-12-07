using UnityEngine;
using UnityEngine.UIElements;
using RPG.Core;

namespace RPG.UI
{
    public class UIMainMenuState : UIBaseState
    {
        private int sceneIndex;
        public UIMainMenuState(UIController ui) : base(ui) { }

        public override void EnterState()
        {
            if (PlayerPrefs.HasKey("SceneIndex"))
            {
                sceneIndex = PlayerPrefs.GetInt("SceneIndex");
                AddButton();
            }

            controller.mainMenuContainer.style.display = DisplayStyle.Flex;

            controller.buttons = controller.mainMenuContainer
                .Query<Button>(null, "menu-button")
                .ToList();

            controller.buttons[0].AddToClassList("active");
        }

        public override void SelectButton()
        {
            Button btn = controller.buttons[controller.currentSelection];

            switch (btn.name)
            {
                case "start-button":
                    // new game
                    PlayerPrefs.DeleteAll();
                    controller.StartCoroutine(SceneTransition.Initiate(1));
                    break;

                case "continue-button":
                    // load saved scene
                    controller.StartCoroutine(SceneTransition.Initiate(sceneIndex));
                    break;

            }
        }

        private void AddButton()
        {
            Button continueButton = new Button();

            continueButton.name = "continue-button";
            continueButton.AddToClassList("menu-button");
            continueButton.text = "Continue";

            VisualElement mainMenuButtons = controller.mainMenuContainer
                .Q<VisualElement>("buttons");

            mainMenuButtons.Add(continueButton);
        }
    }
}