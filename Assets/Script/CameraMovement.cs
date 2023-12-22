using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float sensitivity = 1.0f; // Sensibilidade do mouse
    public float maxYAngle = 80.0f; // Ângulo máximo de rotação para cima e para baixo
    [SerializeField] Transform orientation; //Direção do Player
    private Vector2 currentRotation = new Vector2(0, 0); //Rotação do Mouse
    

    void Start()
    {
        //Deixar o mouse travado e invisivel
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        transform.position = orientation.position; //Camera acompanha posição do Player
        
        // Captura os movimentos do mouse
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // Calcula a rotação com base na sensibilidade
        currentRotation.x += mouseX * sensitivity;
        currentRotation.y -= mouseY * sensitivity;

        // Limita o ângulo de rotação vertical
        currentRotation.y = Mathf.Clamp(currentRotation.y, -maxYAngle, maxYAngle);

        // Aplica a rotação à câmera
        transform.localRotation = Quaternion.Euler(0, currentRotation.x, 0);
        orientation.transform.rotation = Quaternion.Euler(0, currentRotation.x, 0);
    }
}
