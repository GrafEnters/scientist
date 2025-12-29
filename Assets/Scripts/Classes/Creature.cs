using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Creature : Thing {
    public NavMeshAgent agent;

    private void Start() {
        agent.Move(new Vector3(0, 50, 0));
    }
}