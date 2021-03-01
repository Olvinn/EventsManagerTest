using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using System.IO;
using Newtonsoft.Json;

namespace Assets.Scripts
{
    public class EventsManager : MonoBehaviour
    {
        public string url;
        public float cooldownBeforeSend = 5f;

        List<GameEvent> events; 

        float timer;

        void Awake()
        {
            events = new List<GameEvent>();
        }

        private void Start()
        {
            //TrackEvent(GameEventFactory.Create(EventType.level_lose, System.DateTime.Now.Ticks, "debug", new Dictionary<string, object>() { { "level", 1 } }));
            //TrackEvent(GameEventFactory.Create(EventType.payment, System.DateTime.Now.Ticks, "debug", new Dictionary<string, object>() { { "id", 6 }, { "cost", 200 } }));

            LoadEvents();
        }

        void FixedUpdate()
        {
            timer -= Time.fixedDeltaTime;
            if (timer <= 0)
            {
                SendEvents();
                timer = cooldownBeforeSend;
            }
        }

        public void TrackEvent(GameEvent value)
        {
            if (!events.Contains(value))
            {
                if (value.enType != EventType.empty)
                    events.Add(value);
                SaveEvents();
            }
        }

        void SendEvents()
        {
            if (events.Count == 0)
                return;
            List<GameEvent> uploading = new List<GameEvent>(events);
            string data = JsonConvert.SerializeObject(uploading);
            StartCoroutine(Upload(data, uploading));
            events.Clear();
        }

        void SaveEvents()
        {
            if (!Directory.Exists("EventsManager"))
            {
                Directory.CreateDirectory("EventsManager");
            }
            File.WriteAllText("EventsManager/events.txt", JsonConvert.SerializeObject(events));
        }

        void LoadEvents()
        {
            if (File.Exists("EventsManager/events.txt"))
            {
                events = JsonConvert.DeserializeObject<List<GameEvent>>(File.ReadAllText("EventsManager/events.txt"));
            }
        }

        IEnumerator Upload(string data, List<GameEvent> uploading)
        {
            Debug.Log(data);
            using (UnityWebRequest www = UnityWebRequest.Post(url, data))
            {
                yield return www.SendWebRequest();

                if (www.isNetworkError || www.isHttpError || www.responseCode != 200)
                { 
                    Debug.Log("EventsManager: error - " + www.error + ";\ncode: " + www.responseCode);
                    events.AddRange(uploading);
                    SaveEvents();
                }
            }
        }

        //public static class JsonHelper
        //{
        //    public static GameEvent[] FromJson<T>(string json)
        //    {
        //        Wrapper<GameEvent> wrapper = JsonUtility.FromJson<Wrapper<GameEvent>>(json);
        //        return wrapper.events;
        //    }

        //    public static string ToJson(GameEvent[] array)
        //    {
        //        string resultsJson = "";
        //        resultsJson += "{\"events\":[";
        //        for (int i = 0; i < array.Length; i++)
        //        {
        //            resultsJson += array[i].ToJson();
        //            if (i < array.Length - 1)
        //            {
        //                resultsJson += ",";
        //            }
        //        }
        //        resultsJson += "]}";
        //        return resultsJson;
        //    }

        //    public static string ToJson(GameEvent[] array, bool prettyPrint)
        //    {
        //        Wrapper<GameEvent> wrapper = new Wrapper<GameEvent>();
        //        wrapper.events = array;
        //        return JsonUtility.ToJson(wrapper, prettyPrint);
        //    }

        //    [Serializable]
        //    private class Wrapper<T>
        //    {
        //        public T[] events;
        //    }
        //}
    }
}
