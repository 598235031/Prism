using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
namespace Wanghzh.Prism.Regions
{
    public class NavigationParameters : IEnumerable<KeyValuePair<string, object>>
    {
        private readonly List<KeyValuePair<string, object>> entries = new List<KeyValuePair<string, object>>();
        public NavigationParameters()
        {
        }
        public NavigationParameters(string query)
        {
            if (query != null)
            {
                int num = query.Length;
                for (int i = ((query.Length > 0) && (query[0] == '?')) ? 1 : 0; i < num; i++)
                {
                    int startIndex = i;
                    int num4 = -1;
                    while (i < num)
                    {
                        char ch = query[i];
                        if (ch == '=')
                        {
                            if (num4 < 0)
                            {
                                num4 = i;
                            }
                        }
                        else if (ch == '&')
                        {
                            break;
                        }
                        i++;
                    }
                    string str = null;
                    string str2 = null;
                    if (num4 >= 0)
                    {
                        str = query.Substring(startIndex, num4 - startIndex);
                        str2 = query.Substring(num4 + 1, (i - num4) - 1);
                    }
                    else
                    {
                        str2 = query.Substring(startIndex, i - startIndex);
                    }
                    this.Add(str != null ? Uri.UnescapeDataString(str) : null, Uri.UnescapeDataString(str2));
                    if ((i == (num - 1)) && (query[i] == '&'))
                    {
                        this.Add(null, "");
                    }
                }
            }
        }
        public object this[string key]
        {
            get
            {
                foreach (var kvp in this.entries)
                {
                    if (string.Compare(kvp.Key, key, StringComparison.Ordinal) == 0)
                    {
                        return kvp.Value;
                    }
                }
                return null;
            }
        }
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return this.entries.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        public void Add(string key, object value)
        {
            this.entries.Add(new KeyValuePair<string, object>(key, value));
        }
        public override string ToString()
        {
            var queryBuilder = new StringBuilder();
            if (this.entries.Count > 0)
            {
                queryBuilder.Append('?');
                var first = true;
                foreach (var kvp in this.entries)
                {
                    if (!first)
                    {
                        queryBuilder.Append('&');
                    }
                    else
                    {
                        first = false;
                    }
                    queryBuilder.Append(Uri.EscapeDataString(kvp.Key));
                    queryBuilder.Append('=');
                    queryBuilder.Append(Uri.EscapeDataString(kvp.Value.ToString()));
                }
            }
            return queryBuilder.ToString();
        }
    }
}
