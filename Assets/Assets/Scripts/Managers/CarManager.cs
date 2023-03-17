using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarManager : MonoBehaviour
{
    [SerializeField] private Transform map;
    [SerializeField] private GameObject[] _cars;
    
    private enum Pos
    {
        LEFT,
        RIGHT,
        TORIGHT,
        TOLEFT
    }

    private readonly Dictionary<Pos, string> _animVal = new();

    private float _toLeftTunnelTimer = 0;
    private float _toRightTunnelTimer = 0;
    private float _leftThroughTimer = 0;
    private float _rightThroughTimer = 0;

    private float _rand1 = 0;
    private float _rand2 = 0;

    void Start()
    {
        _animVal.Add(Pos.LEFT, "LeftTunnel");
        _animVal.Add(Pos.RIGHT, "RightTunnel");
        _animVal.Add(Pos.TOLEFT, "UpperToLeft");
        _animVal.Add(Pos.TORIGHT, "UpperToRight");

        SetRandomVars();
    }

    void Update()
    {
        _toLeftTunnelTimer += Time.deltaTime;
        _toRightTunnelTimer += Time.deltaTime;
        _leftThroughTimer += Time.deltaTime;
        _rightThroughTimer += Time.deltaTime;

        if (_toLeftTunnelTimer > _rand1)
        {
            PlayCarAt(Pos.TOLEFT);
            SetRandomVars();
            _toLeftTunnelTimer = 0;
        }
        else if (_toRightTunnelTimer > _rand1)
        {
            PlayCarAt(Pos.TORIGHT);
            SetRandomVars();
            _toRightTunnelTimer = 0;
        }
        if (_leftThroughTimer > _rand2)
        {
            PlayCarAt(Pos.LEFT);
            SetRandomVars();
            _leftThroughTimer = 0;
        }
        else if (_rightThroughTimer > _rand2)
        {
            PlayCarAt(Pos.RIGHT);
            SetRandomVars();
            _rightThroughTimer = 0;
        }
        
    }

    private void PlayCarAt(Pos pos)
    {
        var randomCar = Instantiate(_cars[Random.Range(0, _cars.Length)]);
        randomCar.transform.parent = map;
        randomCar.GetComponent<Animator>().Play(_animVal[pos]);
        Destroy(randomCar, 5f);
    }

    private void SetRandomVars()
    {
        _toLeftTunnelTimer = Random.Range(0, 10);
        _toRightTunnelTimer = Random.Range(0, 10);
        _leftThroughTimer = Random.Range(0, 10);
        _rightThroughTimer = Random.Range(0, 10);

        _rand1 = Random.Range(10, 20);
        _rand2 = Random.Range(7, 20);
    }
}
