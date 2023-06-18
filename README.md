# RSS Feed Notifier as Windows Application
The RSS Feed Notifier for Windows is a simple desktop application that helps you stay updated with your favorite RSS feeds by providing real-time notifications on your Windows desktop.

# Technical specfications of developing Feed Notifier  
Framework:.net framework 4.8.1
IDE: Visual Studio 2022
Language: C#
UI: Winform

# Features
RSS Feed Integration: The application allows you to get latest feed.
Real-time Notifications: Whenever a new item is published in one of your subscribed feeds, the application will display a desktop notification to inform you.
Customizable Settings: Now the settings are hardcoded within the code, customisation can be expected in future.
User-Friendly Interface: The application provides an easy-to-use interface, making it simple for users.
Installation: current version is the basic version it includes only the code. Installer is yet to come. 

# Usage
Build the code and launch the application.
Enter feed url in the textbox and click on "Get Feed" button. 
The feeds published within 24 hours will be listed in the grid below. 
The summary of the feed can be viewed by double clicking on the row header.
The url link to each feed is also available in the grid. 
Refresh interval for checking new updates is set to 5 minutes.
Even if you close the window, the application will run in the background and periodically check for new items in your subscribed RSS feeds.
Whenever a new item is published, a desktop notification will appear on your Windows desktop.
The feed title will be spoken out too.

# Log
Log can be found at the bottom incase of any error or issue.
