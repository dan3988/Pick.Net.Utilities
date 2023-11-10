using System.Collections;
using System.Collections.Specialized;

namespace DotNetUtilities.Collections;

using CCAction = NotifyCollectionChangedAction;
using CCEventArgs = NotifyCollectionChangedEventArgs;
using CCEventHandler = NotifyCollectionChangedEventHandler;

public static class ObservableCollectionHelper
{
	public static void InvokeAdded(this CCEventHandler handler, INotifyCollectionChanged sender, object item, int index)
		=> handler.Invoke(sender, new CCEventArgs(CCAction.Add, item, index));

	public static void InvokeAdded(this CCEventHandler handler, INotifyCollectionChanged sender, IList items, int index)
		=> handler.Invoke(sender, new CCEventArgs(CCAction.Add, items, index));

	public static void InvokeRemoved(this CCEventHandler handler, INotifyCollectionChanged sender, object item, int index)
		=> handler.Invoke(sender, new CCEventArgs(CCAction.Remove, item, index));

	public static void InvokeReplaced(this CCEventHandler handler, INotifyCollectionChanged sender, object? oldItem, object? newItem)
		=> handler.Invoke(sender, new CCEventArgs(CCAction.Replace, oldItem, newItem));

	public static void InvokeReplaced(this CCEventHandler handler, INotifyCollectionChanged sender, object? oldItem, object? newItem, int index)
		=> handler.Invoke(sender, new CCEventArgs(CCAction.Replace, oldItem, newItem, index));

	public static void InvokeReset(this CCEventHandler handler, INotifyCollectionChanged sender)
		=> handler.Invoke(sender, new CCEventArgs(CCAction.Reset));
}
