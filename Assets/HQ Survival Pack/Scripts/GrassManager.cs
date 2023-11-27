using UnityEngine;

namespace HQSurvival
{
	[ExecuteInEditMode]
	public class GrassManager : MonoBehaviour 
	{
//		[SerializeField]
//		private Color m_TranslucencyColor = Color.white;
//
//		[SerializeField]
//		private float m_Gloss;

		[SerializeField]
		[Range(0f, 1f)]
		private float m_Shininess = 0.5f;


		private void OnEnable()
		{
			//Shader.SetGlobalColor("_GrassTranslucencyColor", m_TranslucencyColor);
			//Shader.SetGlobalFloat("_GrassGloss", m_Gloss);

			Shader.SetGlobalFloat("_GrassShininess", m_Shininess);
		}

		private void OnValidate()
		{
			//Shader.SetGlobalColor("_GrassTranslucencyColor", m_TranslucencyColor);
			//Shader.SetGlobalFloat("_GrassGloss", m_Gloss);

			Shader.SetGlobalFloat("_GrassShininess", m_Shininess);
		}
	}
}
