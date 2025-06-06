using UnityEngine;

public class TailMovement : MonoBehaviour
{
    public int length;
    public float smoothspeed;
    public float T_Dist;

    public Vector3[] segmentPoses;
    private Vector3[] segmentV;

    public LineRenderer lineren;

    public Transform targetDir;

    //Wiggle Variables
    public float wiggleSpeed;
    public float wiggleMag;
    public Transform wiggleDir;

    [HideInInspector] public float startingWiggleSpeed; 



    // Start is called before the first frame update
    void Start()
    {
        lineren.positionCount = length;
        segmentPoses = new Vector3[length];
        segmentV = new Vector3[length];

        startingWiggleSpeed = wiggleSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        wiggleDir.localRotation = Quaternion.Euler(0, 0, Mathf.Sin(Time.time * wiggleSpeed) * wiggleMag);
        segmentPoses[0] = targetDir.position;

        for(int i = 1; i < segmentPoses.Length; i++)
        {
            segmentPoses[i] = Vector3.SmoothDamp(segmentPoses[i], segmentPoses[i-1] + targetDir.right * T_Dist, ref segmentV[i], smoothspeed);
        }
        lineren.SetPositions(segmentPoses);
    }
}