/*******************************************************************
* COPYRIGHT       : 2025
* PROJECT         : AI Tutorial
* FILE NAME       : NPC.cs
* DESCRIPTION     : Behaviors for AI NPC
*
* REVISION HISTORY:
* Date             Author                    Comments
* ---------------------------------------------------------------------------
* 2005/04/18      Akram Taghavi-Burris      Created Interface
*
*
/******************************************************************/

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using LLMUnity;


namespace ProfessorAkram.AITutorial
{
    [RequireComponent(typeof(BoxCollider))]
    public class AIChatNPC : MonoBehaviour//, IInteractable
    {
        public event Action<LLMCharacter> OnStartConversation; // Event to notify when AI Conversation gets triggered
        public event Action OnEndConversation; //Event to notify when AI conversation ends

        [SerializeField] [Tooltip("GameObject that has the LLM Module")]
        private LLM _llmModel;

        [SerializeField] [Tooltip("GameObject to use for LLM Character")]
        private GameObject _llmCharacterGameObject;

        private LLMCharacter _llmCharacter;

        [SerializeField] [Tooltip("List of NCP AI character prompts")] [TextArea(3, 10)]
        private List<string> _aiCharacterPrompts;

        private string _aiCharacterPrompt;

        //Reference to the player input
        private PlayerInput _playerInput;

        // Async Task allows this method to be awaited without freezing the game loop.
        async void Start()
        {

            //Ensure BoxCollider is a trigger
            BoxCollider npcCollider = this.gameObject.GetComponent<BoxCollider>();
            npcCollider.isTrigger = true;

            //Get random AI prompt
            int randomIndex = UnityEngine.Random.Range(0, _aiCharacterPrompts.Count);
            _aiCharacterPrompt = _aiCharacterPrompts[randomIndex];

            await InitalizeLLMCharacter();

        } //end Start()



        //Create the LLM Character component and set defaults
        // Async Task allows this method to be awaited without freezing the game loop.
        async Task InitalizeLLMCharacter()
        {

            // disable gameObject so that theAwake is not called immediately
            //_llmCharacterGameObject.SetActive(false);

            //Get LLM Character component
            if (!_llmCharacterGameObject.gameObject.TryGetComponent<LLMCharacter>(out LLMCharacter llmCharacter))
            {
                // If the component is not found, add it
                _llmCharacter = _llmCharacterGameObject.gameObject.AddComponent<LLMCharacter>();
            }
            else
            {
                _llmCharacter = llmCharacter;
            }

            // Set it as a child of another GameObject
            _llmCharacterGameObject.transform.parent = this.transform;

            Debug.Log(_llmCharacter.prompt);

            //Set LLM Model and prompt
            _llmCharacter.llm = _llmModel;


            // set the character prompt
            _llmCharacter.SetPrompt(_aiCharacterPrompt);

            Debug.Log(_llmCharacter.prompt);

            // re-enable gameObject
            gameObject.SetActive(true);

            await Task.CompletedTask; // Complete immediately if nothing else async

        } //end InstantiateLLMCharacter()


        // Required by IInteractable Interface
        // Called when the player (or another object) interacts with the item. 
        public void OnInteract(GameObject interactor)
        {
            //Prepare player input for conversation
            Debug.Log($"Start AI Interaction");
            OnStartConversation?.Invoke(_llmCharacter);

        } //end Interact()


        private void OnTriggerEnter(Collider other)
        {
            //If player get the player input
            if (other.CompareTag("Player"))
            {
                OnInteract(other.gameObject);
                //   _playerInput = other.gameObject.GetComponent<PlayerInput>();
                //  _playerInput.enabled = false;
            }

        } //end OnTriggerEnter()

        public void EndConversation()
        {
            Debug.Log($"End Conversation");
            OnEndConversation?.Invoke();
        }

        private void OnTriggerExit(Collider other)
        {
            OnEndConversation?.Invoke();
            EndConversation();
        }


    } //end IInteractable
    
}//end namespace