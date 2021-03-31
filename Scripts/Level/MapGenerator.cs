using System.Collections;
using System.Collections.Generic;
//using System.Diagnostics;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    public GameObject player; 
    public Tilemap layer0;
    public Tilemap layer0a;
    public Tilemap layer1;
    public Tilemap layer2;
    public Tilemap layer2a;
    public TileBase grass;
    public TileBase flora;
    public TileBase wall;
    public TileBase wallTop;
    public TileBase obstacle;
    public double seed;
    public double threshold;
    public bool isSaved;

    //xscale and yscale response to how big the obstacles would be
    [Range(0,10)]
    public float yscale;
    [Range(0,10)]
    public float xscale;

    public int[,] cavePoints;

    public int width = 45;

    public int height = 30;

    private int smoothCycles;

    private TilemapRenderer wallTopTile;

    //works when set to 4 because there are usually 3 neighbors max.
    private int minNeighbors = 4;

    //determine where to spawn the player/enemy
    public float tileSize = 0.32f;
    public float tileOffset = 0.16f;
    private int wallThresholdSize = 10;
    private int deadZoneArea ;
    int floraMin = 40;
    int floraMax = 80;
    int obstacleMin = 10;
    int obstacleMax = 30;
    int obstacleCap = 50;
    int floraCap = 100;

    private bool generationDone = false;
    
    private void Awake() 
    {
        width = 45;
        height = 30;
        deadZoneArea = Mathf.RoundToInt(width * height / 3);
        generationDone = false;
    }
    /*//All possible generation result
    public void allGeneration()
    {   
        xscale = Random.Range(0,10f);
        yscale = Random.Range(0,10f);
        threshold = Random.Range(0.0f,0.5f);
        smoothCycles = Random.Range(0,5);
    }
    public void forrestGeneration()
    {
        xscale = 4;
        yscale = 4;
        threshold = 0.38f;
        smoothCycles = 1;
    }
    public void mazeGeneration()
    {
        xscale = 3;
        yscale = 3;
        threshold = 0.35f;
        smoothCycles = 0;
    }
    public void mountainGeneration()
    {
        xscale = 10;
        yscale = 10;
        threshold = 0.3f;
        smoothCycles = Random.Range(1,4);
    }

    public void optimizedGeneration()
    {
        xscale = Random.Range(3f,4f);
        yscale = Random.Range(2f,3f);
        threshold = Random.Range(0.37f,0.42f);
        smoothCycles = Random.Range(1,3);
    }

    public void demoGeneration()
    {
        xscale = 5;
        yscale = 5;
        threshold = 0.4f;
    }*/

    
    public void bossGeneration(float difficulty)
    {
        float minXScale = width / 5f;
        float minYScale = height / 5f; ;
        float maxXScale = width / 3f;
        float maxYScale = height / 3f;
        float scaleStep = 0.05f;
        
        float maxThreshold = 0.32f;
        float minThreshold = 0.22f;
        float thresholdStep = 0.001f;
        float maxDifficulty = 100f;
        if (difficulty < 0)
        {
            UnityEngine.Debug.Log("Difficulty is negative.");           
        }
        else if (difficulty > maxDifficulty)
        {
            xscale = maxXScale;
            yscale = maxYScale;
            threshold = minThreshold;
        }
        else
        {
            xscale = minXScale + difficulty * scaleStep;
            yscale = minYScale + difficulty * scaleStep; 
            threshold = maxThreshold - difficulty * thresholdStep;
        }
        UnityEngine.Debug.Log("Boss level: " + difficulty);
    }
    
    public void normalGeneration(float difficulty)
    {
        float minXScale = width / 10f;
        float minYScale = height / 10f; 
        float maxXScale = width / 5f;
        float maxYScale = height / 5f; 
        float scaleStep = 0.05f;

        float maxThreshold = 0.40f;
        float minThreshold = 0.30f;
        float thresholdStep = 0.001f;

        float maxDifficulty = 100f;
        if (difficulty < 0)
        {
            UnityEngine.Debug.Log("Difficulty is negative");           
        }
        else if (difficulty > maxDifficulty)
        {
            xscale = maxXScale;
            yscale = maxYScale;
            threshold = minThreshold;
        }
        else
        {
            xscale = minXScale + difficulty * scaleStep;
            yscale = minYScale + difficulty * scaleStep;  
            threshold = maxThreshold - difficulty * thresholdStep; 
        }

        UnityEngine.Debug.Log("Normal level: " + difficulty);
    }

    public void generateNewGrid()
    {
        ClearAllTiles();
        cavePoints = new int[width, height];
        smoothCycles = 2;

        if(isSaved == false)
        {
            seed = Random.Range(0, 200f);
        }
        else
        {
            isSaved = false;
        }
        
        //normalGeneration(1); 

        UnityEngine.Debug.Log("xscale: "+ xscale+", yscale: "+ yscale+ ", threshold: " + threshold);

        for (int y = 0; y < height; ++y)
        {
            for (int x = 0; x < width; ++x)
            {
                if (x <= 1 || y <= 1 || x >= width - 2 || y >= height - 2)
                    {
                    cavePoints[x, y] = 1;
                    }
                    else
                    {
                        float sample = Mathf.PerlinNoise(x / xscale + (float)seed, y / yscale + (float)seed);                                 
                        if (sample > threshold)
                        {
                            cavePoints[x,y] = 0;
                        } 
                        else
                        {
                            cavePoints[x,y] = 1;
                        }  
                    }                    
                
            }
        }

		UnityEngine.Debug.Log("Smoothcycle: " + smoothCycles );
        for (int i = 0; i < smoothCycles; i++)
            {
                //UnityEngine.Debug.Log("Smoothcycle: " + smoothCycles );
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        int neighboringWalls = GetNeighbors(x, y);

                        if (neighboringWalls > minNeighbors)
                        {
                            cavePoints[x, y] = 1;
                        }else if (neighboringWalls < minNeighbors)
                        {
                            cavePoints[x, y] = 0;
                        }
                    }
                }
            }  
        ProcessMap();
        UnityEngine.Debug.Log("before generateTile()");
        generateTile(); 
        UnityEngine.Debug.Log("after generateTile()");
        generationDone = true;  
    }

    public void Generate(double seedValue, double thresholdValue, float xValue, float yValue, float dValue)
    {
        seed = seedValue;
        threshold = thresholdValue;
        xscale = xValue;
        yscale = yValue;
        isSaved = true;
        
    }

    public List<List<Coord>> GetRegions(int tileType) {
            List<List<Coord>> regions = new List<List<Coord>>();
            int[,] mapFlags = new int[width,height];

            for (int x = 0; x < width; x ++) {
                for (int y = 0; y < height; y ++) {
                    if ((mapFlags[x,y] == 0) && (cavePoints[x,y] == tileType)) 
                    {
                        UnityEngine.Debug.Log("region at " + x + "," + y);
                        List<Coord> newRegion = GetRegionTiles(x,y);
                        regions.Add(newRegion);

                        foreach (Coord tile in newRegion) {
                            mapFlags[tile.tileX, tile.tileY] = 1;
                        }
                    }
                }
            }
            if (regions == null)
            {
                UnityEngine.Debug.Log("Unable to find tileType: " + tileType);
            } 
            return regions;
        }

    public List<Coord> GetRegionTiles(int startX, int startY) {
            List<Coord> tiles = new List<Coord> ();
            int[,] mapFlags = new int[width,height];
            int tileType = cavePoints [startX, startY];

            Queue<Coord> queue = new Queue<Coord> ();
            queue.Enqueue (new Coord (startX, startY));
            mapFlags [startX, startY] = 1;

            UnityEngine.Debug.Log("start get regiontiles");
            while (queue.Count > 0) {
                Coord tile = queue.Dequeue();
                tiles.Add(tile);

                for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++) {
                    for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++) {
                        if (IsInMapRange(x,y) && (y == tile.tileY || x == tile.tileX)) {
                            if (mapFlags[x,y] == 0 && cavePoints[x,y] == tileType) {
                                mapFlags[x,y] = 1;
                                queue.Enqueue(new Coord(x,y));
                            }
                        }
                    }
                }
            }
        UnityEngine.Debug.Log("end get regiontiles");


        return tiles;
        }

    public bool IsInMapRange(int x, int y) {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    public void ProcessMap() {
        List<List<Coord>> wallRegions = GetRegions (1);
        foreach (List<Coord> wallRegion in wallRegions) {
            if (wallRegion.Count < wallThresholdSize) {
                foreach (Coord tile in wallRegion) {
                    cavePoints[tile.tileX,tile.tileY] = 0;
                }
            }
        }
        List<List<Coord>> roomRegions = GetRegions (0);       
        foreach (List<Coord> roomRegion in roomRegions) 
        {            
            if (roomRegion.Count < deadZoneArea) 
            {
                foreach (Coord tile in roomRegion) 
                {                    
                    cavePoints[tile.tileX,tile.tileY] = 1;
                }
               
            }            
        }
    }


    public void generateTile()
    {        
        //Generating tiles
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if(cavePoints[x,y] == 0)
                {
                    setFloor(x,y);                    
                }
                if (cavePoints[x,y] == 1)
                {

                    setTopWall(x,y);
                }
            }
        }

        UnityEngine.Debug.Log("flora...");
        //Generating flora and obstacle
        //starting count for flora and obstacle
        int floraCount = Random.Range(floraMin, floraMax);
        int obstacleCount = Random.Range(obstacleMin, obstacleMax);
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
 
                // this was a while because you wanted ...
                // a random for loop.  i opted to not eliminate
                // the 2d grid traversal just in case this while
                // was causing the infinite loop.
                if (floraCount <= floraCap)
                {
                    UnityEngine.Debug.Log("floraCount: " + floraCount);
                    int randX = Random.Range(2,width - 2);
                    int randY = Random.Range(2,height - 2);        
                    if (cavePoints[randX,randY] == 0)
                    {
                        setFlora(randX,randY);
                        floraCount++;
                        
                    }
                    //just for safety.
                    if(floraCount > floraCap)
                    {
                        break;
                    }
                }
                UnityEngine.Debug.Log("escaped flora loop");
                /* Turn off for Demo.
                while (obstacleCount <= obstacleCap)
                {
                    int randX = Random.Range(2,width - 2);
                    int randY = Random.Range(2,height - 2);
                    if (cavePoints[randX,randY] == 0)
                    {
                        setObstacle(randX,randY);
                        obstacleCount++;
                    }

                    //just for safety.
                    if (obstacleCount > obstacleCap)
                    {
                        break;
                    }
                }
                */
                
            }
        }
        //UnityEngine.Debug.Log("floraMin: "+ floraMin + ", obstacleMin: " + obstacleMin);
        UnityEngine.Debug.Log("floraCount: " + floraCount + ", obstacleCount: "
        + obstacleCount);
    }
    public void setFloor(int x, int y)
    {
        layer0.SetTile(new Vector3Int(x, y, 0), grass);           
    }
    public void setWall(int x,int y)
    {
        layer0.SetTile(new Vector3Int(x, y, 0), grass);  
        layer1.SetTile(new Vector3Int(x, y, 0), wall);
        
    }
    public void setTopWall(int x,int y)
    {
        layer0.SetTile(new Vector3Int(x, y, 0), grass);  
        layer1.SetTile(new Vector3Int(x, y - 1, 0), wall);
        layer2.SetTile(new Vector3Int(x, y, 0), wallTop);
        
    }
    public void setFlora(int x, int y)
    {
        layer0a.SetTile(new Vector3Int(x, y, 0), flora);         
    }
    public void setObstacle(int x, int y)
    {
        layer2a.SetTile(new Vector3Int(x, y, 0), obstacle);    
    }
   
    public int GetNeighbors(int pointX, int pointY)
    {

        int wallNeighbors = 0;

        for (int x = pointX - 1; x <= pointX + 1; x++)
        {
            for (int y = pointY - 1; y <= pointY + 1; y++)
            {
                if (x >= 0 && x < width && y >= 0 && y < height)
                {
                    if (x != pointX || y!= pointY)
                    {
                        if (cavePoints[x,y] == 1)
                        {
                            wallNeighbors++;
                        }
                    }
                }
                else
                {
                    wallNeighbors++;
                }
            }
        }

        return wallNeighbors;
    }
    public bool canSpawn()
    {
        return generationDone;
    }
    public void ClearAllTiles()
    {
        layer0.ClearAllTiles();
        layer0a.ClearAllTiles();
        layer1.ClearAllTiles();
        layer2.ClearAllTiles();
        layer2a.ClearAllTiles();
    }

    public struct Coord 
    {
        public int tileX;
        public int tileY;

        public Coord(int x, int y){
            tileX = x;
            tileY = y;
        }
    }
}
