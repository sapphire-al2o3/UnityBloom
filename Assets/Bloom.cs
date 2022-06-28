using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bloom : MonoBehaviour
{
    [SerializeField]
    Shader shader = null;

    Material _mat = null;

	[SerializeField, Range(1, 6)]
	int pyramid = 2;

	[SerializeField, Range(0, 1)]
    float _threhold = 0.1f;

    [SerializeField]
    float _intensity = 1.0f;

    RenderTexture[] _blur = new RenderTexture[6];

    readonly static int _Threshold = Shader.PropertyToID("_Threshold");
	readonly static int _BlurTex = Shader.PropertyToID("_BlurTex");
	readonly static int _Intensity = Shader.PropertyToID("_Intensity");

	private void OnEnable()
    {
        _mat = new Material(shader);
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
		int width = source.width / 2;
		int height = source.height / 2;
        var current = _blur[0] = RenderTexture.GetTemporary(width, height, 0, source.format);

        // down sampling & extraction
        _mat.SetFloat("_Threshold", _threhold);
        Graphics.Blit(source, current, _mat, 0);

        RenderTexture tmp = null;

        // down sampling
        int i = 1;
        for (; i < pyramid; i++)
		{
            width /= 2;
            height /= 2;

            if (width < 2 || height < 2)
            {
                break;
            }

            tmp = _blur[i] = RenderTexture.GetTemporary(width, height, 0, source.format);
			
            Graphics.Blit(current, tmp, _mat, 1);
			current = tmp;
        }

        for (i -= 2; i >= 0; i--)
        {
            tmp = _blur[i];
            Graphics.Blit(current, tmp, _mat, 2);
			RenderTexture.ReleaseTemporary(current);
			current = tmp;
        }

        _mat.SetTexture("_BlurTex", current);
        _mat.SetFloat("_Intensity", _intensity);
        Graphics.Blit(source, destination, _mat, 3);
		RenderTexture.ReleaseTemporary(current);

        return;
    }
}
