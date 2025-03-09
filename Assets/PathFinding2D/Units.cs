using System.Collections;
using UnityEngine;

public class Units : MonoBehaviour
{
    [SerializeField] private Transform target;
    float speed = 20f;
    Vector2[] path;
    int targetindex;

    private void Start()
    {
        PathFinderManager.RequestPath(transform.position, target.position, OnPathFound);
    }
    public void OnPathFound(Vector2[] newPAth, bool ispathSuccesfull)
    {
        if (ispathSuccesfull)
        {
            path = newPAth;
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }
    IEnumerator FollowPath()
    {
        Vector2 currentPoint = path[0];

        while (true)
        {
            if ((Vector2)transform.position == currentPoint)
            {
                targetindex++;
            }
            if(targetindex >= path.Length)
            {
                yield break;
            }
            currentPoint = path[targetindex];
            transform.position = Vector2.MoveTowards(transform.position, currentPoint,speed*Time.deltaTime);
            yield return null;
        }        
    }
    public void OnDrawGizmos()
    {
        if (path != null)
        {
            for(int i = targetindex; i < path.Length; i++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(path[i], Vector3.one);
                if(i == targetindex)
                {
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else
                {
                    Gizmos.DrawLine(path[i - 1], path[i]);
                }
            }
        }
    }
}
