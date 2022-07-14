using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using MarvinBrouwer.ServiceBusManager.Azure.Models;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;

namespace MarvinBrouwer.ServiceBusManager.Components;


public abstract class ResourceTreeViewItem : BaseTreeViewItem
{
	public IAzureResource<IResource> Resource { get; }

	protected ResourceTreeViewItem(IAzureResource<IResource> resource)
	{
		Resource = resource;
	}
}

