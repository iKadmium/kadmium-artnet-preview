using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    [Serializable]
    class FixtureDefinition
    {
        public string name;
        public string type;
        public ChannelDefinition[] channels;
        public MovementDefinition[] movements;
    }

    [Serializable]
    public class ChannelDefinition
    {
        public string name;
        public int dmx;
        public byte min;
        public byte max;
    }

    [Serializable]
    public class MovementDefinition
    {
        public string name;
        public int min;
        public int max;
    }
}
