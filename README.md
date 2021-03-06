[![Build status](https://ci.appveyor.com/api/projects/status/0ohnap9efouupl72?svg=true)](https://ci.appveyor.com/project/pierregillon/backgroundr)

# What is Backgroundr ?
Backgroundr is a Windows application that automatically and periodically download a random Flickr photo and update your desktop background image with it.

# Features

## v0.2
US :
- [x] Remove tokens from UI
- [x] Notify when no photo found on Flickr
- [x] Add button connect/disconnect to link a Flickr account
TS :
- [x] Encrypt tokens in .config file
- [x] Better exception management and user messages
- [x] Incorporate Flickr api tokens in App.config
- [x] Update icon


## v0.1
US/TS :
- [x] Configure a Flickr account
- [x] Filter by tags
- [x] Configure OAuth credentials to access to private photos
- [x] Change the refresh period
- [x] Enable on system startup
- [x] CI

## Next releases
- [ ] Get all photos not only the first 500
- [ ] Wix installer

# Development
Let's talk here about technical details. You might be interested of this section if you want to run the code on your machine.

## How the application is built ?
The application is built in .NET WPF and following : 
- Command Query Response Segregation (CQRS)
- Domain Driven Design (DDD)
- Model-View-ViewModel (MVVM) xaml binding pattern

## Main libraries
* FlickrNet : Flickr library to access photos
* Hardcodet.Wpf.TaskbarNotification : Library to manage task bar in WPF

## Installing & Executing
The easiest way is to open the solution in VS2015 or earlier.

## Running the tests
Tests are written following Behaviour Driven Development (BDD) with xUnit and NSubstitute (Mocks).

# Versioning
The project use [SemVer](http://semver.org/) for versioning. For the versions available, see [the tags on this repository](https://github.com/pierregillon/backgroundr/releases).

# License
This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details
