# Qudini-CloudEmployee-TestApp
The project uploaded is a Windows Store Application that utilizes the Qudini API for Customer Queueing.

The Task: 

Develop a Windows 8.1 tablet app that will fetch a list of customers from an API and display them on screen as detailed below. 

The API is: https://app.qudini.com/api/queue/gj9fs

The Username and password for the API are: 

Username: codetest1
Password: codetest100

The API will need to be authenticated against using HTTP Basic auth and will return a JSON object that contains some queue data containing a list of customer objects. 

The app should make a request to this api and display the list of customers name and their ‘expectedTime' and ordered by the expected time closest time first. 

It should also fetch the profile image of the customer by using the Gravatar Image request api (use their email for this, or show a placeholder if no email is present): https://en.gravatar.com/site/implement/images/

The app should ensure that the list reloads automatically every 30 seconds and that the UI is not blocked. 
