using System;
using UnityEngine;

namespace UnityEditor
{
    [Serializable]
    public class SpriteNameFileIdPair : IEquatable<SpriteNameFileIdPair>
    {
        [SerializeField]
        private string m_Name;
        [SerializeField]
        private long m_FileId;

        public string name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        public long fileId
        {
            get { return m_FileId; }
            set { m_FileId = value; }
        }

        internal SpriteNameFileIdPair() {}

        internal SpriteNameFileIdPair(string name, long fileId)
        {
            this.name = name;
            this.fileId = fileId;
        }

        public override int GetHashCode()
        {
            return (name ?? string.Empty).GetHashCode() ^ fileId.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var pair = obj as SpriteNameFileIdPair;
            return pair != null && Equals(pair);
        }

        public bool Equals(SpriteNameFileIdPair pair)
        {
            return pair != null && name == pair.name && fileId == pair.fileId;
        }
    }
}
