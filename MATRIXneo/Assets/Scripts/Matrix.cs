using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Matrix : MonoBehaviour
{
    public int height;
    public int width;
    public int[,] matrix;
    public Vector2Int startNeoPos;
    public Vector2Int neo;
    public List<Vector4> buildingsVectors;
    public List<Vector2Int> phones;
    public Cell cellPrefab;
    public Cell[,] cells;
    int queue = 0;

    // 0 - empty
    // 1 - neo
    // 2 - building
    // 3 - phones

    void Start ()
    {
        Init();
	}

    void Init()
    {
        matrix = new int[width, height];
        GenerateField();
        SetZeroMatrix();
        SetBuildings();
        SetNeoPos(startNeoPos.x, startNeoPos.y);
        SetPhones();        
    }

    void SetZeroMatrix()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                matrix[i, j] = 0;
                cells[i, j].text.text = string.Empty;
            }
        }
    }

    void SetNeoPos(int x, int y)
    {
        Vector2Int oldNeo = neo;
        neo.x = x;
        neo.y = y;
        matrix[neo.x, neo.y] = 1;
        cells[neo.x, neo.y].text.text = "N";
        cells[neo.x, neo.y].image.color = Color.blue;
        matrix[oldNeo.x, oldNeo.y] = 0;
    }

    void SetBuildings()
    {
        foreach (var build in buildingsVectors)
        {
            for (int i = (int)build.x; i < (int)build.z; i++)
            {
                for (int j = (int)build.y; j < (int)build.w; j++)
                {
                    matrix[i, j] = 2;
                    cells[i, j].text.text = "B";
                    cells[i, j].image.color = Color.red;
                }
            }
        }
    }

    void SetPhones()
    {
        for (int i = 0; i < phones.Count; i++)
        {
            matrix[phones[i].x, phones[i].y] = 3;
            cells[phones[i].x, phones[i].y].text.text = "P";
            cells[phones[i].x, phones[i].y].image.color = Color.green;
        }
    }

    void GenerateField()
    {
        cells = new Cell[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                var go = Instantiate(cellPrefab, cellPrefab.transform.parent);
                go.gameObject.SetActive(true);
                cells[i, j] = go;

                go.transform.localPosition += Vector3.down * ((go.transform as RectTransform).rect.height + 1) * j;
                go.index.x = i;
                go.index.y = j;                
            }
        }
        cellPrefab.gameObject.SetActive(false);
        MoveTowerToRight();
    }

    void MoveTowerToRight()
    {
        for (int i = 1; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                cells[i,j].transform.localPosition += Vector3.right * ((cells[i, j].transform as RectTransform).rect.width + 1) * i;
            }
        }
    }
}