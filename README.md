# DiagramDesigner
Screen Recording (alpha): https://vimeo.com/591414378 

A Windows application that intends to make computational design more like sketching than math. The core feature is to run layout optimization algorithms on shape grammars to produce architecture program diagrams that both follow the aesthetic rules and satisfy the program requirements. Most importantly, the shape grammars are to be machine-learned from the sketched examples provided by the designer. 

## Algorithm for Room Finding
First, the lines are broken down so that no two lines intersect either in X shape or T shape. The only from of contact between two lines is end-to-end. In this state, any line without both endpoints being connected to some other lines are removed because they can never be a part of a room. 

Second, we need to find the boundary of the whole geometry. Start with the top-most, right-most point, which is guaranteed to be part of the boundary, then traversal the geometry by always turning the largest corners when the path diverges. In this way, we are guaranteed to return to the starting point, and this path form the boundary. 

Third, iterate through all lines not on this boundary to figure out which ones are inside the boundary and which ones are outside. The ones inside, along with the boundary, are made into a new Fragment object. The rest will go again from the first step and repeat until there is no line left. 

Once we have a Fragment, a traversal algorithms similar to classic depth-first graph traversal is used to find a path from one point on the boundary to another, without going along the boundary of course. This path is guaranteed to split the Fragment into two, generating two smaller Fragment objects. If no such path can be found, this is a room. The area of the room can be easily calculated as its boundary is a known polyline shape. 

## Shape and Rule Definition
Each shape is defined as a graph. A geometry is of a shape if and only if there exists a one-to-one point-to-node mapping (with line segments between points mapped to edges) through which the geometry become identical to the graph definition of the shape. 

There are two parts of a rule definition. The first part consists of the definition of the LHS (left hand shape) and the definition of the RHS (right hand shape). Each node in each shape is labeled in order to express which nodes remains the same from LHS to RHS and which nodes have to be removed or generated. This part of the definition is used to check whether a user-provided rule application sample is valid. 

The second part is the history of the applications of this rule. Each record would contain the exact coordination of each point and which point correspoind to which node. This data is used to compliment the extreme liberal nature of the graph definition of the shapes so that they can be generated following the user's expectations. 

## Approaches for Right Hand Shape Generation
This is a big topic, so at the moment I will only describe my current solution: 
