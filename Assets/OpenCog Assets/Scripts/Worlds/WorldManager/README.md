
# OpenCog.Entities.EntityManager  

* Purpose: These files contain parts of the EntityManager class. Each is a protected subclass contained in the public partial class EntityManager wrapper. Each also contains a public interface for exposing those methods.

Notes:  

* These should be used through .* such as .load.AtRunTime();  



### An explanation of how we came to this pattern:
----------------------------------
It is believed that EntityManager will eventually contain a large number of methods which can be grouped into subclasses. 

For example, there may be 20 ways to send data for: loading a character, moving a character, artificially constructing a path, giving orders, etc. 

In order to neatly sort all of these methods, keep them accessible to the outside, and yet prevent anyone else from instantiating these method-group helper-subclasses, we are:

1. Placing the method-group helper-sub-classes (ie: Load) into their own files (breaking them up for easy browsing)
2. Keeping them nested in EntityManager as protected classes (so the outside world can't instantiate them)
3. Using the 'partial class' keyword combo to satisfy points 1 and 2 (each of the files mentioned in part 1 will then be wrapped in `public partial class etc.`)
4. Exposing the protected class's method prototypes to the outside world using a public interface (which can't be instantiated on its own)

