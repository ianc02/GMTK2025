using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Generation : MonoBehaviour
{
    // Start is called before the first frame update

    private Camera cam;
    public List<TileBase> BackgroundImages = new List<TileBase>();
    public List<TileBase> MainDungeonSprites = new List<TileBase>();
    public Tilemap Basemap;
    public Tilemap Mainmap;
    int mv = 0;
    Vector2 campos;
    Vector2 lastCamPos;
    void Start()
    {
        cam = Camera.main;
        campos = cam.transform.position;  
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GenerateNext(int step, bool rev)
    {
        int diff;
        if (rev)
        {
            cam.transform.position -= Vector3.right;
            diff = -7;
        }
        else
        {
            cam.transform.position += Vector3.right;
            diff = 6;
        }
         
        step = step % 7;
        Vector3Int cellPos = Basemap.WorldToCell(cam.transform.position);
        
        for (int i = -5; i < 5; i++)
        {
            
            
            Basemap.SetTile(new Vector3Int(cellPos.x+diff, cellPos.y + i,0), BackgroundImages[GetRandomBackTile()]);
        }
        if (step == 0)
        {
            Mainmap.SetTile(new Vector3Int(cellPos.x + diff, cellPos.y + 1, 0), MainDungeonSprites[0]);
            Mainmap.SetTile(new Vector3Int(cellPos.x + diff, cellPos.y , 0), MainDungeonSprites[1]);
            Mainmap.SetTile(new Vector3Int(cellPos.x + diff, cellPos.y -1, 0), MainDungeonSprites[2]);
            Mainmap.SetTile(new Vector3Int(cellPos.x + diff, cellPos.y -2, 0), MainDungeonSprites[3]);
        }
        if (step == 1)
        {
            Mainmap.SetTile(new Vector3Int(cellPos.x + diff, cellPos.y, 0), MainDungeonSprites[0]);
            Mainmap.SetTile(new Vector3Int(cellPos.x + diff, cellPos.y - 1, 0), MainDungeonSprites[3]);
        }
        if (step == 5)
        {
            Mainmap.SetTile(new Vector3Int(cellPos.x + diff, cellPos.y, 0), MainDungeonSprites[4]);
            Mainmap.SetTile(new Vector3Int(cellPos.x + diff, cellPos.y - 1, 0), MainDungeonSprites[5]);
        }
        if (step == 6)
        {
            Mainmap.SetTile(new Vector3Int(cellPos.x + diff, cellPos.y + 1, 0), MainDungeonSprites[4]);
            Mainmap.SetTile(new Vector3Int(cellPos.x + diff, cellPos.y, 0), MainDungeonSprites[6]);
            Mainmap.SetTile(new Vector3Int(cellPos.x + diff, cellPos.y - 1, 0), MainDungeonSprites[7]);
            Mainmap.SetTile(new Vector3Int(cellPos.x + diff, cellPos.y - 2, 0), MainDungeonSprites[5]);
        }
    }

    int GetRandomBackTile()
    {
        float odds = Random.value;
        if (odds < 0.1)
        {
            return Random.Range(1, BackgroundImages.Count);
        }
        return 0;
    }
}

