using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Row : MonoBehaviour
{
    public float BlockOffset;
    public GameObject BlockPrefab;

    private List<GameObject> _blocks = new List<GameObject>();

    public void AddBlock(DataModels.Stack block, UnityEvent<bool> inTestingChanged)
    {
        var blockObject = Instantiate(BlockPrefab, transform);
        var blockScript = blockObject.GetComponent<Block>();

        var oldPos = blockObject.transform.localPosition;
        blockObject.transform.localPosition = new Vector3(oldPos.x, oldPos.y, oldPos.z + (BlockOffset * _blocks.Count));

        blockScript.Init(block, inTestingChanged);

        _blocks.Add(blockObject);
    }
}
