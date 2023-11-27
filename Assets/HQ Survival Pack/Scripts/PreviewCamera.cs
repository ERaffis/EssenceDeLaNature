using UnityEngine;

namespace HQSurvival
{
    public enum Scenes { Standard, Icons }
    public class PreviewCamera : MonoBehaviour
    {
        [SerializeField] Scenes sceneType = Scenes.Standard;

        Animator animator;


        void Start()
        {
            animator = GetComponent<Animator>();

            if (sceneType == Scenes.Standard)
            {
                animator.SetTrigger("Standard");
            }
            else animator.SetTrigger("Icons");
        }
    }
}
