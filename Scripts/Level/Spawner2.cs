using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;
//using Unity.Mathematics;
using UnityEngine;
using Unity.Collections;

public class Spawner2 : MonoBehaviour
{
    [SerializeField] private Mesh unitMesh;
    [SerializeField] private Material unitMaterial;
    [SerializeField] private GameObject gameObjectPrefab;

    public GameObject player = null;
    public Grid mGrid;
    private MapGenerator currentLevel;
    private float spawnRadius = 20f;
    List<List<MapGenerator.Coord>> spawnPool;
    private Entity entityPrefab;
    private World defaultWorld;
    private EntityManager entityManager;
    private int ranX = 0;
    private int ranY = 0;
    private bool playerSpawned = false;

    void Start()
    {
        currentLevel = mGrid.GetComponent<MapGenerator>();

        StartCoroutine(SpawnPlayer()); 
        defaultWorld = World.DefaultGameObjectInjectionWorld;
        entityManager = defaultWorld.EntityManager;

        GameObjectConversionSettings settings = GameObjectConversionSettings.FromWorld(defaultWorld, null);
        entityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(gameObjectPrefab, settings);

        InstantiateEntity(0, 10);

    }
    private void InstantiateEntity(int TileType, int cap)
    {

        spawnPool = mGrid.GetComponent<MapGenerator>().GetRegions(TileType);  
      
        if (spawnPool == null)
        {
            Debug.Log("Unable to find spawnable tile for enemy!");
        } 
        int counter = 0;
        NativeArray<Entity> bubbleArray = new NativeArray<Entity>(cap, Allocator.Temp);
        while(cap > 0)
        { 
            ranX = Random.Range(2,currentLevel.width);
            ranY = Random.Range(2,currentLevel.height);
            if (currentLevel.cavePoints[ranX,ranY] == TileType)
            {
                Vector2 p = player.transform.position;    
                    if( (Mathf.Pow(ranX - p.x, 2f) + Mathf.Pow(ranY - p.y, 2f)) > Mathf.Pow(spawnRadius, 2f))
                    {

                        bubbleArray[counter] = entityManager.Instantiate(entityPrefab);
                        entityManager.SetComponentData(bubbleArray[counter], new Translation
                        {
                            Value = new Vector3(ranX, ranY, 0)
                        });
                        //GameObject e = Instantiate(stuff, Vector3.zero, stuff.transform.rotation) as GameObject;                
                        //e.transform.localPosition = new Vector3(ranX, ranY, 0);
                        counter++;
                        cap--;      
                    }
            }
        /*
        Entity myEntity = entityManager.Instantiate(entityPrefab);
        entityManager.SetComponentData(myEntity, new Translation
        {
            Value = position
        });
        */
    }
    bubbleArray.Dispose();
}

public IEnumerator SpawnPlayer()
    { 
        spawnPool = mGrid.GetComponent<MapGenerator>().GetRegions(0);
        if (spawnPool == null)
            {
                Debug.Log("Unable to find spawnable tile for player!");
            }
        while (playerSpawned == false)
        {           
            ranX = Random.Range(2,currentLevel.width);
            ranY = Random.Range(2,currentLevel.height);
            if (currentLevel.cavePoints[ranX,ranY] == 0)
            {             
                player.transform.localPosition = new Vector3(ranX, ranY, 0);
                playerSpawned = true;    
            }
        }
        yield return new WaitForSeconds(0.1f);
    }
}
