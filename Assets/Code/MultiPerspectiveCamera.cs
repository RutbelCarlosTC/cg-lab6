using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class MultiPerspectiveCamera : MonoBehaviour
{

    public bool tPerson = true;
    [Header("Objetivos de cámara")]
    public Transform tpTarget;
    public Transform fpTarget;

    [Header("Visibilidad de Jugador")]
    public bool disablePlayerMesh = true;

    [Space(2)]

    public GameObject playerMesh;
    [Space(5)]

    private Vector2 angle = new Vector2(90 * Mathf.Deg2Rad, 0);
    private new Camera camera;
    private Vector2 nearPlaneSize;
    private Transform follow;
    private float defaultDistance;
    private float newDistance;

    [Header("Ajustes de Cámara")]
    public float maxDistace = 7f;
    public float minDistance = 2f;

    [Space(6)]

    public int zoomVelocity = 300;
    public float zoomSmoth = 0.1f;
    public Vector2 sensitivity = new Vector2(1, 1);

    [Header("Tecla para cambiar perspectiva")]
    public KeyCode keyCode = KeyCode.Q;

    [Header("Transparencia de obstáculos")]
    public LayerMask transparentLayerMask;
    public float transparentAlpha = 0.3f;
    private List<Renderer> transparentObjects = new List<Renderer>();

    // Start is called before the first frame update
    void Start()
    {

        ChangePerspective(tPerson);


        defaultDistance = (maxDistace + minDistance) / 2;
        newDistance = defaultDistance;

        Cursor.lockState = CursorLockMode.Locked;
        camera = GetComponent<Camera>();

        CalculateNearPlaneSize();
    }
    void SetAlpha(Renderer rend, float alpha)
    {
        foreach (Material mat in rend.materials)
        {
            Color color = mat.color;
            color.a = alpha;
            mat.color = color;

            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.renderQueue = 3000;
        }
    }
    void RestoreTransparency()
    {
        foreach (Renderer rend in transparentObjects)
        {
            SetAlpha(rend, 1f);
        }
        transparentObjects.Clear();
    }
    void ChangePerspective(bool ThirdPerson)
    {
        if (ThirdPerson)
        {
            follow = tpTarget;

            if (disablePlayerMesh)
                playerMesh.SetActive(true);


            tPerson = true;
        }
        else if (!ThirdPerson)
        {
            follow = fpTarget;

            if (disablePlayerMesh)
                playerMesh.SetActive(false);


            tPerson = false;


        }

    }

    private void CalculateNearPlaneSize()
    {
        float height = Mathf.Tan(camera.fieldOfView * Mathf.Deg2Rad / 2) * camera.nearClipPlane;
        float width = height * camera.aspect;

        nearPlaneSize = new Vector2(width, height);
    }

    void Update()
    {
        float hor = Input.GetAxis("Mouse X");

        if (hor != 0)
        {
            angle.x += hor * Mathf.Deg2Rad * sensitivity.x;
        }

        float ver = Input.GetAxis("Mouse Y");

        if (ver != 0)
        {
            angle.y += ver * Mathf.Deg2Rad * sensitivity.y;
            angle.y = Mathf.Clamp(angle.y, -80 * Mathf.Deg2Rad, 80 * Mathf.Deg2Rad);
        }

        if (tPerson)
        {

            float scrollDelta = Input.GetAxis("Mouse ScrollWheel");

            if (scrollDelta > 0)
            {
                newDistance -= 0.1f * (Time.deltaTime * zoomVelocity);
            }
            else if (scrollDelta < 0)
            {
                newDistance += 0.1f * (Time.deltaTime * zoomVelocity);
            }
            newDistance = Mathf.Clamp(newDistance, minDistance, maxDistace);
            defaultDistance = Mathf.Lerp(defaultDistance, newDistance, zoomSmoth);
        }
        else if (!tPerson)
        {
            defaultDistance = 0.1f;


        }


        if (Input.GetKeyDown(keyCode))
        {
            if (tPerson)
                ChangePerspective(false);
            else
                ChangePerspective(true);
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 direction = new Vector3(
            Mathf.Cos(angle.x) * Mathf.Cos(angle.y),
            -Mathf.Sin(angle.y),
            -Mathf.Sin(angle.x) * Mathf.Cos(angle.y)
            );

        float distance = defaultDistance;

        transform.position = follow.position + direction * distance;
        transform.rotation = Quaternion.LookRotation(follow.position - transform.position);
        // Restaurar los objetos que fueron transparentados anteriormente
        RestoreTransparency();

        // Raycast desde la cámara hacia el jugador para detectar obstáculos
        Vector3 rayDir = follow.position - transform.position;
        float rayDist = Vector3.Distance(transform.position, follow.position);

        RaycastHit[] hits = Physics.RaycastAll(transform.position, rayDir, rayDist, transparentLayerMask);

        foreach (RaycastHit hit in hits)
        {
            Renderer rend = hit.collider.GetComponent<Renderer>();
            if (rend != null)
            {
                SetAlpha(rend, transparentAlpha);
                transparentObjects.Add(rend);
            }
        }
    }
}
