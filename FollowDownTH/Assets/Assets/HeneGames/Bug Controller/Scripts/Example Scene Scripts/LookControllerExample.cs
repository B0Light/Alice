using UnityEngine;

namespace HeneGames.BugController
{
    public class LookControllerExample : MonoBehaviour
    {
        [Header("Bug Controller Reference")]
        [SerializeField] private BugController bugController;

        [Header("Look values")]
        [SerializeField] private bool look = true;
        [SerializeField] private float lookSpeed = 5f;
        [SerializeField] private Transform lookTransform;


        private void Update()
        {
            if(look)
            {
                bugController.LookTarget(lookTransform, lookSpeed);
            }
        }
    }
}