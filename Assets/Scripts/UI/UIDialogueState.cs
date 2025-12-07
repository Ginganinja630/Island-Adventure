using UnityEngine.UIElements;
using UnityEngine;
using Ink.Runtime;
using UnityEngine.InputSystem;
using RPG.Utility;
using System.Collections.Generic;
using RPG.Character;

namespace RPG.UI
{
    public class UIDialogueState : UIBaseState
    {
        private VisualElement dialogueContainer;
        private Label dialogueText;
        private VisualElement nextButton;
        private VisualElement choicesGroup;
        private Story currentStory;
        private PlayerInput playerInputCmp;
        private NPCController npcControllerCmp;

        private bool hasChoices = false;

        public UIDialogueState(UIController ui) : base(ui) { }

        public override void EnterState()
        {
            dialogueContainer = controller.root.Q<VisualElement>("dialogue-container");
            dialogueText = dialogueContainer.Q<Label>("dialogue-text");
            nextButton = dialogueContainer.Q<VisualElement>("dialogue-next-button");
            choicesGroup = dialogueContainer.Q<VisualElement>("choices-group");

            dialogueContainer.style.display = DisplayStyle.Flex;

            playerInputCmp = GameObject.FindGameObjectWithTag(
                Constants.GAME_MANAGER_TAG
            ).GetComponent<PlayerInput>();
            playerInputCmp.SwitchCurrentActionMap(Constants.UI_ACTION_MAP);
        }

        public override void SelectButton()
        {
            UpdateDialogue();
        }

        public void SetStory(TextAsset inkJSON, GameObject npc)
        {
            currentStory = new Story(inkJSON.text);
            currentStory.BindExternalFunction("VerifyQuest", VerifyQuest);

            npcControllerCmp = npc.GetComponent<NPCController>();

            if (npcControllerCmp.hasQuestItem)
            {
                currentStory.ChoosePathString("postCompletion");
            }

            UpdateDialogue();
        }

        public void UpdateDialogue()
        {
            if (hasChoices)
            {
                currentStory.ChooseChoiceIndex(controller.currentSelection);
            }

            if (!currentStory.canContinue)
            {
                ExitDialogue();
                return;
            }

            dialogueText.text = currentStory.Continue();

            hasChoices = currentStory.currentChoices.Count > 0;

            if (hasChoices)
            {
                HandleNewChoices(currentStory.currentChoices);
            }
            else
            {
                nextButton.style.display = DisplayStyle.Flex;
                choicesGroup.style.display = DisplayStyle.None;
            }
        }

        private void HandleNewChoices(List<Choice> choices)
        {
            nextButton.style.display = DisplayStyle.None;
            choicesGroup.style.display = DisplayStyle.Flex;

            choicesGroup.Clear();
            controller.buttons?.Clear();

            choices.ForEach(CreateNewChoiceButton);

            controller.buttons = choicesGroup.Query<Button>().ToList();
            controller.buttons[0].AddToClassList("active");

            controller.currentSelection = 0;
        }

        private void CreateNewChoiceButton(Choice choice)
        {
            Button choiceButton = new Button();

            choiceButton.AddToClassList("menu-button");
            choiceButton.text = choice.text;
            choiceButton.style.marginRight = 20;

            choicesGroup.Add(choiceButton);
        }

        private void ExitDialogue()
        {
            dialogueContainer.style.display = DisplayStyle.None;
            playerInputCmp.SwitchCurrentActionMap(
                Constants.GAMEPLAY_ACTION_MAP
            );
        }

        public void VerifyQuest()
        {
            currentStory.variablesState["questCompleted"] = npcControllerCmp.CheckPlayerForQuestItem();
        }
    }
}