using Newtonsoft.Json;
using Proyecto26;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class StackManager : MonoBehaviour
{
    public GameObject LookAt;
    public float CameraSpeed;

    public float StackOffset;
    public GameObject StackPrefab;

    [Header("UI")]
    public GameObject UI;
    public GameObject GradeUIPrefab;
    public Transform GradeMenu;
    public Button TestMyStack;

    private Dictionary<string, Stack> _stacks;
    private int _lookingAt = 0;
    private bool _started = true;
    private Vector3 _mousePanStart = Vector3.zero;
    private bool _inTesting = false;
    private UnityEvent<bool> _inTestingChanged;

    private void Awake()
    {
        _getStacks();

        _inTestingChanged = new UnityEvent<bool>();

        TestMyStack.onClick.AddListener(() =>
        {
            _inTesting = !_inTesting;
            _inTestingChanged.Invoke(_inTesting);
        });
    }

    private void _getStacks()
    {
        RestClient.Get(new RequestHelper
        {
            Uri = "https://ga1vqcu3o1.execute-api.us-east-1.amazonaws.com/Assessment/stack",
            Retries = 2,
            RetrySecondsDelay = 1
        }).Then(response =>
        {
            var stacks = JsonConvert.DeserializeObject<List<DataModels.Stack>>(response.Text);

            _buildStacks(stacks);
        }).Catch(err =>
        {
            Debug.Log(err.Message);

            return;
        });
    }

    private void _buildStacks(List<DataModels.Stack> stacks)
    {
        _stacks = new Dictionary<string, Stack>();

        var orderedStacks = stacks
            .OrderBy(s => s.Grade)
            .ThenBy(s => s.Domain)
            .ThenBy(s => s.Cluster)
            .ThenBy(s => s.StandardId);

        foreach (var block in orderedStacks)
        {
            if (!_stacks.ContainsKey(block.Grade))
            {
                // UI
                var gradeUiObject = Instantiate(GradeUIPrefab, GradeMenu);
                var uiText = gradeUiObject.GetComponentInChildren<TMPro.TMP_Text>();
                uiText.text = block.Grade;
                var uiButton = gradeUiObject.GetComponent<Button>();
                uiButton.onClick.AddListener(() =>
                {
                    _lookAtStack(gradeUiObject.transform.GetSiblingIndex());
                });

                // CONTENT
                var stackObject = Instantiate(StackPrefab, transform);
                var stackScript = stackObject.GetComponent<Stack>();

                stackScript.Name.text = block.Grade;

                stackObject.transform.position = new Vector3(StackOffset * _stacks.Count, 0f, 0f);

                _stacks.Add(block.Grade, stackScript);
            }

            var stack = _stacks[block.Grade];

            stack.AddBlock(block, _inTestingChanged);
        }

        _lookAtStack(0);

        UI.SetActive(true);
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            if (_lookingAt + 1 < _stacks.Count)
            {
                _lookAtStack(_lookingAt + 1);
            }
            else
            {
                _lookAtStack(0);
            }
        }

        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            if (_lookingAt - 1 >= 0)
            {
                _lookAtStack(_lookingAt - 1);
            }
            else
            {
                _lookAtStack(_stacks.Count - 1);
            }
        }

        var mousePanState = Input.GetKey(KeyCode.Mouse1);
        if (mousePanState)
        {
            var move = Input.mousePosition;

            if (_started)
            {
                _mousePanStart = move;
                _started = false;
            }

            float rotation = move.x - _mousePanStart.x / CameraSpeed;
            var oldRotation = LookAt.transform.rotation.eulerAngles;

            LookAt.transform.rotation = Quaternion.Euler(0f, oldRotation.y + rotation, 0f);

            _mousePanStart = move;
        }
        else
        {
            _started = true;
        }
    }

    private void _lookAtStack(int index)
    {
        var stack = _stacks.ElementAt(index);
        _lookingAt = index;
        LookAt.transform.position = stack.Value.transform.position;
    }
}
