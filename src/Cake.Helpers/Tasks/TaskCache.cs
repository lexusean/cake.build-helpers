using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cake.Core;

namespace Cake.Helpers.Tasks
{
  public class HelperTask : IHelperTask
  {
    protected const string DefaultTaskCategory = "Generic";
    protected const string DefaultTaskType = "Unknown";

    private string _Category = DefaultTaskCategory;
    /// <inheritdoc />
    public string Category
    {
      get
      {
        if (string.IsNullOrWhiteSpace(this._Category))
        {
          this._Category = DefaultTaskCategory;
        }

        return this._Category;
      }
      set
      {
        this._Category = value;
      }
    }

    private string _TaskType = DefaultTaskType;
    /// <inheritdoc />
    public string TaskType
    {
      get
      {
        if (string.IsNullOrWhiteSpace(this._TaskType))
        {
          this._TaskType = DefaultTaskType;
        }

        return this._TaskType;
      }
      set
      {
        this._TaskType = value;
      }
    }

    /// <inheritdoc />
    public bool IsTarget { get; set; }

    /// <inheritdoc />
    public ActionTask Task { get; set; }

    /// <inheritdoc />
    public string TaskName
    {
      get { return this.Task?.Name; }
    }
  }

  public class TaskCache : IDictionary<string, IHelperTask>
  {
    #region Private Fields

    private ConcurrentDictionary<string, IHelperTask> _Cache = new ConcurrentDictionary<string, IHelperTask>();

    #endregion

    #region IDictionary Members

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<string, IHelperTask>> GetEnumerator()
    {
      return this._Cache.GetEnumerator();
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
      return ((IEnumerable) this._Cache).GetEnumerator();
    }

    /// <inheritdoc />
    public void Add(KeyValuePair<string, IHelperTask> item)
    {
      this._Cache.AddOrUpdate(item.Key, item.Value, (s, task) => item.Value);
    }

    /// <inheritdoc />
    public void Clear()
    {
      this._Cache.Clear();
    }

    /// <inheritdoc />
    public bool Contains(KeyValuePair<string, IHelperTask> item)
    {
      return this._Cache.Contains(item);
    }

    /// <inheritdoc />
    public void CopyTo(KeyValuePair<string, IHelperTask>[] array, int arrayIndex)
    {
      
    }

    /// <inheritdoc />
    public bool Remove(KeyValuePair<string, IHelperTask> item)
    {
      return this.Remove(item.Key);
    }

    /// <inheritdoc />
    public int Count
    {
      get { return this._Cache.Count; }
    }

    /// <inheritdoc />
    public bool IsReadOnly
    {
      get { return false; }
    }

    /// <inheritdoc />
    public bool ContainsKey(string key)
    {
      return this._Cache.ContainsKey(key);
    }

    /// <inheritdoc />
    public void Add(string key, IHelperTask value)
    {
      this.Add(new KeyValuePair<string, IHelperTask>(key, value));
    }

    /// <inheritdoc />
    public bool Remove(string key)
    {
      return this._Cache.TryRemove(key, out IHelperTask task);
    }

    /// <inheritdoc />
    public bool TryGetValue(string key, out IHelperTask value)
    {
      return this._Cache.TryGetValue(key, out value);
    }

    /// <inheritdoc />
    public IHelperTask this[string key]
    {
      get { return this._Cache[key]; }
      set { this._Cache.GetOrAdd(key, value); }
    }

    /// <inheritdoc />
    public ICollection<string> Keys
    {
      get { return this._Cache.Keys; }
    }

    /// <inheritdoc />
    public ICollection<IHelperTask> Values
    {
      get { return this._Cache.Values; }
    }

    #endregion
  }
}
