using System.Net.NetworkInformation;
using UnityEngine;
using Yarn.Unity;

public class Rubble : MonoBehaviour
{
    private bool destroying = false;

    private void Update()
    {
        if (destroying || !GameController.instance.rubbleCleared) return;

        // wait for the dialogue to finish, then delete
        if (!GameController.instance.dialogue.IsDialogueRunning)
        {
            destroying = true;
            Destroy(gameObject);
        }
    }
}