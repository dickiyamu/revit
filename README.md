# Honeybee/Dragonfly for Revit

<b>Honeyebee / Dragonfly plugins for Revit.</b>

# Installation

In order to test this plugin out, please go to [Releases](https://github.com/ladybug-tools/honeybee-revit/releases), and download appropriate version of the ZIP that matches your version of Revit. 

Once you have that ZIP, please remember to **unblock** it before unzipping the contents into the following location: `C:\Users\{username}\AppData\Roaming\Autodesk\Revit\Addins\{version}`
- `Honeybee.Revit.addin` goes directly into that folder
- The rest of the contents in the ZIP file go into folder called `Honeybee.Revit`, so just make a new folder, and copy paste them there.

# Dependencies

Honeybee-Revit has a dependecy on LadybugTools. Please follow these instructions to get that deployed. 

https://github.com/ladybug-tools/lbt-grasshopper/wiki/1.1-Windows-Installation-Steps

Instead of downloading anything from Food4Rhino (as the first step says), you should use this grasshopper installer definition here:

https://github.com/ladybug-tools/lbt-grasshopper/blob/master/installer.gh

Also note that there hasn't been a release of a compatible version of Radiance yet so you can skip all of those Radiance steps and just test out the energy modeling side of things for now.
