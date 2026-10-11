using System.Collections;
using UnityEngine;
using Yarn.Unity;


public class Kid_Capsule : MonoBehaviour
{
    public KidCapsuleData data;

    private NPC_Interact interact;
    private DialogueRunner dialogue;
    private bool rescued = false;
    
    private void Start()
    {
        //if already rescueded its gonna destroy itself!!! wow!~!!!!
        if (GameController.instance.followers.Contains(data)) { Destroy(gameObject); return; }

        interact = GetComponent<NPC_Interact>();
        dialogue = GameController.instance.dialogue;
        interact.startNode = data.startNode.Trim();
    }

    [YarnCommand("rescue_kid")]
    public void Rescue()
    {
        if (rescued) return;
        rescued = true;
        StartCoroutine(JoinAfterTalk());
    }

    private IEnumerator JoinAfterTalk()
    {
        while (interact.IsTalking) yield return null;

        //stops being intertactable in every way
        interact.interactText.gameObject.SetActive(false);
        interact.enabled = false;
        foreach (Collider c in GetComponentsInChildren<Collider>()) c.enabled = false;

        // become a follower that survives scene changes!!!
        transform.SetParent(null);  
        DontDestroyOnLoad(gameObject);

        KidFollower f = gameObject.AddComponent<KidFollower>();
        GameController.instance.AddFollower(data, f);
    }
}
