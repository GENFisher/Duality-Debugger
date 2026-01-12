using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    private Vector3 offset = new Vector3(0, 0, -10);

    [Range(1,10)]
    public float smoothFactor;

    public Vector3 minValues, maxValues;

    public bool differentWorldLayout = false; // for different layouts in real and dark world
    public Vector3 bugminValues, bugmaxValues;

    private void FixedUpdate()
    {
        //Calling follow function
        Follow();
    }

    void Follow()
    {
        Vector3 boundPos;
        Vector3 targetPosition = target.position;

        //Clamp camera position within bounds based on world layout
        if (differentWorldLayout && !target.gameObject.GetComponent<PlayerController>().isReal)
        {
            boundPos = new Vector3(
            Mathf.Clamp(targetPosition.x, bugminValues.x, bugmaxValues.x),
            Mathf.Clamp(targetPosition.y, bugminValues.y, bugmaxValues.y),
            offset.z);
        }
        else
        {
            boundPos = new Vector3(
            Mathf.Clamp(targetPosition.x, minValues.x, maxValues.x),
            Mathf.Clamp(targetPosition.y, minValues.y, maxValues.y),
            offset.z);
        }

        //Smooth follow with Lerp
        Vector3 smoothPos = Vector3.Lerp(transform.position, boundPos, smoothFactor * Time.deltaTime);
        transform.position = smoothPos;
    }
}
