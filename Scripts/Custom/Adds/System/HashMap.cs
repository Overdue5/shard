using Server.Logging;
using System;
using System.Collections;

namespace Server.Scripts.Custom.Adds.System
{
    public class HashMap<TKey, TValue> : Hashtable
    {
        public HashMap() : base()
        {
            
        }

        public void Add(TKey key, TValue value)
        {
            try
            {
                if (base.Contains(key))
                    base[key] = value;
                else
                    base.Add(key, value);
            }
            catch (Exception e)
            {
                ConsoleLog.Write.Warning(e);
            }

        }

        public TValue Get(TKey key)
        {
            try
            {
                return (TValue) base[key];
            }
            catch (Exception e)
            {
                ConsoleLog.Write.Warning(e);
                return default(TValue);
            }
        }
    }
}
