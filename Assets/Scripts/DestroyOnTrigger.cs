using UnityEngine;

public class DestroyOnTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Check if the other object has a rigidbody attached (so we don't accidentally destroy a trigger)
        if (other.gameObject.GetComponent<Rigidbody>() != null)
        {
            // Destroy the other object
            Destroy(other.gameObject);
        }
    }
}
