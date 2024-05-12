namespace RedBlueGames.Tools.TextTyper
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using RedBlueGames.Tools.TextTyper;
    using UnityEngine.UI;
    using TMPro;

    /// <summary>
    /// Class that tests TextTyper and shows how to interface with it.
    /// </summary>
    public class TextTyperTester : MonoBehaviour
    {
#pragma warning disable 0649 // Ignore "Field is never assigned to" warning, as these are assigned in inspector
        [SerializeField]
        private AudioClip printSoundEffect;

        [Header("UI References")]

        [SerializeField]
        private Button printNextButton;


  

        private Queue<string> dialogueLines = new Queue<string>();

        [SerializeField]
        [Tooltip("The text typer element to test typing with")]
        private TextTyper testTextTyper;

#pragma warning restore 0649
        public void Start()
        {
            this.testTextTyper.PrintCompleted.AddListener(this.HandlePrintCompleted);
            this.testTextTyper.CharacterPrinted.AddListener(this.HandleCharacterPrinted);

            this.printNextButton.onClick.AddListener(this.HandlePrintNextClicked);

            dialogueLines.Enqueue("<animation=slowsine>CAWWW!</animation>\n(Hey! You look lost.)");
            dialogueLines.Enqueue("<anim=lightpos>CA KAWW.... KAWW...</animation>\n(Seems like your room flooded...\nthat sucks...)?");
            dialogueLines.Enqueue("<animation=bounce>KAWWW!!!</animation>\n(But here, I’ll give you some tools\nto keep you company!)");
            dialogueLines.Enqueue("<animation=bounce>CAW, CA KAWW!!</animation>\n(You can access them by moving \nyour right thumbstick. Try it!)");
            dialogueLines.Enqueue("<animation=slowsine>CAWW!!</animation>\n(Okay, Bye! Good luck! )");
            ShowScript();
        }

        public void Update()
        {
         
            if (Input.GetKeyDown(KeyCode.Space))
            {

                var tag = RichTextTag.ParseNext("blah<color=red>boo</color");
                LogTag(tag);
                tag = RichTextTag.ParseNext("<color=blue>blue</color");
                LogTag(tag);
                tag = RichTextTag.ParseNext("No tag in here");
                LogTag(tag);
                tag = RichTextTag.ParseNext("No <color=blueblue</color tag here either");
                LogTag(tag);
                tag = RichTextTag.ParseNext("This tag is a closing tag </bold>");
                LogTag(tag);
            }
        }

        private void HandlePrintNextClicked()
        {
            if (this.testTextTyper.IsSkippable() && this.testTextTyper.IsTyping)
            {
                //this.testTextTyper.Skip();
            }
            else
            {
                ShowScript();
            }
        }

        private void HandlePrintNoSkipClicked()
        {
            ShowScript();
        }

        private void ShowScript()
        {
            if (dialogueLines.Count <= 0)
            {
                return;
            }

            this.testTextTyper.TypeText(dialogueLines.Dequeue());
        }

        private void LogTag(RichTextTag tag)
        {
            if (tag != null)
            {
                Debug.Log("Tag: " + tag.ToString());
            }
        }

        private void HandleCharacterPrinted(string printedCharacter)
        {
            // Do not play a sound for whitespace
            if (printedCharacter == " " || printedCharacter == "\n")
            {
                return;
            }

            var audioSource = this.GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = this.gameObject.AddComponent<AudioSource>();
            }

            audioSource.clip = this.printSoundEffect;
            audioSource.Play();
        }

        private void HandlePrintCompleted()
        {
            Debug.Log("TypeText Complete");
        }
    }
}