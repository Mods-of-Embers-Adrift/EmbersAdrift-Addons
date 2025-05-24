using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using MelonLoader;

namespace DPSAddon
{
    public class DPSTracker : MonoBehaviour
    {
        private const string LOG_PREFIX = "[DPSTracker]";
        private const int MAX_PAST_FIGHTS = 20;
        private const float DEFAULT_COMBAT_TIMEOUT = 8f;

        private static DPSTracker _instance;
        private static GameObject _trackerObject;
        private readonly Dictionary<string, DpsEntry> _dpsDict = new Dictionary<string, DpsEntry>();
        private readonly Dictionary<string, DpsEntry> _hpsDict = new Dictionary<string, DpsEntry>();
        private readonly List<FightSession> _pastFights = new List<FightSession>();

        private float _lastActionTime;
        private float _combatTimeout = DEFAULT_COMBAT_TIMEOUT;

        public static IReadOnlyDictionary<string, DpsEntry> Entries => _instance?._dpsDict;
        public static IReadOnlyDictionary<string, DpsEntry> HealingEntries => _instance?._hpsDict;
        public static IReadOnlyList<FightSession> PastFights => _instance?._pastFights;

        public static Dictionary<string, float> CurrentDps =>
            _instance?._dpsDict.ToDictionary(kv => kv.Key, kv => kv.Value.DPS);

        public static Dictionary<string, float> CurrentHps =>
            _instance?._hpsDict.ToDictionary(kv => kv.Key, kv => kv.Value.HPS);

        private void Awake()
        {
            MelonLogger.Msg($"{LOG_PREFIX} Awake called");

            // If we already have an instance but it's not this one, destroy this one
            if (_instance != null && _instance != this)
            {
                MelonLogger.Msg($"{LOG_PREFIX} Destroying duplicate instance");
                Destroy(gameObject);
                return;
            }

            // If this is the first instance, set it up
            if (_instance == null)
            {
                _instance = this;
                _trackerObject = gameObject;
                DontDestroyOnLoad(gameObject);
                MelonLogger.Msg($"{LOG_PREFIX} Instance set in Awake");
            }
        }

        public static void Launch()
        {
            try
            {
                MelonLogger.Msg($"{LOG_PREFIX} Launch called");

                // If we already have an instance and it's valid, use it
                if (_instance != null && _instance.gameObject != null)
                {
                    MelonLogger.Msg($"{LOG_PREFIX} Instance already exists");
                    return;
                }

                // If we have a tracker object but no instance, try to get the instance
                if (_trackerObject != null)
                {
                    _instance = _trackerObject.GetComponent<DPSTracker>();
                    if (_instance != null)
                    {
                        MelonLogger.Msg($"{LOG_PREFIX} Recovered existing instance");
                        return;
                    }
                }

                // Create new instance
                _trackerObject = new GameObject("DPSTracker");
                if (_trackerObject == null)
                {
                    MelonLogger.Error($"{LOG_PREFIX} Failed to create GameObject");
                    return;
                }

                _instance = _trackerObject.AddComponent<DPSTracker>();
                if (_instance == null)
                {
                    MelonLogger.Error($"{LOG_PREFIX} Failed to add DPSTracker component");
                    Destroy(_trackerObject);
                    return;
                }

                DontDestroyOnLoad(_trackerObject);

                MelonLogger.Msg($"{LOG_PREFIX} Successfully created tracker instance");
                MelonLogger.Msg($"{LOG_PREFIX} GameObject name: {_trackerObject.name}");
                MelonLogger.Msg($"{LOG_PREFIX} Instance active: {_instance.gameObject.activeSelf}");
                MelonLogger.Msg($"{LOG_PREFIX} Instance enabled: {_instance.enabled}");
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"{LOG_PREFIX} Error in Launch: {ex.Message}");
                MelonLogger.Error($"{LOG_PREFIX} Stack trace: {ex.StackTrace}");
            }
        }

        public static void TestRecord()
        {
            if (_instance == null)
            {
                MelonLogger.Error($"{LOG_PREFIX} Test failed - instance is null");
                return;
            }

            RecordDamage("TestPlayer", 100);
            MelonLogger.Msg($"{LOG_PREFIX} Test record completed");
            MelonLogger.Msg($"{LOG_PREFIX} Current entries: {_instance._dpsDict.Count}");
        }

        public static void RecordDamage(string source, float amount)
        {
            MelonLogger.Msg($"{LOG_PREFIX} Attempting to record damage - Source: {source}, Amount: {amount}");

            if (_instance == null)
            {
                MelonLogger.Error($"{LOG_PREFIX} Instance is null!");
                return;
            }

            _instance._lastActionTime = Time.time;

            if (!_instance._dpsDict.TryGetValue(source, out var entry))
            {
                MelonLogger.Msg($"{LOG_PREFIX} Creating new entry for {source}");
                entry = new DpsEntry { StartTime = Time.time };
                _instance._dpsDict[source] = entry;
            }

            entry.TotalDamage += amount;
            MelonLogger.Msg($"{LOG_PREFIX} Recorded {amount} damage for {source} (Total: {entry.TotalDamage:F0})");
        }

