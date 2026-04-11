using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class StoneController : MonoBehaviour
{
    public float maxForce = 1000f;
    public float chargeSpeed = 500f;
    public float rotationSpeed = 100f;
    public float maxCurl = 2f;
    public LineRenderer trajectoryLine;
    public int simulationSteps = 80;
    public float timeStep = 0.05f;

    private Rigidbody rb;

    private float currentForce = 0f;
    private float currentCurl = 0.1f;

    private bool isCharging = false;
    private bool hasLaunched = false;
    private GameManager gameManager;
    private bool hasStopped = false;

    float stopTimer = 0f;
    public float stopThreshold = 0.08f;
    public float stopTimeRequired = 0.5f;

    private Slider powerBar;

    void Start()
    {
        currentForce = 0f;
        hasLaunched = false;
        hasStopped = false;
        powerBar = GameObject.Find("PowerBar").GetComponent<Slider>();
        rb = GetComponent<Rigidbody>();
        gameManager = FindFirstObjectByType<GameManager>();
    }
    
    void Update()
    {
        DrawTrajectory();
        if (hasLaunched)
        {
            trajectoryLine.enabled = false;
            ApplyCurl();
            return;
        }
        Vector3 indicatorRotation = new Vector3(0, 0, -currentCurl * 20f);
        transform.GetChild(0).localRotation = Quaternion.Euler(indicatorRotation);
        HandleAiming();
        HandlePower();
        HandleCurl();
    }

    void FixedUpdate()
    {
        CheckIfStopped();
    }

    void HandleAiming()
    {
        float rotateInput = 0f;

        if (Keyboard.current.aKey.isPressed)
            rotateInput = -1f;
        if (Keyboard.current.dKey.isPressed)
            rotateInput = 1f;

        transform.Rotate(Vector3.up, rotateInput * rotationSpeed * Time.deltaTime);
    }

    void HandlePower()
    {
        if (Keyboard.current.spaceKey.isPressed)
        {
            isCharging = true;
            currentForce += chargeSpeed * Time.deltaTime;
            currentForce = Mathf.Clamp(currentForce, 0, maxForce);
        }

        if (Keyboard.current.spaceKey.wasReleasedThisFrame && isCharging)
        {
            Launch();
        }
        powerBar.value = currentForce / maxForce;
    }

    void HandleCurl()
    {
        if (Keyboard.current.leftArrowKey.isPressed)
            currentCurl -= Time.deltaTime;

        if (Keyboard.current.rightArrowKey.isPressed)
            currentCurl += Time.deltaTime;

        currentCurl = Mathf.Clamp(currentCurl, -maxCurl, maxCurl);
    }

    void Launch()
    {
        rb.AddForce(transform.forward * currentForce);

        // apply spin 
        rb.AddTorque(Vector3.up * currentCurl, ForceMode.Impulse);

        hasLaunched = true;
    }

    void ApplyCurl()
    {
        float speed = rb.linearVelocity.magnitude;

        if (speed < 0.2f) return;
        if (speed > 0.1f) //stone is moving
        {
            Vector3 sideways = Vector3.Cross(rb.linearVelocity.normalized, Vector3.up); //get sideways direction

            float curlForce = currentCurl * speed * 0.3f;

            rb.AddForce(sideways * curlForce);
        }
        Debug.DrawRay(transform.position, transform.right * currentCurl * 2f, Color.blue);
    }

    void DrawTrajectory()
    {
        if (hasLaunched)
        {
            trajectoryLine.enabled = false;
            return;
        }

        trajectoryLine.enabled = true;

        Vector3[] points = new Vector3[simulationSteps];

        Vector3 simPosition = transform.position;

       
        Vector3 simVelocity = transform.forward * (currentForce * Time.fixedDeltaTime / rb.mass);

        for (int i = 0; i < simulationSteps; i++)
        {
            float speed = simVelocity.magnitude;

            // curl
            if (speed > 0.01f)
            {
                Vector3 sideways = Vector3.Cross(simVelocity.normalized, Vector3.up);

                float curlForce = currentCurl * speed * 0.3f;

                simVelocity += sideways * curlForce * timeStep;
            }

            //drag
            simVelocity *= Mathf.Clamp01(1f - rb.linearDamping * timeStep);

            simPosition += simVelocity * timeStep;

            points[i] = simPosition;
        }

        trajectoryLine.positionCount = simulationSteps;
        trajectoryLine.SetPositions(points);
    }

    void CheckIfStopped()
    {
        if (!hasLaunched || hasStopped) return;

        float speed = rb.linearVelocity.magnitude;
       
        if (speed < 0.3f) //change this number to hard stop faster
        {
            stopTimer += Time.fixedDeltaTime;

            if (stopTimer >= 0.5f)
            {
                hasStopped = true;

                // HARD STOP
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.Sleep(); // important

                gameManager.OnStoneStopped();
            }
        }
        else
        {
            stopTimer = 0f;
        }
    }
}