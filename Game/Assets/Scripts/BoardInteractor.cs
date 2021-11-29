using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DotsCore;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class BoardInteractor : MonoBehaviour
{
    public SocketBehaviour socketBehaviour;

    public Text currentTurnLabel; 
    
    public GameObject redDotPrefab;
    public GameObject blueDotPrefab;
    public GameObject redCapturePrefab;

    public Color redColor = new Color(255, 0, 0);
    public Color blueColor = new Color(0, 0, 255);
    
    [Range(0f, 1f)]
    public float captureOpacity = 0.57f;
    
    private BoardState _state;
    private SpriteRenderer _spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        
        var gridSize = _spriteRenderer.size / 0.64f;
        Debug.Log(gridSize);
        _state = new BoardState((int)gridSize.y, (int)gridSize.x);

        socketBehaviour.Connection.BoardStateUpdated += state =>
        {
            _state = state;
            SyncBoardState();
        };
        
        SyncBoardState();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            var cursorWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            cursorWorldPos.z = 0;

            var cursorLocalPos = transform.InverseTransformPoint(cursorWorldPos);

            var cellIdx = ((Vector2)cursorLocalPos + _spriteRenderer.size / 2) / 0.64f;

            int cellCol = Mathf.RoundToInt(cellIdx.x) - 1;
            int cellRow = Mathf.RoundToInt(cellIdx.y) - 1;

            if (_state.CanPlace(cellRow, cellCol))
            {
                Debug.Log($"Placing at row={cellRow} col={cellCol}");
                socketBehaviour.Connection.MakeMove(new Move {Player = _state.CurrentMove, Row = cellRow, Col = cellCol});                
                _state.Place(cellRow, cellCol);
                
                SyncBoardState();
            } 
        }
    }

    void SyncBoardState()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);                
        }
        for (int i = 0; i < _state.Rows; i++)
        {
            for (int j = 0; j < _state.Cols; j++)
            {
                var cell = _state.Get(i, j);
                if (cell.IsPlaced)
                {
                    var dotLocation = GetOnScreenLocation(i, j);
                    Instantiate(cell.Player == Player.Red ? redDotPrefab : blueDotPrefab, transform.TransformPoint(dotLocation), Quaternion.identity, transform);
                }
            }
        }
        
        foreach (var capture in _state.Captures)
        {
            var obj = Instantiate(redCapturePrefab, transform.position, Quaternion.identity, transform);
            var shape = obj.GetComponent<SpriteShapeController>();
            shape.spline.Clear();
            for (int i = 0; i < capture.Points.Points.Count; i++)
            {
                var (row, col) = capture.Points.Points[i];
                shape.spline.InsertPointAt(i, GetOnScreenLocation(row, col));
            }

            var color = capture.Player == Player.Red ? redColor : blueColor;
            color.a = captureOpacity;
            obj.GetComponent<SpriteShapeRenderer>().color = color;
        }

        currentTurnLabel.text = $"Current turn: {_state.CurrentMove.ToString()}";
    }

    private Vector2 GetOnScreenLocation(int row, int col)
    {
        return new Vector2(col + 1, row + 1) * 0.64f - _spriteRenderer.size / 2;
    }
}
