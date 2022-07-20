# <img src="./Resources/app-icon.png" alt="logo" width="25" height="25" /> VTBAIL Queue Manager
  
This is an application to archive/clear/inject queues and topic-subscriptions for any ServiceBus your Azure subscription can access.

## Authentication  
  
Since this is a developer utility the authentication is done by system authentication of your Azure subscription.
There is no need to log in, if you're connected to Azue in visual studio.

## Using the app

To select a resource to manage utilize the tree component:  
<img src="./Resources/Documentation/tree.png" alt="" width="800" />
  
The tree contains a list of configured service busses and expands to their managable resources:  
<img src="./Resources/Icons/topic.png" alt="" width="18" height="18" class="icon"/>`Topics`,
<img src="./Resources/Icons/topic-subscription.png" alt="" width="18" height="18" class="icon"/>`Subscribtions`,
<img src="./Resources/Icons/queue.png" alt="" width="18" height="18" class="icon"/>`Queues` 
and for both `Subscribtions` and `Queues` there is a  
<img src="./Resources/Icons/dead-letter.png" alt="" width="18" height="18" class="icon"/> `dead-letter` node to access 
dead lettered messagges.

### Downloading 

You have the option to download all the `Queue`, `Subscription` or `Dead letter` messages by pressing the `Download` button.  
When pressing the `Download` button a user is prompted with a confirm dialog showing you the amount of messages present at the time.  
<img src="./Resources/documentation/download-01.png" alt="" width="800" />  
  
If you click `OK` all messages will be downloaded to the download folder.   
![](./Resources/documentation/download-02.png)   

To view the downloaded items please click the 
<img src="./Resources/Icons/open-folder.png" alt="" width="18" height="18" class="icon"/>`Download folder` button in the bottom right,   
or select <img src="./Resources/Icons/open-folder.png" alt="" width="18" height="18" class="icon"/>`Open download folder` from the `Actions` menu.

Your files will be downloaded into a zip, the filename will be based on their guid and the file type 
will be based on the content type you set when Queueing the items.
![](./Resources/documentation/download-04.png)  
![](./Resources/documentation/download-05.png)  

