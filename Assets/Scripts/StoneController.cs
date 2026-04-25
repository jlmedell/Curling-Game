using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class StoneController : MonoBehaviour
{
    public float rotationSpeed = 100f;
    public LineRenderer trajectory; //aim indictor line
    private Rigidbody body;
    private float force = 0f; //current force
    private float curl = 0.1f; //current curl force
    private bool charging = false;
    private bool launched = false;
    private GameManager gamemanager;
    private bool stopped = false;
    float stopTimer = 0f;

    public bool inHome = false;
    public float distanceFromCenter;

    private Slider bar; //power bar

    void Start()
    {
        force = 0f;
        launched = false;
        stopped = false;
        bar = GameObject.Find("PowerBar").GetComponent<Slider>();
        body = GetComponent<Rigidbody>();
        gamemanager = FindFirstObjectByType<GameManager>();
    }

    void Update()
    {
        Trajectory();

        if (launched)
        {
            trajectory.enabled = false;
            return;
        }
        Vector3 rotate = new Vector3(0, 0, -curl * 20f); //aim indicator rotation
        transform.GetChild(0).localRotation = Quaternion.Euler(rotate);

        Aim();
        Power();
        if (Keyboard.current.leftArrowKey.isPressed)
        {
            curl = curl - Time.deltaTime;
        }
        if (Keyboard.current.rightArrowKey.isPressed)
        {
            curl = curl + Time.deltaTime;
        }
        curl = Mathf.Clamp(curl, -2f, 2f); //2 = max curl force
    }

    void FixedUpdate()
    {
        if (launched)
        {
            Curl(); 
        }

        IfStop(); //check if stone has stopped
    }

    void Aim()
    {
        float input = 0f;
        if (Keyboard.current.aKey.isPressed)
        {
           input = -1f;
        }
        if (Keyboard.current.dKey.isPressed)
        {
            input = 1f;
        }
        transform.Rotate(Vector3.up, input * rotationSpeed * Time.deltaTime);
    }

    void Power()
    {
        if (Keyboard.current.spaceKey.isPressed)
        {
            charging = true;
            force = force + 500f * Time.deltaTime;
            force = Mathf.Clamp(force, 0, 1000f); //1000 = max force
        }

        if (Keyboard.current.spaceKey.wasReleasedThisFrame && charging)
        {
            Launch();
        }
        bar.value = force / 1000f;
    }

    void Launch()
    {
        body.AddForce(transform.forward * (force * Time.fixedDeltaTime), ForceMode.Impulse); //impulse scaling for forward force
        body.AddTorque(Vector3.up * curl, ForceMode.Impulse);

        launched = true;
    }

    void Curl()
    {
        float speed = body.linearVelocity.magnitude;
        if (speed < 0.2f)
        {
            return;
        }
        Vector3 sideways = Vector3.Cross(body.linearVelocity.normalized, Vector3.up); //sideways force for curl
        body.AddForce(sideways * (curl * speed * 0.3f));
        Debug.DrawRay(transform.position, sideways * curl * 2f, Color.blue);
    }

    void Trajectory()
    {
        if (launched)
        {
            trajectory.enabled = false;
            return;
        }
        trajectory.enabled = true;
        Vector3[] points = new Vector3[80];
        Vector3 position = transform.position; //expected position
        Vector3 ev = transform.forward * (force * 0.01f); //expected velocity
        for (int i = 0; i < 80; i++)
        {
            float speed = ev.magnitude;
            if (speed > 0.01f)
            {
                Vector3 sideways = Vector3.Cross(ev.normalized, Vector3.up);
                ev = ev + sideways * (curl * speed * 0.3f) * 0.05f;
            }
            ev = ev * Mathf.Clamp01(1f - body.linearDamping * 0.05f);
            position = position + ev * 0.05f;
            points[i] = position;
        }
        trajectory.positionCount = 80;
        trajectory.SetPositions(points);
    }

    void IfStop()
    {
        if (!launched || stopped)
        {
            return;
        }
        float speed = body.linearVelocity.magnitude;
        if (speed < 0.3f)
        {
            stopTimer = stopTimer + Time.fixedDeltaTime;
            if (stopTimer >= 0.5f)
            {
                stopped = true;
                //gradual stop
                body.linearDamping = 10f;
                body.angularDamping = 10f;
                body.Sleep();
                gamemanager.Stopped();
            }
        }
        else
        {
            stopTimer = 0f;
        }
    }
}