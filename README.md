# BookHub
I took a class at my college called Software Systems & Analysis. Throughout the semester, you and your group go through the Software Development Life Cycle to design a "system." The group I was a part of designed an application where college students can resell their used textbooks and other related academic materials to other students on their campus.

Since it's not a programming class, there is no requirement to build a functioning version of your application. However, since I had the knowledge and experience, I decided to build a working version of our application just for kicks.

## How It Works
This is a relatively simple application. The client is a progressive web app (PWA) and the server is written in C#. The server and client talk to each other, data is cached and subsequently saved in a database, and it pulls information about the textbooks from the Google Books API based soley from the ISBN. The one major thing both sides lack is encryption, however, I wanted to make sure things were in plain text for easy demonstration for the class.

The server caches the data it receives from the client in objects stored in collections in RAM, and then saves the data to a Mongo database every two minutes. This is done to prevent the server from making queries to the external database every time the client makes a change. 

I made sure to meticulously comment and document the code so that anyone can learn from it and understand my rationale for why I did things certain ways. 

## Setup
To setup the project, you will need:
* [.NET Core 3.0](https://dotnet.microsoft.com/en-us/download/dotnet/3.0)
* [MongoDB](https://www.mongodb.com/try/download/community)
* [XAMPP Server](https://www.apachefriends.org/download.html)

To build the server, run the following commands: 
```
git clone https://github.com/kylecourounis/BookHub.git && cd BookHub/BookHub.Server
dotnet publish "BookHub.Server.csproj" -c Release -o app
```

When you download and run the installer for MongoDB, make sure to check the box in the installer that tells it to install MongoDB Compass. MongoDB Compass is the GUI for MongoDB. Then, open MongoDB Compass and create a database called `BookHub`. The server will automatically create the collections on its first run.

To make the client work properly, you'll need to install XAMPP Server and place the `BookHub.Client` directory inside the `htdocs` folder in your XAMPP installation location.

Then start the server and navigate to `localhost/BookHub.Client` in your browser or your `your.local.IP.address/BookHub.Client` on your phone, and it should serve the page.
