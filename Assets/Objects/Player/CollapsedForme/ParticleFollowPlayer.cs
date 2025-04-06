using UnityEngine;

public class ParticleFollowPlayer : MonoBehaviour
{
    [SerializeField] private Transform objToFollow;

    // Update is called once per frame
    void Update()
    {
        transform.position = objToFollow.position;
    }
}
