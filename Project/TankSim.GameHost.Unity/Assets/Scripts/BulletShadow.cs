using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletShadow : MonoBehaviour
{

    private Vector3 _start;
    private Vector3 _end;
    private Vector3 _endSize;

    private Vector3 _minSize = new Vector3(0.5f, 0.5f);


    private bool _scaleToEnd = false;

    private float _scaleTime = 0;
    private float _positionTime = 0;

    private bool _expandScale = false;

    private bool _isSmallExplosion;

    public Explosion SmallExplosionPrefab;
    public Explosion LargeExplosionPrefab;

    // Start is called before the first frame update
    void Start()
    {

        transform.localScale = _endSize;

        Destroy(gameObject, DamageCircle.DamgeTime);
        Invoke(nameof(ExpandScale), DamageCircle.DamgeTime / 2.0f);
    }

    // Update is called once per frame
    void Update()
    {
        _scaleTime += Time.deltaTime * DamageCircle.DamgeTime;
        _positionTime += Time.deltaTime / DamageCircle.DamgeTime;
        if (_expandScale)
        {
            transform.localScale = Vector3.Lerp(_endSize, _minSize, _scaleTime);
        }
        else
        {
            transform.localScale = Vector3.Lerp(_minSize, _endSize, _scaleTime);
        }

        transform.position = Vector3.Slerp(_start, _end, _positionTime);
    }

    void ExpandScale()
    {
        _expandScale = true;
        _scaleTime = 0;
    }


    public void SetShadowInfo(Vector3 start, Vector3 end, Vector3 endScale, bool isSmallExplosion)
    {
        //need to clone because it is pass by reference
        _start = new Vector3(start.x, start.y, start.z);
        _end = new Vector3(end.x, end.y, end.z);
        _endSize = new Vector3(endScale.x, endScale.y, endScale.z);
        _isSmallExplosion = isSmallExplosion;
    }

    private void OnDestroy()
    {
        if (_isSmallExplosion)
        {
            Instantiate(SmallExplosionPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Instantiate(LargeExplosionPrefab, transform.position, Quaternion.identity);
        }
        
    }
}
