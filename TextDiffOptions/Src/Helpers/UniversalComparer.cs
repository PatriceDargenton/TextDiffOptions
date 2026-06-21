
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;

namespace Comparer.Helper
{
    /// <summary>
    /// Generic comparer that sorts objects by a comma-separated list of field/property names,
    ///  each optionally followed by DESC
    /// </summary>
    public class UniversalComparer<T> : IComparer, IComparer<T>
    {
        private readonly SortKey[] _sortKeys;
        private bool _msgShown = false;
        private readonly string _sortExpression;

        public UniversalComparer(string sort)
        {
            if (string.IsNullOrEmpty(sort)) sort = "";
            _sortExpression = sort;

            Type type = typeof(T);
            string[] props = sort.Split(',');
            _sortKeys = new SortKey[props.Length];

            for (int i = 0; i < props.Length; i++)
            {
                string memberName = props[i].Trim();
                if (memberName.ToLower().EndsWith(" desc"))
                {
                    _sortKeys[i].Descending = true;
                    memberName = memberName.Remove(memberName.Length - 5).TrimEnd();
                }
                _sortKeys[i].MemberName = memberName;
                _sortKeys[i].FieldInfo = type.GetField(memberName);
                if (_sortKeys[i].FieldInfo == null)
                    _sortKeys[i].PropertyInfo = type.GetProperty(memberName);
            }
        }

        public int Compare(object x, object y) => Compare((T)x, (T)y);

        public int Compare(T x, T y)
        {
            if (x == null)
                return y == null ? 0 : -1;
            if (y == null)
                return 1;

            for (int i = 0; i < _sortKeys.Length; i++)
            {
                object value1, value2;
                SortKey sortKey = _sortKeys[i];

                if (sortKey.FieldInfo != null)
                {
                    value1 = sortKey.FieldInfo.GetValue(x);
                    value2 = sortKey.FieldInfo.GetValue(y);
                }
                else
                {
                    if (sortKey.PropertyInfo == null)
                    {
                        if (!_msgShown)
                        {
                            MessageBox.Show(
                                "A sort key was not found: the specified field does not exist\r\n" +
                                "or is not public!\r\n" +
                                typeof(T) + " : " + _sortKeys[i].MemberName + " : " + _sortExpression,
                                "UniversalComparer:Compare",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            _msgShown = true;
                        }
                        return 0;
                    }
                    value1 = sortKey.PropertyInfo.GetValue(x, null);
                    value2 = sortKey.PropertyInfo.GetValue(y, null);
                }

                int res;
                if (value1 == null && value2 == null)
                    res = 0;
                else if (value1 == null)
                    res = -1;
                else if (value2 == null)
                    res = 1;
                else
                    res = ((IComparable)value1).CompareTo(value2);

                if (res != 0)
                {
                    if (sortKey.Descending) res = -res;
                    return res;
                }
            }
            return 0;
        }

        private struct SortKey
        {
            public FieldInfo FieldInfo;
            public PropertyInfo PropertyInfo;
            public bool Descending;
            public string MemberName;
        }
    }
}