using UnityEngine;

[CreateAssetMenu(fileName = "BabyCapsule")]
public class KidCapsuleData : ScriptableObject
{
    public string kidName;
    public string startNode;       
    public float followSpacing = 1f;
    public float followSpeed = 5f;

    //follower shiii
    public float heightOffset = 1f;
}