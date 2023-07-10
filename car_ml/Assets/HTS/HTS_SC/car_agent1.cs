using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System.IO;
using System.Text;

public class car_agent1 : Agent
{
    [SerializeField]
    private WheelCollider[] wheels = new WheelCollider[4];
    [SerializeField]
    private Transform[] tires = new Transform[4];

    private string filename = "./image_F/image/";
    private int filenumber = 1;
    private bool camera_on = false;
    float captureInterval = 0.1f;
    float timer = 0f;

    string filePath = "./Image_F/df.csv";
    StringBuilder sb = new StringBuilder();
    float csv_car_angle;

    private float maxpower = 5f;
    private float power = 250f;
    private float rot = 20f;
    private Rigidbody rb;

    private Vector3 wheel_position;
    private Quaternion wheel_rotation; 

    private void Start()
    {
        if(!File.Exists(filePath))
        {
            sb.AppendLine("image_name,drive_mod,speed,angle");
        }
        rb = this.GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, 0, 0);
    }

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(0, 0, 0);
        transform.rotation = Quaternion.identity;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        
    }
    private void Update()
    {
        timer += Time.deltaTime;

        if(Input.GetKeyDown(KeyCode.C))
        {
            if(camera_on)
            {
                camera_on = false;
            }
            else
            {
                camera_on = true;
            }
        }

        if (camera_on && timer>captureInterval)
        {
            capture_def();
            timer = 0;
        }
    }
    void capture_def()
    {
        string image_number_temp = ("image_"+filenumber.ToString("D7"));
        ScreenCapture.CaptureScreenshot(filename+image_number_temp+".png");
        string newLine = string.Format("{0},{1},{2},{3}", image_number_temp, 2, 0.8, csv_car_angle);
        sb.Clear();
        sb.AppendLine(newLine);
        File.AppendAllText(filePath, sb.ToString());

        filenumber += 1;
        Debug.Log("��Ĭ");
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        var car_angle = Mathf.Floor(actions.ContinuousActions[0]*10) / 10;
        csv_car_angle = Mathf.Floor((car_angle+1)/2 *10f)/10f;
        var car_speed = 5f;

        if (-0.2<car_angle & car_angle<0.2)
        {
            AddReward(0.5f);
        }
        AddReward(1f);

        if (rb.velocity.magnitude >= maxpower / 5)
        {
            if (rb.velocity.magnitude >= maxpower)
            {
                rb.velocity = rb.velocity.normalized * maxpower;
            }
        }
        for (int i=2; i<4; i++)
        {
            wheels[i].motorTorque = car_speed * power;

            wheels[i].GetWorldPose(out wheel_position, out wheel_rotation);
            tires[i].rotation = wheel_rotation;
        }
        for(int i=0; i<2; i++)
        {
            wheels[i].steerAngle = car_angle * rot;
            wheels[i].GetWorldPose(out wheel_position, out wheel_rotation);
            tires[i].rotation = wheel_rotation;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("floor"))
        {
            AddReward(-100f);
            EndEpisode();
        }
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var action = actionsOut.ContinuousActions;
        action[0] = Input.GetAxis("Horizontal");
    }
}
