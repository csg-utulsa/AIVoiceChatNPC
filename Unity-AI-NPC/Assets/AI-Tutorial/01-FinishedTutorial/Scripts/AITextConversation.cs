/*******************************************************************
* COPYRIGHT       : 2025
* PROJECT         : AI NPC Tutorial - Transformers AI & Games Conference
* FILE NAME       : AITextConversation.cs
* DESCRIPTION     : Handles the AI Text Interaction
* REQUIREMENTS    : Requires LLM Unity package https://github.com/undreamai/LLMUnity
*
* REVISION HISTORY:
* Date             Author                    Comments
* ---------------------------------------------------------------------------
* 2005/04/18      Akram Taghavi-Burris      Created Class
*
*
/******************************************************************/


using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using LLMUnity;
using StarterAssets;
using UnityEngine.Events; //reference to LLM Unity namespace


namespace ProfessorAkram.AITutorial
{
    public class AITextConversation : MonoBehaviour
    {
        [Header("Game Actors")]
        [SerializeField]
        [Tooltip("Reference to player input component")]
        private PlayerInput _playerInput;
        // private FirstersonController _playerController;
        // [SerializeField]
        // [Tooltip("String name of the player action map")]
        private string _playerActionMapName;
        [SerializeField]
        [Tooltip("String name of the ui action map")]
        private string _uiActionMapName;
        
        [SerializeField]
        [Tooltip("Reference to ai character NPC component")]
        private AIChatNPC _aiChatNPC;
        private LLMCharacter _llmCharacter;

        [Header("Text Conversation Components")]
        [SerializeField]
        [Tooltip("Canvas for the AI conversation")]
        private Canvas _aiCanvas; //Canvas for aI 
        
        [SerializeField]
        [Tooltip("Input Text for the player")]
        private InputField _playerInputText;
        
        [SerializeField]
        [Tooltip("Text for the AI response")]
        private Text _aiResponseText;

        [SerializeField]
        private RunJets TTSOutput;

        
        
        
        void Start()
        {
            //Disable the AI conversation canvas at start
            _aiCanvas.enabled = false;

            Debug.Log(_playerActionMapName);
            Debug.Log(_uiActionMapName);
            
            //Deactivate the player input field
            _playerInputText.interactable = false;

        }//end Start()
        
        /// <summary>
        /// Behaviors for toggling player input to ui input and cursor
        /// </summary>
// This method toggles between UI and Player controls
        private void ToggleUI()
        {
            if (_playerInput.currentActionMap.name == "UI")
            {
                // Switch back to Player controls
                _playerInput.SwitchCurrentActionMap("Player");
                LockCursor();
            }
            else
            {
                // Switch to UI controls
                _playerInput.SwitchCurrentActionMap("UI");
                UnlockCursor();
            }
        }//end ToggleUI

        private void UnlockCursor()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            
        }// end UnlockCursor()

        private void LockCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }//end LockCursor()
      
        
        
        // Subscribe to the SceneDirectorReady event
        private void OnEnable()
        {
            _aiChatNPC.OnStartConversation += HandleStartConversation;
            _aiChatNPC.OnEndConversation += HandleEndConversation;
        }

        // Unsubscribe to avoid memory leaks
        private void OnDisable()
        {
            _aiChatNPC.OnStartConversation -= HandleStartConversation;
            _aiChatNPC.OnEndConversation -= HandleEndConversation;
        }

        //Handle the start of the conversation
        private void HandleStartConversation(LLMCharacter llmCharacterComponent)
        {
            Debug.Log("HandleStartConversation");
            //Toggle UI interaction
            ToggleUI();
            UnlockCursor();
            
            _llmCharacter = llmCharacterComponent;
            

            //Check if llm model has been set, then proceed with conversation
            if (IsModelSet())
            {
                //enable the AI conversation canvas
                _aiCanvas.enabled = true;
  
                //Default player text input
                _playerInputText.text = "Talk to NPC";
                
                //Activate the player input field
                _playerInputText.interactable = true;
                
                //Add listener for text submission
                _playerInputText.onSubmit.AddListener(OnInputFieldSubmit);
                _playerInputText.Select();
               
                
            }//end if (IsModelSet())
            
        }//end HandleStartConversation()
        
        //Handle the end of the conversation
        private void HandleEndConversation()
        {
            //If the canvas is already hidden exit early
            if(_aiCanvas.enabled == false) return;
            
            //Toggle UI interaction
            ToggleUI();
            LockCursor();
            
            _playerInput.SwitchCurrentActionMap(_playerActionMapName);
            
            //Remove text submission listen
            _playerInputText.onSubmit.RemoveListener(OnInputFieldSubmit);
            
            //Disable the AI conversation canvas at start
            _aiCanvas.enabled = false;
            
        }//end HandleEndConversation()


        
        
        //When player input is submitted
        void OnInputFieldSubmit(string message)
        {
            //Deactivate the player input field
            _playerInputText.interactable = false;
            
            //Set AI initial response
            _aiResponseText.text = "...";
            
            //Initiate chat but ignore any return values
            _ =  _llmCharacter.Chat(message, PostAIResponse, OnReplyCompleted);
            
        }//end OnInputFieldSubmit
        

        //Post the AI response to the text box
        private void PostAIResponse(string text)
        {
            Debug.Log(text);
            _aiResponseText.text = text;

            
        }//end PostAIResponse()
        

        //When AI reply is completed
        private void OnReplyCompleted()
        {
            _playerInputText.interactable = true;
            _playerInputText.Select();
            _playerInputText.text = "";
            TTSOutput.inputText = _aiResponseText.text;
            Debug.Log("REPLY COMPLETE"); //do not touch, load bearing debug
            TTSOutput.isTextNew = true;

            
        }//end OnReplyCompleted()
        

        //Cancel AI request
        public void OnCancelRequests()
        {
            _llmCharacter.CancelRequests();
            OnReplyCompleted();
            
        }//end CancelRequest()
        
        
        //End Conversation with AI
        public void EndConversation()
        {
            _aiChatNPC.EndConversation();

        }//end CancelRequest()
        

        
        //Validate that the LLM model has been assigned
       private bool IsModelSet()
        {
            if (! _llmCharacter.remote &&  _llmCharacter.llm != null &&  _llmCharacter.llm.model == "")
            {
                Debug.LogWarning($"Please select a model in the { _llmCharacter.llm.gameObject.name} GameObject!");
                return false;
            }
            
            return true;
            
        }//end OnValidate()
        
    }//end AITextInteraction
    
}//end namespace
