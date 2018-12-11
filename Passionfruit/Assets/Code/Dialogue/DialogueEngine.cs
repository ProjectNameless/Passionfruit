using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueEngine : MonoBehaviour {
    private Text Captions;
    private GameObject TextBox;
    private Text ReputationText;
    private Slider ReputationScaler;
    public GameObject OptionPrefab;
    public Transform OptionsContainer;
    public List<GameObject> options = new List<GameObject>();
    private AudioSource lastCall;
    public int reputation = 0;
    #region singleton
    public static DialogueEngine instance;
    private void Awake()
    {
        if (instance == null || instance == this)
            instance = this;
        else
            Destroy(this);
    }
    #endregion
    // Use this for initialization
    void Start () {
        Captions = GameObject.FindGameObjectWithTag("Captions").GetComponent<Text>();
        TextBox = GameObject.FindGameObjectWithTag("Text Box");
        ReputationText = GameObject.FindGameObjectWithTag("Reputation Text").GetComponent<Text>();
        ReputationScaler = GameObject.FindGameObjectWithTag("Reputation Scaler").GetComponent<Slider>();
        ChangeReputation(10);
	}
    /// <summary>
    /// displays whole string within time specified. letters per second = text.length/time
    /// </summary>
    /// <param name="text"></param>
    /// <param name="time"></param>
	public void StartDisplayTextInTime(DialogueSO[] dialogues, GameObject caller)
    {
        if (lastCall != null)
        {
            lastCall.Pause();
            lastCall = null;
        }
        StopAllCoroutines();
        clearOptions();
        StartCoroutine(DisplayTextInTime(dialogues, caller));
    }
    /// <summary>
    /// Change by amount the players reputation, and adjust sliders to the current value.
    /// </summary>
    /// <param name="amount"></param>
    public void ChangeReputation(int amount)
    {
        reputation += amount;
        ReputationScaler.value = reputation;
        ReputationText.text = "Reputation: " + reputation;
    }
    /// <summary>
    /// the coroutine that runs along side the update function to allow pausing and checking things mid frame.
    /// </summary>
    /// <param name="dialogues"></param>
    /// <param name="caller"></param>
    /// <returns></returns>
    private IEnumerator DisplayTextInTime(DialogueSO[] dialogues, GameObject caller)
    {
        foreach (DialogueSO currentDialogue in dialogues)
        {
            DialogueSO dialogue = currentDialogue;
            while(dialogue.minimumRepValue > reputation)
            {
                if (dialogue.incorrectReputationResponse != null)
                    dialogue = dialogue.incorrectReputationResponse;
                else
                    break;
            }
            TextBox.SetActive(true);
            if (dialogue is DialogueWithAudioSO)
            {
                AudioSource audioSource;
                DialogueWithAudioSO dialogueWithAudio = ((DialogueWithAudioSO)dialogue);
                audioSource = caller.GetComponent<AudioSource>();
                AudioClip soundClip = dialogueWithAudio.voice;
                audioSource.clip = soundClip;
                audioSource.Play();
                lastCall = audioSource;
                float lettersPerSecond = (dialogue.text.Length - 1) / soundClip.length * dialogueWithAudio.time;
                int letterIndex = 0;
                Captions.text = "";
                while ((Captions.text.Length < dialogue.text.Length) && (!Input.GetKeyDown(KeyCode.Return)))
                {
                    Captions.text += dialogue.text[letterIndex];
                    letterIndex++;
                    yield return new WaitForSeconds(1 / lettersPerSecond);
                }
                Captions.text = dialogue.text;
                while (audioSource.isPlaying)
                    yield return null;
            }
            else
            {
                float lettersPerSecond = (dialogue.text.Length - 1) / dialogue.time;
                int letterIndex = 0;
                Captions.text = "";
                while ((Captions.text.Length - 1 < dialogue.text.Length - 1) && (!Input.GetKeyDown(KeyCode.Return)))
                {
                    Captions.text += dialogue.text[letterIndex];
                    letterIndex++;
                    yield return new WaitForSeconds(1 / lettersPerSecond);
                }
                Captions.text = dialogue.text;
                float timeTillExpire = dialogue.time;
                yield return null;
                while (timeTillExpire > 0 && !Input.GetKeyDown(KeyCode.Return))
                {
                    yield return null;
                    timeTillExpire -= Time.deltaTime;
                }
            }
            if (dialogue.dialogueOptions != null)
            {
                foreach (DialogueOptionSO option in dialogue.dialogueOptions)
                {
                    GameObject newOptionGO = GameObject.Instantiate(OptionPrefab, OptionsContainer);
                    options.Add(newOptionGO);
                    newOptionGO.GetComponent<Button>().onClick.AddListener(() => option.OnSelect(caller));
                    newOptionGO.GetComponentInChildren<Text>().text = option.text;
                }
                while (true)
                {
                    yield return null;
                }
            }
            EventEngine.instance.Call(dialogue.nextEvent);
            Captions.text = "";
        }
        TextBox.SetActive(false);
    }
    public void clearOptions()
    {
        foreach (GameObject option in options)
        {
            Destroy(option);
        }
        options = new List<GameObject>();
    }
}
