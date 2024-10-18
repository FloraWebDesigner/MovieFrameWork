# MyMovie
This project is a movie ticket booking application built using Entity Framework. 
It allows users to book movie tickets, manage available movies, viewers, and handle ticket sales efficiently. 
The app prevents overselling by checking the availability of movie tickets before a booking is completed. 
The app supports flexible booking options from both movie and viewer perspectives, creating a robust and user-friendly experience.

Data bias:
- The ticket number doesn't work. I just added the total number, the number of tickets sold and the number of remaining tickets, so the function that shows and lets the user fill in the ticket number when booking a ticket doesn't help, ideally it would be set up to calculate the number of remaining tickets and present a list for the user to choose from. 
- The same user can buy the same movie, and I'm not restricting it here.

Nice features: 
- I designed three booking modes to give the system a powerful entity interaction. Users can enter the booking through the movie interface and the user information interface respectively, without choosing the movie or the user name repeatedly. Of course, the administrator can also enter directly from the ticketing interface and select the movie and user name at the same time.
- I am able to add images for the movie posterse. 
- I made the homepage style to make the project look more complete. 
- I added links to add new movie and add new customer on the booking page to enhance the user experience.

Next steps:
1. To solve the problem of ticket number reality.
2. To solve the problem of uploading new movie images, currently my image update and list image rendering is working.
3. To add search function to user and movie interface




