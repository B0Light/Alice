using UnityEngine;

namespace HeneGames.BugController
{
    public class LegsControllerExample : MonoBehaviour
    {
        [Header("Bug Controller Reference")]
        [SerializeField] private BugController bugController;

        [Header("Horizontal values")]
        [SerializeField] private bool horizontalOverride;

        [Range(0f, 5f)]
        [SerializeField] private float horizontalPosition = 1f;

        [Header("Vertical values")]
        [SerializeField] private bool verticalOverride;

        [Range(0f, 5f)]
        [SerializeField] private float verticalPosition = 1f;

        [Header("Refresh legs")]
        [SerializeField] private bool refresh;

        private void Update()
        {
            bugController.SetLegsHorizontalPosition(horizontalOverride, horizontalPosition);

            bugController.SetLegsVerticalPostition(verticalOverride, verticalPosition);

            if(refresh)
            {
                bugController.AdjustLegs();
                refresh = false;
            }
        }
    }
}