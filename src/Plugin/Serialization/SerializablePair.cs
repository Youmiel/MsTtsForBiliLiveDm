using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MsTtsForBiliLiveDm.Plugin
{
    [XmlType("KeyValue"), XmlRoot("KeyValue")]
    public class SerializablePair<K, V>
    {
        public K Key { get; set; }
        public V Value { get; set; }

        public SerializablePair<K,V> Clone()
        {
            return new SerializablePair<K, V>() { Key = this.Key, Value = this.Value };
        }
    }

}
