using UnityEngine;
using System.Collections;

public class test_uv : MonoBehaviour
{
    public float xspeed = 0;
    public float yspeed = 0;
	public int count = -1;

	private Material _mat;
    private Vector2 v2;
	private Vector2 lastV2;

    void Start()
    {
        v2 = Vector2.zero;
		lastV2 = v2;
        Renderer r = GetComponent<Renderer>();
        if (r)
            _mat = r.material;
    }
    void Update()
    {
		if (_mat && count != 0)
        {
            v2.x += Time.fixedDeltaTime * xspeed;
            v2.y += Time.fixedDeltaTime * yspeed;
            _mat.mainTextureOffset = v2;

			if(count > 0)
			{
				if(v2.x - lastV2.x >= 1)
				{
					lastV2 = v2;
					count --;
				}else if( v2.y - lastV2.y >= 1)
				{
					lastV2 = v2;
					count -- ;
				}
			}

        }
    }
}
