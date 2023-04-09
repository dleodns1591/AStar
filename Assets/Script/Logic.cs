using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Logic : MonoBehaviour
{
    Node[,] node;
    Node startNode;
    Node endNode;
    Node currentNode;

    public List<Node> open = new List<Node>();
    public List<Node> close = new List<Node>();

    public Vector2Int startLocation; // 시작위치
    public Vector2Int endLocation; // 도착위치

    public int sizeX = 0; // 맵 X크기
    public int sizeY = 0; // 맵 Y크기

    void Start()
    {
        Test();
    }

    void Update()
    {

    }

    public void Test()
    {
        CreatNode(sizeX, sizeY);
        SetTergetLocation(startLocation, endLocation);
        PathFinding();
    }

    // x,y 크기에 맞는 노드 배열
    public void CreatNode(int sizeX, int sizeY)
    {
        node = new Node[sizeX, sizeY];

        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                bool isWall = false; // 막힌 길 체크
                node[i, j] = new Node(isWall, i, j);
            }
        }
    }

    // 시작지점과 목표지점
    public void SetTergetLocation(Vector2Int start, Vector2Int end)
    {
        startNode = node[start.x, start.y];
        endNode = node[end.x, end.y];
    }

    // 길 찾기 알고리즘
    public void PathFinding()
    {
        open.Clear(); // 갈 수 있는 길 목록
        close.Clear(); // 한번 들른 목록

        // 시작 지점부터 시작
        open.Add(startNode);

        while (open.Count > 0)
        {
            // 갈 수 있는 길 중에 가중치가 낮은 길을 찾는다.
            currentNode = open[0];

            for (int i = 1; i < open.Count; i++)
            {
                if (open[i].f <= currentNode.f && open[i].h < currentNode.h)
                    currentNode = open[i];
            }

            // 찾은 길은 갈 수 있는 곳 목록에서 지운다.
            // 한 번 들른 곳 목록에 넣어준다.
            // 이로써 한 번 갔던 길을 중복으로 검색하는 일은 없어진다.
            open.Remove(currentNode);
            close.Add(currentNode);

            // 목적지에 도착했는지 체크한다.
            if (currentNode == endNode)
            {
                for (int i = 0; i < close.Count; i++)
                    Debug.Log(i + "번째는 " + close[i].x + ", " + close[i].y);
            }

            // 갈 수 있는 길을 찾아서 목록에 넣어준다.
            // 순서는 일반적으로 위, 오른쪽, 아래, 왼쪽 순이다.
            OpenListAdd(currentNode.x, currentNode.y + 1);
            OpenListAdd(currentNode.x + 1, currentNode.y);
            OpenListAdd(currentNode.x, currentNode.y - 1);
            OpenListAdd(currentNode.x - 1, currentNode.y);
        }
    }

    void OpenListAdd(int checkX, int checkY)
    {
        // 상하좌우 범위를 벗어났다면 빠져나온다.
        if (checkX < 0 || checkX >= node.GetLength(0) || checkY < 0 || checkY >= node.GetLength(1))
            return;

        // 한 번 들른 곳이면 빠져나온다.
        if (close.Contains(node[checkX, checkY]))
            return;

        Node neighborNode = node[checkX, checkY];
        int cost = currentNode.g + (currentNode.x - checkX == 0 || currentNode.y - checkY == 0 ? 10 : 14);

        if (cost < neighborNode.g || !open.Contains(neighborNode))
        {
            neighborNode.g = cost;
            neighborNode.h = (Mathf.Abs(neighborNode.x - endNode.x) + Mathf.Abs(neighborNode.y - endNode.y)) * 10;
            neighborNode.parentNode = currentNode;

            open.Add(neighborNode);
        }
    }

}
