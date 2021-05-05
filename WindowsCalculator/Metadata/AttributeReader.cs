using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.ComponentModel;

namespace WindowsCalculator.Metadata
{
    public static class AttributeReader
    {
        public static string GetDescription<T>(T instance)
        {
            try
            {
                string result = instance.ToString();

                Type type = instance.GetType();
                MemberInfo[] members = type.GetMember(result);

                if (members == null || members.Length == 0)
                    return result;

                return GetDescription(members[0]);
            }
            catch(ArgumentNullException e)
            {
                return e.Message;
            }            
        }

        public static string GetDescription(MemberInfo info)
        {
            try
            {
                string result = info.Name;

                object[] attributes = info.GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attributes == null || attributes.Length == 0)
                    return result;

                if (!(attributes[0] is DescriptionAttribute description) || string.IsNullOrEmpty(description.Description))
                    return result;

                return description.Description;
            }
            catch (ArgumentNullException e)
            {
                return e.Message;
            }            
        }

        public static string GetAbbreviation<T>(T instance)
        {
            try
            {
                string result = instance.ToString();

                Type type = instance.GetType();
                MemberInfo[] members = type.GetMember(result);

                if (members == null || members.Length == 0)
                    return result;

                return GetAbbreviation(members[0]);
            }
            catch (ArgumentNullException e)
            {
                return e.Message;
            }            
        }

        public static string GetAbbreviation(MemberInfo info)
        {
            try
            {
                string result = info.Name;

                object[] attributes = info.GetCustomAttributes(typeof(AbbreviationAttribute), false);
                if (attributes == null || attributes.Length == 0)
                    return result;

                if (!(attributes[0] is AbbreviationAttribute abbreviation) || string.IsNullOrEmpty(abbreviation.Text))
                    return result;

                return abbreviation.Text;
            }
            catch(ArgumentNullException e)
            {
                return e.Message;
            }
        }
    }
}
