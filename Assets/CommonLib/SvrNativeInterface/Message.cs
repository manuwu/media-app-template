using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SVR
{
    public class Message
    {
        public int messageType;
        private Dictionary<string, int> IntMsg;
        private Dictionary<string, object> ObjMsg;
        private Dictionary<string, bool> BoolMsg;
        private Dictionary<string, string> StringMsg;

        public Message(int type)
        {
            messageType = type;
            IntMsg = new Dictionary<string, int>();
            ObjMsg = new Dictionary<string, object>();
            BoolMsg = new Dictionary<string, bool>();
            StringMsg = new Dictionary<string, string>();
        }
        public Message SetInt(string key, int value)
        {
            IntMsg.Add(key, value);
            return this;
        }
        public int GetInt(string key, int defaultvalue = 0)
        {
            int value = defaultvalue;
            if (IntMsg.ContainsKey(key))
                value = IntMsg[key];
            return value;
        }
        public Message SetObject(string key, object value)
        {
            ObjMsg.Add(key, value);
            return this;
        }
        public object GetObject(string key, object defaultvalue = null)
        {
            object value = defaultvalue;
            if (ObjMsg.ContainsKey(key))
                value = ObjMsg[key];
            return value;
        }
        public Message SetBool(string key, bool value)
        {
            BoolMsg.Add(key, value);
            return this;
        }
        public bool GetBool(string key, bool defaultvalue = false)
        {
            bool value = defaultvalue;
            if (BoolMsg.ContainsKey(key))
                value = BoolMsg[key];
            return value;
        }

        public Message SetString(string key, string value)
        {
            StringMsg.Add(key, value);
            return this;
        }

        public string GetString(string key, string defaultvalue = "")
        {
            string value = defaultvalue;
            if (StringMsg.ContainsKey(key))
                value = StringMsg[key];
            return value;
        }

        public override string ToString()
        {
            string[] intkeys = IntMsg.Keys.ToArray();
            string[] Objkeys = ObjMsg.Keys.ToArray();
            string[] Boolkeys = BoolMsg.Keys.ToArray();
            string[] stringkeys = StringMsg.Keys.ToArray();
            StringBuilder strs = new StringBuilder();
            foreach (var item in intkeys)
            {
                strs.Append("[");
                strs.Append(item);
                strs.Append("]");
                strs.Append(",");
            }
            if(Objkeys.Length > 0)
                strs.Append("\n");
            foreach (var item in Objkeys)
            {
                strs.Append("[");
                strs.Append(item);
                strs.Append("]");
                strs.Append(",");
            }
            if(Boolkeys.Length > 0)
                strs.Append("\n");
            foreach (var item in Boolkeys)
            {
                strs.Append("[");
                strs.Append(item);
                strs.Append("]");
                strs.Append(",");
            }
            if(stringkeys.Length > 0)
                strs.Append("\n");
            foreach (var item in stringkeys)
            {
                strs.Append("[");
                strs.Append(item);
                strs.Append("]");
                strs.Append(",");
            }
            return strs.ToString();
        }

    }
}
