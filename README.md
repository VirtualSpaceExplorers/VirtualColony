
# Nexus Aurora Virtual Colony on Mars

![enter image description here](http://h2847766.stratoserver.net/pydio/public/292cda38e/dl/SpaceSuit-CaliD-Banner.png?ct=true)

The Nexus Aurora Virtual Colony on Mars aims to bring our open-source ideas about living in space into live interactive 3D. Using crowdsourced contributions in a controlled manner, we are aiming to create a visually stunning experience to inspire the next generation to think about actually living on Mars. We want as many platforms and devices to run this experience so that we can both educate and demonstrate the scientific merit and technological feasibility of our innovations.

Using Unity version: [Download => 2019.4.0f1](https://unity3d.com/get-unity/download/archive)

Our 3D models can be authored in anything that supports the FBX format.  Several of our models are made in Blender 2.83--it's open source and free to use! [Download Latest Blender Here](https://www.blender.org/)  Unity needs Blender 2.83 to be the default application when you double-click .blend files for it to import them correctly.


# Main Branch Specifications
This branch uses the Unity builtin rendering pipeline, to support WebGL, PCs, devices, and VR with maximum performance and portability.
 - The main branch retains the option of later upgrading to Unity's Universal Rendering Pipeline (URP), which is a high quality shader pipeline.

## Features of the Main Branch
 - Full WebGL support [playable in-browser at this link](http://cim.lawlorcode.com/virtual-colony-WebGL/)
 - Jetpack support (hold the spacebar) and realistic Mars gravity
 - User interaction with airlocks
 - Level of detail models (3 levels) pulled directly from Blender file
 - [PC controls](https://github.com/Nexus-Aurora/vr-unity-demo/issues/12)
 - [Shader-based atmospheric mars skybox](https://github.com/Nexus-Aurora/vr-unity-demo/issues/11)
 - Seamless tiling texture shader, for centimeter to kilometer detail levels.


# HDRP Branch Specifications
If you don't need WebGL support, and prefer Unity's better looking High Definition Rendering Pipeline (HDRP), the HDRP branch on this server supports that.  Send HDRP pull requests to [DeadlyMagikarps](https://github.com/DeadlyMagikarps/na-unity-virtual-colony-experimental).

## Features of the HDRP Branch
 - HDR skybox on Mars
 - Full "city block" model with farms, larger buildings, and the Mars Tree of Life building.
 - SteamVR input support and teleporting

# Want to Contribute?
**Pick a feature to build** If you look above here you can find features that we need built for the next release or if you have a great idea feel free to start on your own idea in your forked branch and present us with the results. If you have any questions please ask the @Tech Leads or direct message @Orion Lawlor for information on Discord. 

**How to start** First you navigate to the GitHub repository that contains all our open source code! [ Click Here](https://github.com/Nexus-Aurora/na-unity-virtual-colony-experimental "https://github.com/Nexus-Aurora/na-unity-virtual-colony-experimental") Then you click the fork button in the top right.

**You have made your forked repository!** A new repository will show up looking exactly like ours but it yours! Now you can use your favorite git software to get the code from the server to your machine. And develop however you want too. Don't forget to commit! If you need help with setting up your development environment, please ask in the #vr-discussions for help from your fellow developers or make an issue on the github. 

**Feature done?** After you have completed your feature you can create a pull request back to our original repository--we are currently accepting pull requests against the master branch.

# Coding conventions
We will use [Microsoft's coding conventions](<https://github.com/ktaranov/naming-convention/blob/master/C%23 Coding Standards and Naming Conventions.md>) to for maximum readability and easy because mostly every company uses it which means using it here will benefit you for working in companies. 

# Design inspirations
![enter image description here](http://h2847766.stratoserver.net/pydio/public/69fe889fd/dl/Social-Media-Post-Suggestion.png?ct=true)
