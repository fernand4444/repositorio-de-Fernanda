using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RollerPlayer : MonoBehaviour
{
    [Header("Movimento")]
    [SerializeField, Tooltip("Velocidade base (multiplica o torque aplicado)")]
    private float moveSpeed = 10f;
    [SerializeField, Tooltip("Multiplicador extra para o torque")]
    private float torqueMultiplier = 15f;
    [SerializeField, Tooltip("Máxima velocidade angular permitida (evita giro descontrolado)")]
    private float maxAngularVelocity = 50f;

    [Header("Input (Input Manager - padrão)")]
    [SerializeField, Tooltip("Nome do eixo horizontal (Input Manager)")]
    private string horizAxisName = "Horizontal";
    [SerializeField, Tooltip("Nome do eixo vertical (Input Manager)")]
    private string vertAxisName = "Vertical";

    [Header("Referências")]
    [SerializeField, Tooltip("Rigidbody da bola (será atribuído automaticamente se vazio)")]
    private Rigidbody rb;
    [SerializeField, Tooltip("Referência opcional da câmera. Se preenchida, o movimento será relativo à câmera")]
    private Transform cameraTransform;

    void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("RollerPlayer: Rigidbody não encontrado. Adicione um Rigidbody ao GameObject.");
            enabled = false;
            return;
        }

        // Define limite de velocidade angular no Rigidbody para evitar rotações muito rápidas
        rb.maxAngularVelocity = maxAngularVelocity;
    }

    void FixedUpdate()
    {
        // Leitura simples de input pelo Input Manager (WASD / setas por padrão)
        float h = Input.GetAxis(horizAxisName);
        float v = Input.GetAxis(vertAxisName);

        Vector3 input = new Vector3(h, 0f, v);

        if (input.sqrMagnitude < 0.0001f)
            return;

        Vector3 move = input;

        // Se houver câmera informada, converte input para espaço do mundo relativo à câmera
        if (cameraTransform != null)
        {
            Vector3 forward = cameraTransform.forward;
            forward.y = 0f;
            forward.Normalize();
            Vector3 right = cameraTransform.right;
            right.y = 0f;
            right.Normalize();
            move = forward * v + right * h;
        }

        // Calcula um torque que faz a bola rolar na direção do input
        Vector3 torque = Vector3.Cross(Vector3.up, move).normalized * move.magnitude * moveSpeed * torqueMultiplier;

        // Aplica torque no Rigidbody (modo Acceleration para ser independente da massa)
        rb.AddTorque(torque, ForceMode.Acceleration);

        // Limita a velocidade angular para o valor configurado
        if (rb.angularVelocity.sqrMagnitude > maxAngularVelocity * maxAngularVelocity)
        {
            rb.angularVelocity = rb.angularVelocity.normalized * maxAngularVelocity;
        }
    }
}

