using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using RPG.Core;
using RPG.Quest;

namespace RPG.UI
{
    public class UIController : MonoBehaviour
    {
        private UIDocument uIDocumentCmp;
        public VisualElement root;
        public List<Button> buttons = new List<Button>();
        public VisualElement mainMenuContainer;
        public VisualElement playerInfoContainer;
        public Label healthLabel;
        public Label potionsLabel;
        private VisualElement questItemIcon;
        public AudioClip gameOverAudio;
        public AudioClip victoryAudio;
        [NonSerialized] public AudioSource audioSourceCmp;

        public UIBaseState currentState;
        public UIMainMenuState mainMenuState;
        public UIDialogueState dialogueState;
        public UIQuestItemState questItemState;
        public UIVictoryState victoryState;
        public UIGameOverState gameOverState;

        public int currentSelection = 0;

        private void Awake()
        {
            audioSourceCmp= GetComponent<AudioSource>();
            uIDocumentCmp = GetComponent<UIDocument>();
            if (uIDocumentCmp == null)
            {
                Debug.LogError("UIController: No UIDocument component found on this GameObject.", this);
                return;
            }

            root = uIDocumentCmp.rootVisualElement;
            if (root == null)
            {
                Debug.LogError("UIController: rootVisualElement is null.", this);
                return;
            }

            // Main menu container (might only exist in scene 0)
            mainMenuContainer = root.Q<VisualElement>("main-menu-container");

            // Player info container (might only exist in gameplay scene)
            playerInfoContainer = root.Q<VisualElement>("player-info-container");
            if (playerInfoContainer != null)
            {
                healthLabel   = playerInfoContainer.Q<Label>("health-label");
                potionsLabel  = playerInfoContainer.Q<Label>("potions-label");
                questItemIcon = playerInfoContainer.Q<VisualElement>("quest-item-icon");
            }
            else
            {
                Debug.LogWarning("UIController: 'player-info-container' not found in this UXML. This is OK in the main menu scene.");
            }


            mainMenuState  = new UIMainMenuState(this);
            dialogueState  = new UIDialogueState(this);
            questItemState = new UIQuestItemState(this);
            victoryState = new UIVictoryState(this);
            gameOverState = new UIGameOverState(this);
        }

        private void OnEnable()
        {
            EventManager.OnChangePlayerHealth += HandleChangePlayerHealth;
            EventManager.OnChangePlayerPotions += HandleChangePlayerPotions;
            EventManager.OnInitiateDialogue += HandleInitiateDialogue;
            EventManager.OnTreasureChestUnlocked += HandleTreasureChestUnlocked;
            EventManager.OnVictory += HandleVictory;
            EventManager.ONGameOver += HandleGameOVer;
        }


        // Start is called before the first frame update
        void Start()
        {
            int sceneIndex = SceneManager.GetActiveScene().buildIndex;

            if (sceneIndex == 0)
            {
                currentState = mainMenuState;
                currentState.EnterState();
            }
            else
            {
                playerInfoContainer.style.display = DisplayStyle.Flex;
            }
        }

        private void OnDisable()
        {
            EventManager.OnChangePlayerHealth -= HandleChangePlayerHealth;
            EventManager.OnChangePlayerPotions -= HandleChangePlayerPotions;
            EventManager.OnInitiateDialogue -= HandleInitiateDialogue;
            EventManager.OnTreasureChestUnlocked -= HandleTreasureChestUnlocked;
            EventManager.OnVictory -= HandleVictory;
            EventManager.ONGameOver -= HandleGameOVer;
        }


        public void HandleInteract(InputAction.CallbackContext context)
        {
            if (!context.performed) return;

            if (currentState == null)
            {
                Debug.LogWarning("UIController.HandleInteract called but currentState is null.");
                return;
            }

            currentState.SelectButton();
        }

        public void HandleNavigate(InputAction.CallbackContext context)
        {
            if (!context.performed || buttons.Count == 0) return;

            Vector2 input = context.ReadValue<Vector2>();

            // Debug so we can SEE what is happening
            Debug.Log($"[UIController] Navigate input: {input}, currentSelection before: {currentSelection}, buttons.Count: {buttons.Count}");

            // Remove active from current
            buttons[currentSelection].RemoveFromClassList("active");

            int direction = 0;

            // Decide whether we’re moving horizontally or vertically
            if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
            {
                // Left / Right
                direction = input.x > 0 ? 1 : -1;
            }
            else
            {
                // Up / Down (note: in Input System, up = +1, down = -1)
                direction = input.y < 0 ? 1 : -1;
            }

            currentSelection += direction;
            currentSelection = Mathf.Clamp(currentSelection, 0, buttons.Count - 1);

            // Add active to new one
            buttons[currentSelection].AddToClassList("active");

            Debug.Log($"[UIController] New selection index: {currentSelection}, new button: {buttons[currentSelection].name}");
        }
        
        private void HandleChangePlayerHealth(float newHealthPoints)
        {
            healthLabel.text = newHealthPoints.ToString();
        }

        private void HandleChangePlayerPotions(int newPotionCount)
        {
            potionsLabel.text = newPotionCount.ToString();
        }

        private void HandleInitiateDialogue(TextAsset inkJSON, GameObject npc)
        {
            currentState = dialogueState;
            currentState.EnterState();

            (currentState as UIDialogueState).SetStory(inkJSON, npc);
        }

        private void HandleTreasureChestUnlocked(QuestItemSO item, bool showUI)
        {
            questItemIcon.style.display = DisplayStyle.Flex;

            if (!showUI) return;

            currentState = questItemState;
            currentState.EnterState();

            (currentState as UIQuestItemState).SetQuestItemLabel(item.itemName);
        }

        private void HandleVictory()
        {
            currentState = victoryState;
            currentState.EnterState();
        }

        private void HandleGameOVer()
        {
            currentState = gameOverState;
            currentState.EnterState();
        }
    }
}