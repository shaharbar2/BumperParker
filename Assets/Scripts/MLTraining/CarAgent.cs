using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace Shahar.Bar.ML
{
    public class CarAgent : Agent
    {
        [SerializeField] public Transform parkingLocation;
        [SerializeField] private CarController car;
        [SerializeField] private Rigidbody rigid;

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void CollectObservations(VectorSensor sensor)
        {   
            sensor.AddObservation(parkingLocation.transform.position);
            sensor.AddObservation(transform.position);
            sensor.AddObservation(transform.rotation);
            sensor.AddObservation(rigid.velocity);
            sensor.AddObservation(Vector3.Distance(transform.position, parkingLocation.transform.position));

            base.CollectObservations(sensor);
        }

        public override void OnActionReceived(float[] vectorAction)
        {
            
            car.steeringAngle = vectorAction[0];
            car.forwardVelocity = vectorAction[1];
            car.horizontalInput = vectorAction[2];
            car.verticalInput = vectorAction[3];
            car.brakeInput = vectorAction[4] > 0;
            car.brakeForceInput = vectorAction[5];
            car.boostInput = vectorAction[6] > 0;
        }

        public override void OnEpisodeBegin()
        {
            base.OnEpisodeBegin();
        }

    }
}
