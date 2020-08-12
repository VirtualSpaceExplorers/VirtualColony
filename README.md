
# Nexus Aurora Virtual Colony on Mars

![enter image description here](http://h2847766.stratoserver.net/pydio/public/292cda38e/dl/SpaceSuit-CaliD-Banner.png?ct=true)

The Nexus Aurora Virtual Colony on Mars aims to bring our open-source ideas about living in space into live interactive 3D. Using crowdsourced contributions in a controlled manner, we are aiming to create a visually stunning experience to inspire the next generation to think about actually living on Mars. We want as many platforms and devices to run this experience so that we can both educate and demonstrate the scientific merit and technological feasibility of our innovations.

Using Unity version: [Download => 2019.4.0f1](https://unity3d.com/get-unity/download/archive)

Our 3D models can be authored in anything that supports the FBX format.  Several of our models are made in Blender 2.83--it's open source and free to use! [Download Latest Blender Here](https://www.blender.org/)  Unity needs Blender 2.83 to be the default application when you double-click .blend files for it to import them correctly.


# Virtual Colony Specifications
This branch uses the Unity builtin rendering pipeline, to support WebGL, PCs, devices, and VR with maximum performance and portability.
 - The main branch retains the option of later upgrading to Unity's Universal Rendering Pipeline (URP), which is a high quality shader pipeline.

## Features
 - Full WebGL support [playable in-browser at this link](http://cim.lawlorcode.com/virtual-colony-WebGL/)
 - Upgraded Cybertruck with amazing drifting capabilities!  (Thanks @TeigRolle!)
 - PostProcess 2 bloom filter for nice fuzzy emissive glowing objects.
 - Fully driveable Cybertruck: WS for throttle, AD for steering, space to jump out.
 - Fully driveable 1S logistics robot: WASD to drive, FR to move the "elbow" joint, QE to move the "wrist" joint, spacebar to jump out.
 - Jetpack support (hold the spacebar) and realistic Mars gravity
 - User interaction with airlocks
 - Level of detail models (3 levels) pulled directly from Blender file
 - [PC controls](https://github.com/Nexus-Aurora/vr-unity-demo/issues/12)
 - [Shader-based atmospheric mars skybox](https://github.com/Nexus-Aurora/vr-unity-demo/issues/11)
 - Seamless tiling texture shader, for centimeter to kilometer detail levels.
 - Real Mars elevation data (from HRSC) for a 100km x 100km area around our landing site.


# HDRP Branch
If you don't need WebGL support, and prefer Unity's better looking High Definition Rendering Pipeline (HDRP), the HDRP version supports that.  See [https://github.com/VirtualSpaceExplorers/VirtualColonyHDRP](https://github.com/VirtualSpaceExplorers/VirtualColonyHDRP).


## Features of the HDRP Branch
 - Audio
 - HDR skybox on Mars
 - Full "city block" model with farms, larger buildings, and the Mars Tree of Life building.
 - SteamVR input support and teleporting

# Want to Contribute?
**Pick a feature to build** If you look above here you can find features that we need built for the next release or if you have a great idea feel free to start on your own idea in your forked branch and present us with the results. If you have any questions please ask the @Tech Leads or direct message @Orion Lawlor for information on Discord--and if you have an improved model, script, or Unity prefab tell us about it and we can get it integrated whether you like git or not!

**How to start with git** First you navigate to the GitHub repository that contains all our open source code! [https://github.com/VirtualSpaceExplorers/VirtualColony](https://github.com/VirtualSpaceExplorers/VirtualColony ) Then you click the fork button in the top right.

**You have made your forked repository!** A new repository will show up looking exactly like ours but it yours! Now you can use your favorite git software to get the code from the server to your machine. And develop however you want too. Don't forget to commit! If you need help with setting up your development environment, please ask in the #vr-discussions for help from your fellow developers or make an issue on the github. 

**Feature done?** After you have completed your feature you can create a pull request back to our original repository--we are currently accepting pull requests against the master branch.

# Coding conventions
We will use [Microsoft's C# coding conventions](<https://github.com/ktaranov/naming-convention/blob/master/C%23 Coding Standards and Naming Conventions.md>) to for maximum readability and easy because mostly every company uses it which means using it here will benefit you for working in companies. 

# Design inspirations
![enter image description here](http://h2847766.stratoserver.net/pydio/public/69fe889fd/dl/Social-Media-Post-Suggestion.png?ct=true)

# Acknowledgements

The source code and assets here are licensed creative commons zero unless otherwise noted.

The original Unity version of Virtual Colony was prepared by @Kosaki Onodera, @NLDukey, and @Bobbbay.  

The terrain data is 100x100km of real HRSC stereo elevations from Mars' east Hellas crater region, combined with CTX optical imagery.  Orion Lawlor lawlor@alaska.edu prepared this elevation data by combining [HRSC elevations, MOLA elevations, and CTX optical images](https://docs.google.com/document/d/1UgCzdZw2R4w5p_X4vAVlrZJwOz1a6WO9EtQ_lVTuwdU/edit), and wrote a detail texture shader, all of which are released to the public domain.

The Cybertruck model began as [STL files on Thingiverse by BREXIT](https://www.thingiverse.com/thing:4000493/files) and are Creative Commons - Attribution - Noncommercial.  It was heavily reworked and rigged for Unity by Orion Lawlor.  @TeigRolle added the WheelCollider support and drive mechanics.  Cybertruck is a product of Tesla, and the real thing probably will be very different from our simulation, although being electric it should be possible to drive it on the surface of Mars.

The 1S logistics robot began as an [Onshape CAD model](https://cad.onshape.com/documents/8fee4f1529602bb55e1d3eb7/w/30baf6f0419c8791741a8e36/e/c707eaa1296f0e26e7522261) by Orion Lawlor, and is public domain.  The 1S container model, and the standardized container concept, are by Albert Ross.  

The audio assets were prepared by Somepoint Sound and Orion Lawlor.

The building models began as FBX files prepared for Nexus Aurora by many people including architecture by Sean Wessels and Koen Kegel, modeling by Vikas Soni, and many others.  

Thanks to the play testing and bug reports by @SpaceInstructor, Sean Wessels, and many other contributors. 

This project would not exist without the leadership of the founder of Nexus Aurora and the Open Source Space Colonization Project, Adrian Moisa @SpaceInstructor.


