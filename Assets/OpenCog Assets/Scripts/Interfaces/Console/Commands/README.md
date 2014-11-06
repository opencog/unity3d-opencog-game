# Interfaces.Console.Commands

* Purpose: Contains all the console commands, as monobehaviors which are dragged onto the console. Many of these have not been implemented entirely just yet, and some may be old

* Refactoring TODO: 
    [ ] Determine how to present honestly, eliminate, or implement incomplete code (ie: Can we just put an interface warning on it saying 'this function has not been implemented yet'? That's perfectly possible.)
    * These currently do nothing:
	    [ ] SaveMapCommand (everything is commented out).
		[ ] TalkCommand (what was this supposed to do? Allow us to chat with a NLP?)
    * These are impossible to use with my current information, and behave badly and unsafely, but it appears that they are largely implemented.
		[ ] ActionCommand. The syntax is not very clear, and it is not obvious how it is supposed to be used or for what. There is also no testing for array out of bounds, which tends to crash things in spectacular fun ways. There is also no input validation. Sending a command like /do AGI_Robot self WalkForwardMove self causes things to go boom and complain of un-implemented functions. 
		[ ] UnloadCommand. This is clearly implemented but does no more than throw errors. It also cannot salvage the situation if a robot ends up getting destroyed because of errors elsewhere, which would be its ideal function if ye asked me.
    * These commands use hacky work-arounds and do not behave as anticipated, but at least work:
        [ ] LoadCommand (Can load the robot in)(doesn't load anything concerning the argument it's sent; loads whatever was dragged into its script. Furthermore, once a robot has been created, if things go boom, a second robot cannot be created.
	* These commands appear to be fully functional and good to keep:
	    [x] ListActionsCommand  (Lists all the actions belonging to an agent)(I just added in something to test for a null, and now it shouldn't throw an exception if run without an agent)