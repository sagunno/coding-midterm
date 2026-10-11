using StarterAssets;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Yarn.Unity;

public class NPC_Interact : MonoBehaviour
{
    public TextMeshPro interactText; // game object text reference
    public string interaction; //sets interaction text
    public string startNode; //the title of the yarn node this npc starts on!! it all makes sense now!!!
    public DialogueRunner dialogue; //leave empty to auto-find the persistent one
    public bool IsTalking => talking;

    //polish slop
    public float fullVisibleDistance = 2f;
    public float invisibleDistance = 5f;

    //player turns during convo
    public Transform faceTarget;
    public float turnSpeed = 5f;

    private bool playerInRange = false;
    private bool talking = false;
    private ThirdPersonController player;

    private void OnDisable()
    {
        //when complted, we are freeeee
        if (dialogue != null) dialogue.onDialogueComplete.RemoveListener(EndConversation);
    }

    private void Start()
    {
        if (faceTarget == null) faceTarget = transform;
        dialogue = GameController.instance.dialogue;
        interactText.text = interaction;
        interactText.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        player = other.GetComponentInParent<ThirdPersonController>();
        playerInRange = true;
        interactText.gameObject.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = false;
        interactText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (talking)
        {
            TurnPlayerToTarget();
            return;
        }

        if (!playerInRange) return;

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            StartConversation();
        }

        // this is purely text tracking
        if (!playerInRange || talking) return;
        Vector3 away = interactText.transform.position - player.transform.position;
        away.y = 0f;

        //idk if im even doin this riht LOL
        if (away.sqrMagnitude > 0.0001f)
        {
            //3D text reads correctly when its forward points AWAY from the viewer
            interactText.transform.rotation = Quaternion.LookRotation(away);
        }

        //fade stuff
        interactText.alpha = Mathf.InverseLerp(invisibleDistance, fullVisibleDistance, away.magnitude);
    }

    private void StartConversation()
    {
        Debug.Log("Starting node: '" + startNode + "' on " + name); //bug fixing jesus hell


        talking = true;
        interactText.gameObject.SetActive(false);
        player.enabled = false; //locks the player in place!!!
        dialogue.onDialogueComplete.AddListener(EndConversation);
        dialogue.StartDialogue(startNode);
    }

    //fires when ANY conversation on the runner ends!!!!
    private void EndConversation()
    {
        if (!talking) return;

        talking = false;
        player.enabled = true;
        dialogue.onDialogueComplete.RemoveListener(EndConversation);
        if (playerInRange) interactText.gameObject.SetActive(true);
    }

    //lerps the player to face towards the interacted target...
    private void TurnPlayerToTarget()
    {
        Vector3 dir = faceTarget.position - player.transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f) return;

        Quaternion goal = Quaternion.LookRotation(dir);
        Transform camTarget = player.CinemachineCameraTarget.transform;
        Quaternion camRot = camTarget.rotation; //remember where the camera is pointing...

        player.transform.rotation = Quaternion.Slerp(player.transform.rotation, goal, turnSpeed * Time.deltaTime);

        camTarget.rotation = camRot; //put it back so the camera doesn't swing with the lerpp
    }
}