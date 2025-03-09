using UnityEngine;
using System.Collections.Generic;
using System;

public class PathFinderManager : MonoBehaviour
{
    Queue<PathRequest> PathRequestQueue = new Queue<PathRequest>();
    PathRequest currentPathRequest;
    PathFinding pathfinding;

    bool isProcessingPath;

    static PathFinderManager instance;
    struct PathRequest
    {
        public Vector2 PathStart;
        public Vector2 PathEnd;
        public Action<Vector2[], bool> callback;

        public PathRequest(Vector2 start,Vector2 end, Action<Vector2[],bool> callback)
        {
            PathStart = start;
            PathEnd = end;
            this.callback = callback;
        }
    }
    private void Awake()
    {
        instance = this;
        pathfinding = GetComponent<PathFinding>();
    }
    public static void RequestPath(Vector2 start,Vector2 end, Action<Vector2[],bool> callback)
    {
        PathRequest pathRequest = new PathRequest(start,end,callback);
        instance.PathRequestQueue.Enqueue(pathRequest);
        instance.TryProcessNext();
    }

    private void TryProcessNext()
    {
        if(!isProcessingPath && PathRequestQueue.Count > 0)
        {
            currentPathRequest = PathRequestQueue.Dequeue();
            isProcessingPath = true;
            pathfinding.StartFindPath(currentPathRequest.PathStart,currentPathRequest.PathEnd);
        }
    }
    public void FinishedProcessingPath(Vector2[] path,bool success)
    {
        currentPathRequest.callback?.Invoke(path, success);
        isProcessingPath=false;
        instance.TryProcessNext();
    }
}
