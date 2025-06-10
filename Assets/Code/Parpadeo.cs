using UnityEngine;

public class LightFlicker : MonoBehaviour 
{
    [Header("Configuración")]
    public float minIntensity = 0.5f;
    public float maxIntensity = 2.0f;
    public float flickerSpeed = 0.1f;
    
    private Light lightComponent;
    
    void Start() 
    {
        lightComponent = GetComponent<Light>();
        InvokeRepeating("Flicker", 0, flickerSpeed);
    }
    
    void Flicker() 
    {
        // Cambiar intensidad aleatoriamente
        lightComponent.intensity = Random.Range(minIntensity, maxIntensity);
        
        // 5% probabilidad de apagarse por un momento
        if (Random.Range(0, 100) < 5) 
        {
            lightComponent.enabled = false;
            Invoke("TurnOn", Random.Range(0.1f, 1.0f));
        }
    }
    
    void TurnOn() 
    {
        lightComponent.enabled = true;
    }
}