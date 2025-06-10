using UnityEngine;

public class LightSwitch : MonoBehaviour 
{
    [Header("Configuración")]
    public Light lightToControl;  // La luz que queremos controlar
    public Camera playerCamera;   // Cámara a usar (arrastra aquí tu cámara)
    public float maxDistance = 5f; // Distancia máxima para activar
    
    void Start() 
    {
        // Si no asignaste una cámara, usar la cámara principal
        if (playerCamera == null)
            playerCamera = Camera.main;
        
        // Si no asignaste la luz, buscar una en el mismo objeto
        if (lightToControl == null)
            lightToControl = GetComponent<Light>();
    }
    
    void Update() 
    {
        // Detectar clic izquierdo del mouse
        if (Input.GetMouseButtonDown(0)) 
        {
            CheckForSwitch();
        }
    }
    
    void CheckForSwitch() 
    {
        if (playerCamera == null) 
        {
            return;
        }
        
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, maxDistance)) 
        {
            if (hit.collider.gameObject == gameObject) 
            {
                ToggleLight();
            }
        }
    }
    
    void ToggleLight() 
    {
        if (lightToControl != null) 
        {
            lightToControl.enabled = !lightToControl.enabled;
        }
    }
    
    // Para mostrar el rango en el editor
    void OnDrawGizmosSelected() 
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, Vector3.forward * maxDistance);
    }
}