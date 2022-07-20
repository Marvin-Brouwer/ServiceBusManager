[//]: # (Header)

[license-url]: /License.md#readme
[license-shield]: https://img.shields.io/badge/license-Apache--2.0-blue.svg?style=flat-square
[repo-stars-url]: https://github.com/Marvin-Brouwer/ServiceBusManager/stargazers
[repo-stars-shield]: https://img.shields.io/github/stars/Marvin-Brouwer/ServiceBusManager.svg?color=brightgreen&style=flat-square

<h1 align="center">
	<img src="/src/MarvinBrouwer.ServiceBusManager/Resources/app-icon.png" alt="logo" width="25" height="25" /> 
	ServiceBusManager
</h1>

<span align="center">

![Application screenshot](./src/MarvinBrouwer.ServiceBusManager/Resources/Documentation/base-plate.png)

</span>

<h3 align="center">

[![Stars][repo-stars-shield]][repo-stars-url] [![License][license-shield]][license-url]

</h3>

[//]: # (TOC)

<h3 align="center">

[Getting started](#getting-started) - [Using the app](#using-the-app)

</h3>
<hr/>

[//]: # (Document)

This is a management tool for reading and writing to Azure ServiceBuses.  
It's like the [AzureStorageExplorer](https://github.com/microsoft/AzureStorageExplorer#readme) but instead of Queues and Tables this application supports ServiceBuses (and it's Queues and Topics).  

## Getting started

Currently there's no installation present.  
To use this application you'll need to checkout the repository and run:  

```text
cd {repo location}
dotnet run --project ./src/MarvinBrouwer.ServiceBusManager --configuration Release
```

A Windows installer will be created ASAP: [#5](https://github.com/Marvin-Brouwer/ServiceBusManager/issues/5).

## Using the app

There is a readme for application specific usage available here: [Application readme](./src//MarvinBrouwer.ServiceBusManager/Readme.md).  
This readme is also available in the application when pressing `F1` or the help button/menu.  

## Contributing

> We are currently figuring out what the best branching model is, and we're still fleshing out release, contribution and development guidelines.  
> **Suggestions on how to do this are very welcome.**  
  
If you want to help out the project, you are very welcome to.  
Please read our [Contribution guidelines](/docs/Contributing.md#readme) and [contributor covenant's code of conduct](https://www.contributor-covenant.org) before starting.
For maintainers we have a separate [Maintenance guide](/docs/Maintaining.md#readme).  

### Release management

Maintainers are responsible for the release cycles.
However, if you register a bug blocking your development you can request an alpha package version for your branch to be deployed while the code is in review.

For more information, review the [Maintenance guides Release management chapter](/docs/Maintaining.md#release-management).  

### Contributors

<a href="https://github.com/Marvin-Brouwer/ServiceBusManager/graphs/contributors">
  <img src="https://contrib.rocks/image?repo=Marvin-Brouwer/ServiceBusManager" />
</a>

Made with [contrib.rocks](https://contrib.rocks).


## Icon attributions

All icons are from [flatIcons.com](https://www.flaticon.com/free-icons). 
Attributions are made because we didn't pay for these and flaticon requires us to do so.  

<a href="https://www.flaticon.com/free-icon/unemployment_4840311">
	<img src="/src/MarvinBrouwer.ServiceBusManager/Resources/Icons/app-icon.png" alt="" width="18" height="18" class="icon"/> The application icon
</a><br/>
<a href="https://www.flaticon.com/free-icon/info_785822">
	<img src="/src/MarvinBrouwer.ServiceBusManager/Resources/Icons/info.png" alt="" width="18" height="18" class="icon"/> The help icon
</a><br/>
<a href="https://www.flaticon.com/premium-icon/refresh-button_2267901">
	<img src="/src/MarvinBrouwer.ServiceBusManager/Resources/Icons/refresh-button.png" alt="" width="18" height="18" class="icon"/> The refresh icon
</a><br/>
<a href="https://www.flaticon.com/premium-icon/open-folder_3735134">
	<img src="/src/MarvinBrouwer.ServiceBusManager/Resources/Icons/open-folder.png" alt="" width="18" height="18" class="icon"/> The open download folder icon
</a><br/>
<a href="https://www.flaticon.com/free-icon/hub_984448">
	<img src="/src/MarvinBrouwer.ServiceBusManager/Resources/Icons/servicebus.png" alt="" width="18" height="18" class="icon"/> The servicebus icons
</a><br/>
<a href="https://www.flaticon.com/premium-icon/book_2702096">
	<img src="/src/MarvinBrouwer.ServiceBusManager/Resources/Icons/topic.png" alt="" width="18" height="18" class="icon"/> The topic icon
</a><br/>
<a href="https://www.flaticon.com/premium-icon/open-book_2702154?related_id=2702154">
	<img src="/src/MarvinBrouwer.ServiceBusManager/Resources/Icons/topic-subscription.png" alt="" width="18" height="18" class="icon"/> The topic subscription icon
</a><br/>
<a href="https://www.flaticon.com/premium-icon/books_2702093">
	<img src="/src/MarvinBrouwer.ServiceBusManager/Resources/Icons/queue.png" alt="" width="18" height="18" class="icon"/> The queue icon
</a><br/>
<a href="https://www.flaticon.com/free-icon/books-stack-of-three_29302">
	<img src="/src/MarvinBrouwer.ServiceBusManager/Resources/Icons/dead-letter.png" alt="" width="18" height="18" class="icon"/> The dead letter icon
</a><br/>
<a href="https://www.flaticon.com/free-icon/warning_595067">
	<img src="/src/MarvinBrouwer.ServiceBusManager/Resources/Icons/warning.png" alt="" width="18" height="18" class="icon"/> The prompt warning icon
</a>
