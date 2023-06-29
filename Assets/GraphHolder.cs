using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class GraphHolder : MonoBehaviour
{
    public PlayableGraph graph;

    // Start is called before the first frame update
    void Awake()
    {
        graph = PlayableGraph.Create();
    }

    // Update is called once per frame
    void OnDestroy()
    {
        graph.Destroy();
    }
}
