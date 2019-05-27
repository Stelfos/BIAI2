using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class BIAIAgent : Agent {
    public GameObject target;

    BIAIAcademy academy;
    private Vector3 goal;
    private Rigidbody rb;

    private Vector3 startingAgentPos;

    public override void InitializeAgent()
    {
        academy = FindObjectOfType(typeof(BIAIAcademy)) as BIAIAcademy;
        goal = target.transform.position;
        rb = GetComponent<Rigidbody>();
        startingAgentPos = transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "agent")
        {
            Physics.IgnoreCollision(collision.collider, GetComponent<SphereCollider>());
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.CompareTag("goal"))
        {
            AddReward(100f);
            Done();
        }
    }

    public override void CollectObservations()
    {
        AddVectorObs(transform.position);
        AddVectorObs(rb.velocity.magnitude);
        AddVectorObs(goal - transform.position);
        AddVectorObs(Vector3.Distance(goal, transform.position));
    }

    public override void AgentAction(float[] vectorAction, string textAction)
	{
        //Movement
        Vector3 dirToGo = Vector3.zero;
        dirToGo.x = vectorAction[0];
        dirToGo.z = vectorAction[1];
        
        //Existential penalty
        AddReward(-1/agentParameters.maxStep);
        //Distance to goal reward
        AddReward(1 / Mathf.Pow((Vector3.Distance(goal, transform.position)),2));
        // Velocity penalty
        AddReward(Mathf.Max(Mathf.Log10(rb.velocity.magnitude) * 0.01f,-100f));

        /*if(rb.velocity.magnitude < 1f)
        {
            AddReward(-0.005f);
        }*/


        rb.AddForce(dirToGo * academy.agentRunSpeed, ForceMode.VelocityChange);
        //Fell off platform
        if (this.transform.position.y < 0)
        {
            AddReward(-100f);
            Done();
        }
    }


    public override void AgentReset()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = startingAgentPos;
        target.transform.position = goal;

    }
}
