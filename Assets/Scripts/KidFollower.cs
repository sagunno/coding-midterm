using UnityEngine;

public class KidFollower : MonoBehaviour
{
    public float spacing { get; private set; }
    private float speed;
    private Transform leader;

    private Transform player;
    private float heightOffset;


    public void Setup(Transform leader, KidCapsuleData data)
    {
        this.leader = leader;
        spacing = data.followSpacing;
        speed = data.followSpeed;
        heightOffset = data.heightOffset;
    }

    public void SetLeader(Transform newLeader)
    {
        leader = newLeader;
    }

    //ughh constantly followsssssss smoothlyyy
    private void Update()
    {
        MatchPlayerHeight();

        if (leader == null) return;

        Vector3 toLeader = leader.position - transform.position;
        toLeader.y = 0f;
        float dist = toLeader.magnitude;

        if (dist > spacing)
        {
            Vector3 dir = toLeader / dist;
            transform.position += dir * Mathf.Min(speed * Time.deltaTime, dist - spacing);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 10f * Time.deltaTime);
        }
    }

    private void MatchPlayerHeight()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p == null) return;
            player = p.transform;
        }

        Vector3 pos = transform.position;
        pos.y = player.position.y + heightOffset;
        transform.position = pos;
    }
}