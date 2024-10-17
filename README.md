# MyMovie
This project is a movie ticket booking application built using Entity Framework. 
It allows users to book movie tickets, manage available movies, viewers, and handle ticket sales efficiently. 
The app prevents overselling by checking the availability of movie tickets before a booking is completed. The app supports flexible booking options from both movie and viewer perspectives, creating a robust and user-friendly experience.

Basically, it has two main processes. 
One is audience independent booking, the process is that the user first sees the movie list from the homepage, checks the movie details. After they decided which movie they want to see, they then enter the booking system by selecting that movie, and provide the viewer information, and the booking is successful.
The other process focuses more on ticket operators, assuming that we want to record all the information of the users who come to the movie, so that when we receive the request for booking tickets, we will first go to the viewer list, select the person, and create a new account if it doesn't exist, so that we can get the user data, and then turn to the booking page from the viewer entity. In this case, we can see all the booking records of each viewer, which is very convenient.
