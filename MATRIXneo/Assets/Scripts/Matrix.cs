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
    public List<Vector4> buildingsVectors;
    public List<Vector2Int> phones;
    public Cell cellPrefab;
    public Cell[,] cells;

    // -1 - empty
    // 0 - neo
    // -2 - building
    // -3 - phones

    void Start()
    {
        Init();
        WaveSearch(startNeoPos.x, startNeoPos.y);
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
                matrix[i, j] = -1;
                cells[i, j].text.text = string.Empty;
            }
        }
    }

    void SetNeoPos(int x, int y)
    {
        matrix[x, y] = 0;
        cells[x, y].text.text = "N";
        cells[x, y].image.color = Color.blue;
    }

    void SetBuildings()
    {
        foreach (var build in buildingsVectors)
        {
            for (int i = (int)build.x; i < (int)build.z; i++)
            {
                for (int j = (int)build.y; j < (int)build.w; j++)
                {
                    matrix[i, j] = -2;
                    cells[i, j].text.text = "B";
                    cells[i, j].image.color = Color.red;
                    cells[i, j].value = matrix[i, j];
                }
            }
        }
    }

    void SetPhones()
    {
        for (int i = 0; i < phones.Count; i++)
        {
            matrix[phones[i].x, phones[i].y] = -3;
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
                cells[i, j].transform.localPosition += Vector3.right * ((cells[i, j].transform as RectTransform).rect.width + 1) * i;
            }
        }
    }

    void WaveSearch(int startX, int startY)
    {
        int x, y, step = 0;
        bool found = false;

        float time = 0;
        do
        {
            for (int i = 0; i < width; i++)
            {
                if (found)
                    break;
                for (int j = 0; j < height; j++)
                {
                    if (found)
                        break;
                    if (matrix[i, j] == step)
                    {
                        if (i != width - 1)
                        {
                            if (matrix[i + 1, j] == -1)
                                matrix[i + 1, j] = step + 1;
                            else if (matrix[i + 1, j] == -3)
                            {
                                cells[i + 1, j].isFound = true;
                                found = true;
                            }
                        }
                        if (j != height - 1)
                        {
                            if (matrix[i, j + 1] == -1)
                                matrix[i, j + 1] = step + 1;
                            else if (matrix[i, j + 1] == -3)
                            {
                                cells[i, j + 1].isFound = true;
                                found = true;
                            }
                        }
                        if (i != 0)
                        {
                            if (matrix[i - 1, j] == -1)
                                matrix[i - 1, j] = step + 1;
                            else if (matrix[i - 1, j] == -3)
                            {
                                cells[i - 1, j].isFound = true;
                                found = true;
                            }
                        }
                        if (j != 0)
                        {
                            if (matrix[i, j - 1] == -1)
                                matrix[i, j - 1] = step + 1;
                            else if (matrix[i, j - 1] == -3)
                            {
                                cells[i, j - 1].isFound = true;
                                found = true;
                            }
                        }
                    }
                }
            }
            time += Time.deltaTime;
            step++;
        } while (!found && step < width * height);

        Debug.LogError("Wasted time = " + time);


        DrowSteps(startX, startY);
        DrawPath(step);
    }

    void DrowSteps(int startX, int startY)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                cells[x, y].value = matrix[x, y];
                if (matrix[x, y] == 0)
                {
                    cells[x, y].image.color = Color.white;
                    cells[x, y].text.text = string.Empty;
                }
                //else
                //if (matrix[x, y] == -2)
                //{
                //    cells[x, y].image.color = Color.red;
                //    cells[x, y].text.text = "B";
                //}
                //else
                if (x == startX && y == startY)
                {
                    cells[x, y].image.color = Color.blue;
                    cells[x, y].text.text = "N";
                }
                else
                if (matrix[x, y] == -3)
                {
                    cells[x, y].image.color = Color.yellow;
                    cells[x, y].text.text = "P";
                }
                else
                if (matrix[x, y] > 0)
                {
                    cells[x, y].image.color = Color.white;
                    cells[x, y].text.text = matrix[x, y].ToString();
                }
            }
        }
    }

    void DrawPath(int step)
    {
        int x;
        int y;
        Cell nearestPhone = null;
        for (int i = 0; i < width; i++)
        {
            if (nearestPhone != null)
                break;
            for (int j = 0; j < height; j++)
            {
                if (cells[i, j].isFound)
                {
                    nearestPhone = cells[i, j];                    
                    break;
                }
            }
        }
        x = nearestPhone.index.x;
        y = nearestPhone.index.y;

        do
        {
            if (x < width - 1)
                if (matrix[x + 1, y] == step - 1)
                {
                    cells[++x, y].image.color = Color.green;
                }
            if (y < height - 1)
                if (matrix[x, y + 1] == step - 1)
                {
                    cells[x, ++y].image.color = Color.green;
                }
            if (x > 0)
                if (matrix[x - 1, y] == step - 1)
                {
                    cells[--x, y].image.color = Color.green;
                }
            if (y > 0)
                if (matrix[x, y - 1] == step - 1)
                {
                    cells[x, --y].image.color = Color.green;
                }
            step--;
        } while (step != 0);
    }
}