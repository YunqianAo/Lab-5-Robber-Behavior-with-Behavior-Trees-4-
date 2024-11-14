using UnityEngine;
using Pada1.BBCore;
using Pada1.BBCore.Tasks;
using Pada1.BBCore.Framework;

[Action("MyActions/HideAndFlee")]
[Help("Find the farthest hiding spot from the cop and move towards it.")]
public class HideAndFleeBB : BasePrimitiveAction
{
    [InParam("game object")]
    [Help("Game object that will move to the hiding spot.")]
    public GameObject targetGameObject;

    [OutParam("hide")]
    [Help("Vector3 position of the hiding spot.")]
    public Vector3 hide;

    private GameObject[] hidingSpots;
    private GameObject currentHidingSpot;
    private Vector3 chosenHidingSpotPosition;
    private const float arriveThreshold = 0.5f;

    public override void OnStart()
    {
        base.OnStart();
        hidingSpots = GameObject.FindGameObjectsWithTag("Hide");

        if (hidingSpots.Length == 0)
        {
            Debug.LogWarning("No hiding spots found!");
            return;
        }

        SelectHidingSpot();
    }

    public override TaskStatus OnUpdate()
    {
        if (hidingSpots.Length == 0) return TaskStatus.FAILED;

        Moves moves = targetGameObject.GetComponent<Moves>();
        if (Vector3.Distance(targetGameObject.transform.position, chosenHidingSpotPosition) > arriveThreshold)
        {
            moves.Seek(hide);
            return TaskStatus.RUNNING;
        }

        SelectHidingSpot();
        return TaskStatus.RUNNING;
    }

    private void SelectHidingSpot()
    {
        GameObject cop = GameObject.FindGameObjectWithTag("Cop");
        if (cop == null)
        {
            Debug.LogWarning("No cop found!");
            return;
        }

        float maxDistanceFromCop = 0;
        GameObject chosenHidingSpot = null;

        foreach (GameObject spot in hidingSpots)
        {
            if (spot == currentHidingSpot) continue;

            float distanceFromCop = Vector3.Distance(cop.transform.position, spot.transform.position);
            if (distanceFromCop > maxDistanceFromCop)
            {
                maxDistanceFromCop = distanceFromCop;
                chosenHidingSpot = spot;
            }
        }

        if (chosenHidingSpot != null)
        {
            currentHidingSpot = chosenHidingSpot;
            chosenHidingSpotPosition = currentHidingSpot.transform.position;
            hide = chosenHidingSpotPosition;
        }
    }
}
