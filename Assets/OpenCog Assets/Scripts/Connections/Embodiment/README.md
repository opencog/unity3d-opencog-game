# Embodiment 

* Purpose: Handles connecting the in-game avatar to the OpenCog-side embodiment. Also displays information about the avatar's psychological state based on what we've learned from the OpenCog side. 

* REfactoring TODO:
    [ ] It is possible the guiskin should belong elsewhere.
	[ ] It is possible that Tagging/EmbodimentXMLTags (seperate, more world-oriented concept), and the FeelingPanel (interface) belong in other folders; but for now they will be kept here. The ConnectorSingleton should probably just be called The Connector. 