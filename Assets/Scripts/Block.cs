using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Block : MonoBehaviour
{
    public Material[] Textures;
    public MeshRenderer Mesh;

    [Header("Details")]
    public GameObject Canvas;
    public TMPro.TMP_Text Title;
    public TMPro.TMP_Text Cluster;
    public TMPro.TMP_Text Content;

    private bool _inTesting = false;
    private bool _canBreak = false;

    public void Init(DataModels.Stack block, UnityEvent<bool> inTestingChanged)
    {
        Mesh.material = Textures[block.Mastery];

        Title.text = $"{block.Grade}: {block.Domain}";
        Cluster.text = block.Cluster;
        Content.text = $"{block.StandardId}: {block.StandardDescription}";

        if (block.Mastery == 0)
        {
            _canBreak = true;
        }

        inTestingChanged.AddListener((changed) =>
        {
            _inTesting = changed;
        });
    }

    private void OnMouseDown()
    {
        if (!_inTesting)
        {
            Canvas.SetActive(true);
        }
        else if (_canBreak)
        {
            Destroy(gameObject);
        }
    }

    private void OnMouseUp()
    {
        if (!_inTesting)
        {
            Canvas.SetActive(false);
        }
    }
}
