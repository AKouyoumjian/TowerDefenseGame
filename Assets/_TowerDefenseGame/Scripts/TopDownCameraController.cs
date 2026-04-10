using UnityEngine;


public class TopDownCameraController : MonoBehaviour
{
    public float panSpeed = 20f;
    public float screenPaddingThickness = 10f;
    public float scrollSpeed = 2000f;

    public Vector2 panLimitX = new Vector2(-20, 20);
    public Vector2 panLimitZ = new Vector2(-20, 25);
    public Vector2 zoomLimitY = new Vector2(5, 25);

    bool cameraControlEnabled = true;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // toggle camera control enabled boolean on escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            cameraControlEnabled = !cameraControlEnabled;
        }

        if (!cameraControlEnabled)
        {
            return;
        }

        Vector3 pos = transform.position;

        // forward
        if (Input.GetKey("w") || Input.mousePosition.y >= Screen.height - screenPaddingThickness)
        {
            pos.z += panSpeed * Time.deltaTime;
        }
        // backward
        if (Input.GetKey("s") || Input.mousePosition.y <= screenPaddingThickness)
        {
            pos.z -= panSpeed * Time.deltaTime;
        }
        // right
        if (Input.GetKey("d") || Input.mousePosition.x >= Screen.width - screenPaddingThickness)
        {
            pos.x += panSpeed * Time.deltaTime;
        }
        // left
        if (Input.GetKey("a") || Input.mousePosition.x <= screenPaddingThickness)
        {
            pos.x -= panSpeed * Time.deltaTime;
        }

        // scrolling to zoom in / out
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        pos.y -= scroll * Time.deltaTime * scrollSpeed;

        // clamp pos axes to prevent camera out of bounds
        pos.x = Mathf.Clamp(pos.x, panLimitX.x, panLimitX.y);
        pos.z = Mathf.Clamp(pos.z, panLimitZ.x, panLimitZ.y);
        pos.y = Mathf.Clamp(pos.y, zoomLimitY.x, zoomLimitY.y);

        transform.position = pos;
    }
}
