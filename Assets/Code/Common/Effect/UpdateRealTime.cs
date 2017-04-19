using UnityEngine;
using System.Collections;

public class UpdateRealTime : MonoBehaviour {
	private Renderer _renderer;
	private Material _mat;
	private int _realTimeId;
	// Use this for initialization
	void Start () {
		_renderer = GetComponent<Renderer> ();
		if (_renderer)
			_mat = _renderer.material;
		_realTimeId = Shader.PropertyToID("_RealTime");
	}
	
	// Update is called once per frame
	void Update () {
		if (_mat)
			_mat.SetFloat (_realTimeId, Time.realtimeSinceStartup);
		else
			UnityEngine.Object.Destroy (this);
	}
}
