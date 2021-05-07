/**
 * Copyright (c) 2019-2021 LG Electronics, Inc.
 *
 * This software contains code licensed as described in LICENSE.
 *
 */

using UnityEngine;
using Simulator.Bridge;
using Simulator.Utilities;
using Simulator.Sensors.UI;
using System.Collections.Generic;

namespace Simulator.Sensors
{
    [SensorType("Cruise Control", new System.Type[] { })]
    public class CruiseControlSensor : SensorBase, IVehicleInputs
    {
        [SensorParameter]
        [Range(0.0f, 200f)]
        public float CruiseSpeed = 0f;

        private IVehicleDynamics Dynamics;
        private IAgentController Controller;

        public float SteerInput { get; private set; } = 0f;
        public float AccelInput { get; private set; } = 0f;
        public float BrakeInput { get; private set; } = 0f;

        [AnalysisMeasurement(MeasurementType.Velocity)]
        public float MaxSpeed = 0;

        public override SensorDistributionType DistributionType => SensorDistributionType.LowLoad;

        private void Start()
        {
            Dynamics = GetComponentInParent<IVehicleDynamics>();
            Controller = GetComponentInParent<IAgentController>();
        }

        public void Update()
        {
            Debug.Assert(Dynamics != null);

            if (Controller.AccelInput >= 0)
            {
                AccelInput = Dynamics.Velocity.magnitude < CruiseSpeed ? 1f : 0f;
            }

            MaxSpeed = Mathf.Max(MaxSpeed, Dynamics.Velocity.magnitude);
        }

        public override void OnBridgeSetup(BridgeInstance bridge)
        {
            // TODO new base class?
        }

        public override void OnVisualize(Visualizer visualizer)
        {
            Debug.Assert(visualizer != null);

            var graphData = new Dictionary<string, object>()
            {
                {"Cruise Speed", CruiseSpeed},
                {"Steer Input", SteerInput},
                {"Accel Input", AccelInput},
                {"Speed", Dynamics.Velocity.magnitude},
                {"Hand Brake", Dynamics.HandBrake},
                {"Ignition", Dynamics.CurrentIgnitionStatus},
                {"Reverse", Dynamics.Reverse},
                {"Gear", Dynamics.CurrentGear},
                {"RPM", Dynamics.CurrentRPM},
                {"Velocity", Dynamics.Velocity}
            };
            visualizer.UpdateGraphValues(graphData);
        }

        public override void OnVisualizeToggle(bool state)
        {
            //
        }
    }
}
