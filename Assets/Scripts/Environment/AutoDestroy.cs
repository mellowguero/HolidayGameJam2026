using System.Collections;
using UnityEngine;

namespace Environment
{
    public class AutoDestroy : MonoBehaviour
    {
        [SerializeField] private float lifetime = 2f;
        
        [Header("Fade Settings")]
        [SerializeField] private bool fadeBeforeDestroy = true;
        [SerializeField] private float fadeDuration = 0.5f;
        [SerializeField] private bool shrinkWhileFading = true;
        [SerializeField] private float shrinkToScale = 0.1f;

        private Renderer _renderer;
        private Material[] _materials;
        private Color[] _originalColors;
        private Vector3 _originalScale;

        public void Initialize(float newLifetime, float newFadeDuration, bool enableShrink = true)
        {
            lifetime = newLifetime;
            fadeDuration = newFadeDuration;
            shrinkWhileFading = enableShrink;
        }

        private void Start()
        {
            _renderer = GetComponent<Renderer>();
            _originalScale = transform.localScale;
            
            if (fadeBeforeDestroy && _renderer != null)
            {
                _materials = _renderer.materials;
                _originalColors = new Color[_materials.Length];
                
                for (int i = 0; i < _materials.Length; i++)
                {
                    if (_materials[i].HasProperty("_BaseColor"))
                    {
                        _originalColors[i] = _materials[i].GetColor("_BaseColor");
                    }
                    else if (_materials[i].HasProperty("_Color"))
                    {
                        _originalColors[i] = _materials[i].GetColor("_Color");
                    }
                    
                    SetMaterialTransparent(_materials[i]);
                }
                
                StartCoroutine(FadeAndDestroy());
            }
            else
            {
                Destroy(gameObject, lifetime);
            }
        }

        private void SetMaterialTransparent(Material mat)
        {
            if (mat.HasProperty("_Surface"))
            {
                mat.SetFloat("_Surface", 1);
                mat.SetFloat("_Blend", 0);
            }
            
            mat.SetOverrideTag("RenderType", "Transparent");
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.renderQueue = 3000;
        }

        private IEnumerator FadeAndDestroy()
        {
            float fadeStartTime = lifetime - fadeDuration;
            
            if (fadeStartTime > 0)
            {
                yield return new WaitForSeconds(fadeStartTime);
            }
            
            float elapsed = 0f;
            
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / fadeDuration;
                float alpha = Mathf.Lerp(1f, 0f, t);
                
                for (int i = 0; i < _materials.Length; i++)
                {
                    if (_materials[i] == null) continue;
                    
                    Color newColor = _originalColors[i];
                    newColor.a = alpha;
                    
                    if (_materials[i].HasProperty("_BaseColor"))
                    {
                        _materials[i].SetColor("_BaseColor", newColor);
                    }
                    else if (_materials[i].HasProperty("_Color"))
                    {
                        _materials[i].SetColor("_Color", newColor);
                    }
                }
                
                if (shrinkWhileFading)
                {
                    float scale = Mathf.Lerp(1f, shrinkToScale, t);
                    transform.localScale = _originalScale * scale;
                }
                
                yield return null;
            }
            
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            if (_materials != null)
            {
                foreach (Material mat in _materials)
                {
                    if (mat != null)
                    {
                        Destroy(mat);
                    }
                }
            }
        }
    }
}