        public static void RecordHealing(string source, float amount)
        {
            try
            {
                MelonLogger.Msg($"{LOG_PREFIX} Attempting to record healing - Source: {source}, Amount: {amount}");

                // Validate instance
                if (_instance == null)
                {
                    MelonLogger.Error($"{LOG_PREFIX} Instance is null! Attempting to create new instance...");
                    Launch();
                    if (_instance == null)
                    {
                        MelonLogger.Error($"{LOG_PREFIX} Failed to create instance!");
                        return;
                    }
                }

                // Validate parameters
                if (string.IsNullOrEmpty(source))
                {
                    MelonLogger.Error($"{LOG_PREFIX} Source name is null or empty!");
                    return;
                }

                if (float.IsNaN(amount) || float.IsInfinity(amount))
                {
                    MelonLogger.Error($"{LOG_PREFIX} Invalid healing amount: {amount}");
                    return;
                }

                _instance._lastActionTime = Time.time;

                // Get or create entry
                if (!_instance._hpsDict.TryGetValue(source, out var entry))
                {
                    MelonLogger.Msg($"{LOG_PREFIX} Creating new entry for {source}");
                    entry = new DpsEntry
                    {
                        StartTime = Time.time,
                        Name = source  // Make sure Name is set
                    };
                    _instance._hpsDict[source] = entry;
                }

                // Record healing
                entry.TotalHealing += amount;
                entry.EndTime = Time.time;  // Update end time

                MelonLogger.Msg($"{LOG_PREFIX} Recorded {amount:F0} healing for {source} " +
                               $"(Total: {entry.TotalHealing:F0}, HPS: {entry.HPS:F1})");
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"{LOG_PREFIX} Error recording healing: {ex.Message}");
                MelonLogger.Error($"{LOG_PREFIX} Stack trace: {ex.StackTrace}");
            }
        }

        private void Update()
        {
            if (_dpsDict.Count == 0 && _hpsDict.Count == 0) return;

            if (Time.time - _lastActionTime > _combatTimeout)
            {
                SaveSession();
                ClearCurrentSession();
            }
        }

        private void SaveSession()
        {
            var session = new FightSession
            {
                StartTimestamp = DateTime.Now,
                EndTime = Time.time
            };

            foreach (var kvp in _dpsDict)
            {
                session.Entries[kvp.Key] = kvp.Value.Clone();
            }

            foreach (var kvp in _hpsDict)
            {
                if (!session.Entries.TryGetValue(kvp.Key, out var existing))
                {
                    session.Entries[kvp.Key] = kvp.Value.Clone();
                }
                else
                {
                    existing.TotalHealing += kvp.Value.TotalHealing;
                    existing.EndTime = Mathf.Max(existing.EndTime, Time.time);
                }
            }

            _pastFights.Insert(0, session);
            if (_pastFights.Count > MAX_PAST_FIGHTS)
            {
                _pastFights.RemoveAt(_pastFights.Count - 1);
            }
        }

        public static void Reset()
        {
            if (_instance == null) return;
            _instance.ClearCurrentSession();
            _instance._pastFights.Clear();
        }

        private void ClearCurrentSession()
        {
            _dpsDict.Clear();
            _hpsDict.Clear();
        }

        private void OnDisable()
        {
            MelonLogger.Msg($"{LOG_PREFIX} OnDisable called");
            // Don't clear the instance here
        }

        private void OnDestroy()
        {
            MelonLogger.Msg($"{LOG_PREFIX} OnDestroy called");
            // Only clear the instance if this is the current instance
            if (_instance == this)
            {
                _instance = null;
                _trackerObject = null;
            }
        }
    }

    public class DpsEntry
    {
        public string Name { get; set; }
        public float TotalDamage { get; set; }
        public float TotalHealing { get; set; }
        public float StartTime { get; set; }
        public float EndTime { get; set; }

        public float Duration => Mathf.Max(((EndTime > 0f) ? EndTime : Time.time) - StartTime, 0.01f);
        public float DPS => TotalDamage / Duration;
        public float HPS => TotalHealing / Duration;

        public DpsEntry Clone()
        {
            return new DpsEntry
            {
                Name = Name,
                TotalDamage = TotalDamage,
                TotalHealing = TotalHealing,
                StartTime = StartTime,
                EndTime = Time.time
            };
        }
    }

    public class FightSession
    {
        public DateTime StartTimestamp { get; set; }
        public float EndTime { get; set; }
        public Dictionary<string, DpsEntry> Entries { get; } = new Dictionary<string, DpsEntry>();

        public float Duration
        {
            get
            {
                if (!Entries.Any()) return 0.01f;
                var minStart = Entries.Values.Min(e => e.StartTime);
                return Mathf.Max(EndTime - minStart, 0.01f);
            }
        }

        public override string ToString()
        {
            return $"{StartTimestamp:HH:mm:ss} - Duration: {Duration:F1}s";
        }
    }
}
