# APE
APE, or Aria's Performance Enhancements, is a VNyan plugin that attempts to improve the performance of some of the more impactful nodes. In some cases, APE can approximately double VNyan's node graph performance.

# Using
Drop the .dll and the .vnobj file into the Items/Assemblies folder.

# Building
To build, you will need to install VNyan, and adjust the .csproj to point to the correct directories. You will also need to install the NuGet packages for HarmonyX and ILRepack.

# How
APE targets some of the slower nodes, namely the Trigger and Call Trigger nodes, and some of the array/dictionary nodes. By caching the array/dictionary that is used, we are able to significantly improve performance despite the additional overhead of caching the object. This can have a small side effect if the user requests to pack a dictionary, causing a small difference in behavior.

Trigger nodes are improved by separating triggers into "channels", which allows more efficient dispatching when a trigger is called. This does result in a difference in behavior when nodes have multiple incoming activation connections, but those were already fairly buggy in VNyan, so ¯\_(ツ)_/¯

The Parameter system is also reworked, allowing more efficient substitution in nodes. This can cause a small difference in substitution, say, if we substitute `[a[b]]`, and we have a parameter `b` and `a1`, and `b = 1`, then the original parameter system could *theoretically* substitute and end up with the value of `a1`. However, this is unstable because of implementation details of how parameters are substituted, so ¯\_(ツ)_/¯
