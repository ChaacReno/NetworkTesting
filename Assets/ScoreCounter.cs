using TMPro;
using Unity.Netcode;
using UnityEngine;

public class ScoreCounter : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    private int score;

    [ServerRpc]
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Grabbable"))
        {
            text.text = score++.ToString();
        }
    }
}
