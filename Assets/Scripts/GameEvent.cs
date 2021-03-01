using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    [Serializable]
    public class GameEvent
    {
        [NonSerialized]
        public EventType enType;
        public string type;
        public Dictionary<string, object> data;

        public GameEvent() { }

        public GameEvent(EventType type)
        {
            this.enType = type;
            this.type = type.ToString();
        }
    }

    public class GameEventFactory
    {
        static public GameEvent Create(EventType type, long time, string version, Dictionary<string, object> data)
        {
            GameEvent result = new GameEvent(type);
            result.data = new Dictionary<string, object>();
            data.Add("time", time);
            data.Add("version", version);
            if (data != null)
                foreach (string k in data.Keys)
                    result.data.Add(k, data[k]);
            return result;
        }
    }

    [Serializable]
    public enum EventType
    {
        empty = -1,
        level_start,
        level_win,
        level_lose,
        payment
    }
}