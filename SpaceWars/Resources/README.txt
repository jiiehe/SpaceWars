Name: Xi Zheng && YiXiong Qin

UID:u1145083 && u0900071

Finished Date: 12/8/2018

Guide for the SpaceWars:
we used method to get all necessary information from the XML file
use that information to create the SpaceWar server which took us almost 2 days because of the some wired issues in XmlReader method
add two new methods in networkController which follows the professor's diagram
according to the diagram provided in the instuction. create new two methods in server project.
and completed a callback method used to deal with the data that get from the client 
implement update method in server project. My partner and I spent much time trying to figure out the logic behind the update method
finally we found that the update method need to firstly get all informaton from the world, and appended into a stringBuilder
Secondly, it needs to clean up the diconected ships and died ships and projectiles
thirdly, check the connected status for every client if connected, send all information to the connected clients, if not remove from 
the client list

our first goal is to make our client to draw a star
after drawing, we draw a ship then we spent a long time on making the ship move around
after that, we started to dealing with data from the client to let the movement of the ship by the commannd we receive
we figured out collison issues we relized the movement of projectiles
we finished clean up method and put it inside of update method in server project 



Extra Features:
adding one more star, star will move as a rectanglur, which will imporve the quality of the game.
player needs to avoid the collsions from the sudden appeared stars,  we spent more than 5 hours to implement this feature.
