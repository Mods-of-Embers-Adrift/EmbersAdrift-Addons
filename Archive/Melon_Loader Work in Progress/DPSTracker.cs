// DPSTracker.cs
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DPSAddon
{
    public static class DPSTracker
    {
        private static readonly Dictionary<string, DpsEntry> _entries = new Dictionary<string, DpsEntry>();
        private static readonly object _lock = new object();

        public static IReadOnlyDictionary<string, DpsEntry> Entries => _entries;

        public static void RecordDamage(string sourceName, float damage)
        {
            if (string.IsNullOrEmpty(sourceName) || damage <= 0f)
                return;

            lock (_lock)
            {
                if (!_entries.TryGetValue(sourceName, out var entry))
                {
                    entry = new DpsEntry { Name = sourceName };
                    _entries[sourceName] = entry;
                }

                entry.TotalDamage += damage;
                entry.Duration += Time.deltaTime;
                entry.Dps = entry.TotalDamage / Mathf.Max(entry.Duration, 0.1f);
            }
        }

        public static void RecordHealing(string sourceName, float healing)
        {
            if (string.IsNullOrEmpty(sourceName) || healing <= 0f)
                return;

            lock (_lock)
            {
                if (!_entries.TryGetValue(sourceName, out var entry))
                {
                    entry = new DpsEntry { Name = sourceName };
                    _entries[sourceName] = entry;
                }

                entry.TotalHealing += healing;
                entry.Duration += Time.deltaTime;
                entry.Hps = entry.TotalHealing / Mathf.Max(entry.Duration, 0.1f);
            }
        }

        public static List<DpsEntry> GetTopDpsEntries(int count = 10)
        {
            lock (_lock)
            {
                return _entries.Values.OrderByDescending(e => e.Dps).Take(count).ToList();
            }
        }

        public static List<DpsEntry> GetTopHealingEntries(int count = 10)
        {
            lock (_lock)
            {
                return _entries.Values.OrderByDescending(e => e.Hps).Take(count).ToList();
            }
        }

        public static void Reset()
        {
            lock (_lock)
            {
                _entries.Clear();
            }
        }

        public class DpsEntry
        {
            public string Name;
            public float TotalDamage;
            public float TotalHealing;
            public float Duration;
            public float Dps;
            public float Hps;
        }
    }
}
