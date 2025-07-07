# 1.0.7

- Excludes UPP function call properly in `BepInExUtility.cs`

# 1.0.6

- Clarified plugin location in project
- Added config to `Tools > Unity Project Patcher > Configs`

# 1.0.5

- Tweaked parts of code for older Unity versions
- Conditionally excluded MonoMod and Harmony from before 2020.3
- Added notice that this does not support before 2020.3 yet

# 1.0.4

- Added usages of `ENABLE_BEPINEX` throughout the code to conditionally include code blocks

# 1.0.3

- BepInEx assemblies now require `ENABLE_BEPINEX` to be enabled, rather than defaulting to enabled

# 1.0.2

- Fixed PluginBlockerPatch when it has no functions to patch

# 1.0.1

- BepInEx paths are handled on enable

# 1.0.0

- Initial version