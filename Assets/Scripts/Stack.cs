using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Stack : MonoBehaviour
{
    public float RowOffset;
    public GameObject RowPrefab;
    public TMPro.TMP_Text Name;

    private int _blockCount = 0;
    private List<Row> _rows = new List<Row>();

    public void AddBlock(DataModels.Stack block, UnityEvent<bool> inTestingChanged)
    {
        if (_blockCount % 3 == 0)
        {
            var rowObject = Instantiate(RowPrefab, this.transform);
            var rowScript = rowObject.GetComponent<Row>();

            if (_rows.Count % 2 == 0)
            {
                rowObject.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
            }
            rowObject.transform.localPosition = new Vector3(0f, RowOffset * _rows.Count, 0f);

            _rows.Add(rowScript);
        }

        var rowIndex = _blockCount / 3;
        var row = _rows[rowIndex];

        row.AddBlock(block, inTestingChanged);

        _blockCount++;
    }
}
