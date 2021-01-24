using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bloom : MonoBehaviour
{
	[SerializeField]
	Shader shader = null;

	Material mat = null;

	[SerializeField, Range(1, 4)]
	int iteration = 1;

	[SerializeField, Range(0, 1)]
	float _threhold = 0.1f;

	[SerializeField]
	float _intensity = 1.0f;

	private void OnEnable()
	{
		mat = new Material(shader);
	}

	void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		//RenderTextureDescriptor desc = new RenderTextureDescriptor();
		//desc.width = source.width / 2;
		//desc.height = source.height / 2;
		//desc.depthBufferBits = 0;
		//desc.colorFormat = RenderTextureFormat.ARGB32;
		//var current = RenderTexture.GetTemporary(desc);

		var current = RenderTexture.GetTemporary(source.width / 2, source.height / 2, 0, source.format);



		// down sampling & extraction
		mat.SetFloat("_Threshold", _threhold);
		Vector4 offset = new Vector4(0, 0, 0, 0);
		Graphics.Blit(source, current, mat, 0);

		var tmp = RenderTexture.GetTemporary(current.width / 2, current.height / 2, 0, source.format);
		Graphics.Blit(current, tmp, mat, 1);

		current = RenderTexture.GetTemporary(current.width / 2, current.height / 2, 0, source.format);
		Graphics.Blit(tmp, current, mat, 1);

		tmp = RenderTexture.GetTemporary(current.width / 2, current.height / 2, 0, source.format);
		Graphics.Blit(current, tmp, mat, 1);

		mat.SetTexture("_BlurTex", tmp);
		mat.SetFloat("_Intensity", _intensity);
		Graphics.Blit(source, destination, mat, 3);
		return;

		// down sampling
		//for (int i = 0; i < iteration - 1; i++)
		//{
		//	desc.width /= 2;
		//	desc.width /= 2;
		//	Graphics.Blit(source, mat, 1);
		//}

		// blur
		mat.SetVector("_Offset", offset);
		Graphics.Blit(source, mat, 1);

		mat.SetVector("_Offset", offset);
		Graphics.Blit(source, mat, 1);

		// up sampling
		for (int i = 0; i < iteration; i++)
		{

		}

		mat.SetTexture("_BlurTex", current);
		Graphics.Blit(source, destination, mat, 2);
		RenderTexture.ReleaseTemporary(current);
	}
}
