using Unity.Netcode;
using UnityEngine;

public class GrabbableCreator : NetworkBehaviour
{
    [SerializeField] private NetworkObject[] prefabs;
    [SerializeField] private Vector2 placementArea = new Vector2(-10, 10);

    [SerializeField] private Vector2 prefabSizes = new Vector2(0.01f, 2);

    [SerializeField] private int maxObjectsToSpawn;

    // Start is called before the first frame update
    void Start()
    {
        if (IsServer || IsHost)
        {
            for (int i = 0; i < maxObjectsToSpawn; i++)
            {
                NetworkObject no = Instantiate(prefabs[Random.Range(0, prefabs.Length)], Vector3.zero, Quaternion.identity);
                
                no.transform.position = new Vector3(Random.Range(placementArea.x, placementArea.y), 5, Random.Range(placementArea.x, placementArea.y));

                float randomSize = Random.Range(prefabSizes.x, prefabSizes.y);
                no.transform.localScale = new Vector3(randomSize, randomSize, randomSize);
                no.Spawn();
            }
        }
    }
}