using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    [Serializable]
    public class FixtureDefinition
    {
        public int id;
        public string manufacturer;
        public string model;
        public ChannelDefinition[] channels;
        public MovementDefinition[] movements;
        public float lux;
        public float beamAngle;
    }

    [Serializable]
    public class ChannelDefinition
    {
        public int min;
        public int max;
        public int address;
        public string name;
    }

    [Serializable]
    public class MovementDefinition
    {
        public int id;
        public string name;
        public int min;
        public int max;
    }
}
