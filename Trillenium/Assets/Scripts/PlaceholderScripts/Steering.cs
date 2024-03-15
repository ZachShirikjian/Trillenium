using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    //SCRIPT IS FROM epicureanistik on Reddit:
    //https://www.reddit.com/r/Unity2D/comments/6ub6il/rpg_player_following_behaviour/
public class Steering : MonoBehaviour
{
    //REFERENCES//
    public Rigidbody2D sylvia;
    public int numFrames = 10;

    PlayerMovement player;
    Queue<Vector3> targetMovement;
    // Start is called before the first frame update
    void Start()
    {
        targetMovement = new Queue<Vector3>();
        player = sylvia.GetComponent<PlayerMovement>();
    }

    //If player moved during last frame, add its position to the queue.
    //If size of queue goes over # of frames follower is trailing behind,
    //Dequeue & set follower's position to obtained valuje 

    // Update is called once per frame
    void FixedUpdate()
    {
        if(sylvia.velocity.x != 0 && sylvia.velocity.y != 0)
        {
            targetMovement.Enqueue(sylvia.transform.position);
        }

        if(targetMovement.Count > numFrames)
        {
            transform.position = targetMovement.Dequeue();
        }
    }
}
