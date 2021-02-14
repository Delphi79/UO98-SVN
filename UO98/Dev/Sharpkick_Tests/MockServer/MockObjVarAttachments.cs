using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Sharpkick;

namespace Sharpkick_Tests
{
    static class MockObjVarAttachments
    {
        struct Attachment
        {
            public VariableType Type;
            public string Name;
            public object Value;
        }

        static Dictionary<int, List<Attachment>> Attachments = new Dictionary<int, List<Attachment>>();

        public static void AddAttachment(int serial, VariableType type, string name, object value)
        {
            if (!Attachments.ContainsKey(serial))
                Attachments[serial] = new List<Attachment>();
            if (HasAny(serial, name))
                Remove(serial, name);
            Attachments[serial].Add(
                new Attachment()
                {
                    Type=type,
                    Name=name,
                    Value=value
                });
        }

        public static void DeleteAllFor(int serial)
        {
            if (Attachments.ContainsKey(serial))
                Attachments.Remove(serial);
        }

        public static bool Remove(int serial, string name)
        {
            if (Attachments.ContainsKey(serial))
                Attachments[serial].RemoveAll(att => att.Name == name);

            return true;
        }

        public static bool Has(int serial, VariableType type, string name)
        {
            if (!Attachments.ContainsKey(serial))
                return false;

            IEnumerable<Attachment> list = Attachments[serial].Where(att => att.Name == name && (type == VariableType.Unknown || att.Type == type));
            return list.Count() > 0;
        }

        public static int GetInt(int serial, string name)
        {
            return (int)(GetValue(serial, VariableType.Integer, name) ?? 0);
        }

        public static string GetString(int serial, string name)
        {
            return GetValue(serial, VariableType.String, name) as string ?? string.Empty;
        }

        public static bool GetLocation(int serial, string name, out Location locationResult)
        {
            if (Has(serial, VariableType.Location, name))
            {
                locationResult = (Location)(GetValue(serial, VariableType.Location, name) ?? new Location());
                return true;
            }
            else
            {
                locationResult = new Location();
                return false;
            }
        }

        static object GetValue(int serial, VariableType type, string name)
        {
            if (!Attachments.ContainsKey(serial))
                return false;

            IEnumerable<Attachment> list = Attachments[serial].Where(att => att.Name == name && (type == VariableType.Unknown || att.Type == type));
            return list.FirstOrDefault().Value;
        }


        public static bool HasAny(int serial, string name)
        {
            if (!Attachments.ContainsKey(serial))
                return false;

            return Has(serial, VariableType.Unknown, name);
        }

    }
}
