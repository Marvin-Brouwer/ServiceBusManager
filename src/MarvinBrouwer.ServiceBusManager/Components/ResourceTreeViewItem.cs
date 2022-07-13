using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;

namespace MarvinBrouwer.ServiceBusManager.Components;

public abstract class ResourceTreeViewItem : BaseTreeViewItem
{
	public IResource Resource { get; }

	public bool IsDeadLetter { get; protected init; } = false;

	protected ResourceTreeViewItem(IResource resource)
	{
		Resource = resource;
	}
}

