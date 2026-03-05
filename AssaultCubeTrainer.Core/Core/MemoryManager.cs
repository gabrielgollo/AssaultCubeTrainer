using Memory;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using AssaultCubeTrainer.Game;

namespace AssaultCubeTrainer.Core
{
    /// <summary>
    /// Memory manager with caching to reduce redundant reads
    /// </summary>
    public class MemoryManager
    {
        private Mem _mem;
        private Dictionary<string, CachedValue> _cache;
        private bool _isAttached;

        private class CachedValue
        {
            public object Value { get; set; }
            public DateTime Timestamp { get; set; }
            public TimeSpan TTL { get; set; }

            public bool IsExpired()
            {
                return DateTime.Now - Timestamp > TTL;
            }
        }

        public bool IsAttached => _isAttached;
        public Process? GameProcess { get; private set; }

        public MemoryManager()
        {
            _mem = new Mem();
            _cache = new Dictionary<string, CachedValue>();
        }

        /// <summary>
        /// Attach to game process
        /// </summary>
        public bool AttachToProcess(IGameProfile profile)
        {
            ClearCache();
            int procId = _mem.GetProcIdFromName(profile.ProcessName);

            if (procId <= 0)
            {
                GameProcess = null;
                _isAttached = false;
                return false;
            }

            GameProcess = Process.GetProcessById(procId);
            _mem.OpenProcess(procId);
            _isAttached = true;

            return true;
        }

        /// <summary>
        /// Clear all cached values
        /// </summary>
        public void ClearCache()
        {
            _cache.Clear();
        }

        /// <summary>
        /// Read ViewMatrix with caching (major performance improvement)
        /// </summary>
        public ViewMatrix ReadViewMatrix(string address, TimeSpan cacheDuration)
        {
            string cacheKey = $"viewmatrix_{address}";

            if (_cache.ContainsKey(cacheKey) && !_cache[cacheKey].IsExpired())
            {
                return (ViewMatrix)_cache[cacheKey].Value;
            }

            ViewMatrix matrix = ReadViewMatrixDirect(address);

            _cache[cacheKey] = new CachedValue
            {
                Value = matrix,
                Timestamp = DateTime.Now,
                TTL = cacheDuration
            };

            return matrix;
        }

        /// <summary>
        /// Read ViewMatrix directly from memory (no cache)
        /// </summary>
        private ViewMatrix ReadViewMatrixDirect(string address)
        {
            var matrix = new ViewMatrix();
            byte[] bytes = _mem.ReadBytes(address, 64);

            matrix.m11 = BitConverter.ToSingle(bytes, 0);
            matrix.m12 = BitConverter.ToSingle(bytes, 4);
            matrix.m13 = BitConverter.ToSingle(bytes, 8);
            matrix.m14 = BitConverter.ToSingle(bytes, 12);

            matrix.m21 = BitConverter.ToSingle(bytes, 16);
            matrix.m22 = BitConverter.ToSingle(bytes, 20);
            matrix.m23 = BitConverter.ToSingle(bytes, 24);
            matrix.m24 = BitConverter.ToSingle(bytes, 28);

            matrix.m31 = BitConverter.ToSingle(bytes, 32);
            matrix.m32 = BitConverter.ToSingle(bytes, 36);
            matrix.m33 = BitConverter.ToSingle(bytes, 40);
            matrix.m34 = BitConverter.ToSingle(bytes, 44);

            matrix.m41 = BitConverter.ToSingle(bytes, 48);
            matrix.m42 = BitConverter.ToSingle(bytes, 52);
            matrix.m43 = BitConverter.ToSingle(bytes, 56);
            matrix.m44 = BitConverter.ToSingle(bytes, 60);

            return matrix;
        }

        /// <summary>
        /// Read ViewMatrix without cache (legacy behavior used by ESP).
        /// </summary>
        public ViewMatrix ReadViewMatrix(string address)
        {
            return ReadViewMatrixDirect(address);
        }

        // Direct pass-through methods to Memory.dll
        public int ReadInt(string address) => _mem.ReadInt(address);
        public float ReadFloat(string address) => _mem.ReadFloat(address);
        public string ReadString(string address) => _mem.ReadString(address);
        public byte[] ReadBytes(string address, long length) => _mem.ReadBytes(address, length);
        public bool WriteMemory(string address, string type, string value) => _mem.WriteMemory(address, type, value);
    }
}