> **Note:** Currently these items aren't stored in any particular order because of their names.
> If this ends up being an issue please report a feature request [here](https://github.com/Marvin-Brouwer/ServiceBusManager/issues).  
  
> **Note:** The Azure ServiceBus api doesn't support peeking more than `100` messages, you can page that but that seems to be very inconsistent.
> Because of this, the application only allows you to download 100 items at a time.
> ![](./Resources/documentation/download-06.png)  

### Clearing 

You have the option to clear all the `Queue`, `Subscription` or `Dead letter` messages by pressing the `Clear` button.  
When pressing the `Clear` button a user is prompted with a confirm dialog showing you the amount of messages present at the time.  
<img src="./Resources/documentation/clear-01.png" alt="" width="800" />  
A user is also presented with a checkbox that, when checked, downloads all messages prior to deletions.
  
If you click `OK` all messages will be cleared when any.  
If you check the checkbox all messages will be downloaded to the download folder (see: [Downloading](#downloading)).  
![](./Resources/documentation/clear-02.png)   

> **Note:** The Azure ServiceBus api doesn't support peeking more than `100` messages, you can page that but that seems to be very inconsistent.
> Because of this, the application only allows you to clear 100 items at a time.
> ![](./Resources/documentation/clear-03.png)  

### Re-queueing 

You have the option to re-queue all the distinct `Dead letter` messages by pressing the `Requeue` button.  
When pressing the `Requeue` button a user is prompted with a confirm dialog showing you the amount of messages present at the time.  
<img src="./Resources/documentation/requeue-01.png" alt="" width="800" />  
  
If you click `OK` all messages will be cleared when any.  
If you check the checkbox all messages will be downloaded to the download folder (see: [Downloading](#downloading)).  
![](./Resources/documentation/requeue-02.png)   

> **Note:** The Azure ServiceBus api doesn't support peeking more than `100` messages, you can page that but that seems to be very inconsistent.
> Because of this, the application only allows you to clear 100 items at a time.
> ![](./Resources/documentation/clear-03.png)  

### Uploading 

You have the option to upload `json`/`xml`/`txt` files or a `zip` containing `json`/`xml`/`txt` files to `Queue`s and `Topic`s by pressing the `Upload` button.    
<img src="./Resources/documentation/upload-01.png" alt="" width="800" />  
When pressing the `Upload` button a user is presented with a file browser dialog defaulting to the download folder.
![](./Resources/documentation/upload-02.png)   
  
This will simply process all files selected (unzip the zips) and queue them to the `Queue` or `Topic`.  
It is possible to apply both `json` and `zip` files in one upload, however the `json` files will be presented first.  

If you click `OK` all messages will be uploaded.  
![](./Resources/documentation/upload-03.png)   

### Additional functionality 

Finally there are some additional buttons at the bottom of the application and some menu items with some additonal functionality
 
**Buttons:**
- <img src="./Resources/Icons/info.png" alt="" width="18" height="18" class="icon"/> The help button 
shows this help doc, also available with `F1` and can be closed by pressing `ESC`.<br />
- <img src="./Resources/Icons/open-folder.png" alt="" width="18" height="18" class="icon"/> The open downloads button 
opens windows explorer to the applications download folder. <br />

**Menu:**
- Action
  - **Reload all:** reloads the entire tree as if the application was restarted.
  - **Reload selected item:** reloads the entire tree from this item onward.
  - **Open download folder:** opens windows explorer to the applications download folder.
- Help
  - **Open GitHub repo:** Opens the browser to the GitHub repository where this app is maintained.
  - **Show readme:** Shows this help doc. 

## Icon attributions

All icons are from [flatIcons.com](https://www.flaticon.com/free-icons). 
Attributions are made because we didn't pay for these and flaticon requires us to do so.  


<a href="https://www.flaticon.com/free-icon/unemployment_4840311">
	<img src="./Resources/Icons/app-icon.png" alt="" width="18" height="18" class="icon"/>The application icon
</a><br/>
<a href="https://www.flaticon.com/free-icon/info_785822">
	<img src="./Resources/Icons/info.png" alt="" width="18" height="18" class="icon"/>The help icon
</a><br/>
<a href="https://www.flaticon.com/premium-icon/refresh-button_2267901">
	<img src="./Resources/Icons/refresh-button.png" alt="" width="18" height="18" class="icon"/>The refresh icon
</a><br/>
<a href="https://www.flaticon.com/premium-icon/open-folder_3735134">
	<img src="./Resources/Icons/open-folder.png" alt="" width="18" height="18" class="icon"/>The open download folder icon
</a><br/>
<a href="https://www.flaticon.com/free-icon/hub_984448">
	<img src="./Resources/Icons/servicebus.png" alt="" width="18" height="18" class="icon"/>The servicebus icons
</a><br/>
<a href="https://www.flaticon.com/premium-icon/book_2702096">
	<img src="./Resources/Icons/topic.png" alt="" width="18" height="18" class="icon"/>The topic icon
</a><br/>
<a href="https://www.flaticon.com/premium-icon/open-book_2702154?related_id=2702154">
	<img src="./Resources/Icons/topic-subscription.png" alt="" width="18" height="18" class="icon"/>The topic subscription icon
</a><br/>
<a href="https://www.flaticon.com/premium-icon/books_2702093">
	<img src="./Resources/Icons/queue.png" alt="" width="18" height="18" class="icon"/>The queue icon
</a><br/>
<a href="https://www.flaticon.com/free-icon/books-stack-of-three_29302">
	<img src="./Resources/Icons/dead-letter.png" alt="" width="18" height="18" class="icon"/>The dead letter icon
</a><br/>
<a href="https://www.flaticon.com/free-icon/warning_595067">
	<img src="./Resources/Icons/warning.png" alt="" width="18" height="18" class="icon"/>The prompt warning icon
</a>

<style>
	document, body {
		overflow-x: hidden;
		overflow-wrap: break-word;
	}
	img.icon {
		margin-bottom: -3px;
		margin-right: 3px;
		border: 0px none;
	}
	pre code {
		font-size: .8em;]
	}
</style>
