2.2.0

Added:
- Contrast Adaptive sharpening method to Sharpen effect
- Color Grading LUT, vibrancy filter
- Radial Blur, now has a center and angle parameter, allowing for a swirl-like blur
- Kaleidoscope, parameter to set the pivot point, horizontal/vertical splits and to maintain the screen's aspect ratio
- (URP) Render features now have a dropdown to select allowed camera types (defaults to Game & Scene view)
  (URP) Added option to Edge Detection render feature to execute before transparent materials are rendered

Changed:
- Backported depth to world reconstruction method from URP. This affects: Fog, Cloud Shadows, Sketch and Caustics
  	* When the camera is far from the world origin, flickering no longer occurs when looking directly along an axis
  	* Effects now support orthographic cameras
  	* Now support VR rendering (Single Pass Instanced, in URP only)
  
- UI for effects will now show a warning/notification if the primary parameter hasn't been overriden, or is set to 0. 
  	* Clarifies why an effect is not visible.
  	* Primary parameters are now always at the top of the UI, for every effect
  
- Radial Blur, blur amount now stays consistent, regardless of the amount of iterations.
- Sunshafts now maintain a 1:1 aspect ratio
- Fog/Cloud Shadows/Caustics/Sun Shafts now automatically use the brightest directional light (Fog Light Source and Sunshaft Caster components were removed).

Fixed:
- Project from sun option (Caustics/Cloud Shadows) not having any effect in Forward rendering.
- Color Grading LUT, color inversion not actually correct when using Linear color space
- Installer errors when using the asset as a package

2.1.9
The minimum supported version is now 2019.4.0f1

Changed:
- Scripts for the Post-processing and Universal RP are now both in a package, found in the Install folder. By default, the asset contains neither and must be unpacked based on what's being used
- Retired support usage of the Post-processing package with URP 7. This greatly simplifies the code-base.

Fixed:
- Situations where Unity can crash when importing the URP package
- Errors in 2021.2.0 beta regarding GetDisplayTitle, due to changes in the volume framework
- HueShift3D, shader error when building to PS5

2.1.8

Added:
- Support for the URP 2D Renderer in Unity 2021.2.0b1+. Effects that depend on 3D information remain nonfunctional.
- (URP) Edge Detection and Hue Shift 3D now have the option to use the integrated depth-normals texture (higher quality, but slower)

Changed:
- Starting from Unity 2021.2.0b1 (URP 12), effects no longer require to copy the screen contents (except 2D). Halves the performance impact for most effects.
- Retired support for the Lightweight Render Pipeline
- For effects that use a texture, a default texture is now assigned when first adding it.
- Improved directional fog color, will appear to be more stable as the camera rotates around the light's axis.

Fixed:
- Gradient effect displaying incorrect colors in Linear color space
- (URP) Fog directional color not respecting HDR values

2.1.7

Added:
- Custom Effect render feature, this allows custom effects to be created using Shader Graph.
  * The Runtime/Custom folder contains an example graph, and several common-use sub-graphs
- Option to project from the brightest directional light for Caustics and Clouds Shadows
- Luminance threshold parameter to Overlay effect, so it can double as a lens dirt effect

2.1.6

Added:
- Caustics effect, renders animated underwater caustics in a specific height range. Example textures are now included in the _Samples package.
- Distance fade, with start and end distance to Edge Detection, Blur, Kuwahara, LUT and Cloud Shadows effects
- (URP) Exposed options on render features:
	* Always enabled: Effect will render, even if the camera has post-processing disabled
	* Allow in scene view: Effect also renders in the scene view, unless post-processing is disabled in the toolbar
	* Allowed camera types: Base and/or Overlay
Note: It appears render features have to be removed, then added again for these settings to properly work!

Changed:
- Edge Detection, Kuwahara and Color Grading LUT distance fading options now have control over start and end distance (similar to linear fog)
- Tilt Shift, now has an offset and angle parameter for horizontal mode

Fixed:
- (URP) Sharpen effect showing artifacts on edges of objects with highly emissive values
- (URP) LUT effect breaking tonemapping when distance-based blend mode was used

2.1.5

Fixed:
- (URP) Effects not working in URP 10 (2020.2) due to changes in rendering code
- (URP) Blur downsampling resulted in a pixelated image

2.1.4
Unity 2020.2 compatibility verified. All effects now ported to URP. Preemptive(!) support for URP 10, fixed shader errors but custom post processing is generally broken.

Added (URP integrated post processing):
- Fog (excluding light scattering)
- Cloud Shadows
- Lens Flares

Added:
- Eye lid transition gradient to samples package
- Hue Shift 3D, option to use a gradient texture as the color spectrum source

Changed:
- (PPS) Shaders now use libraries from the package, rather than a local copy (remnant of backwards compatibility between Unity 2017/2018)
- Demo scenes can now only be installed when Post Processing Stack is installed when using URP (incompatible with SRP volume system)
- VolumeTrigger sample script was renamed to PostProcessingVolumeTrigger and moved to Runtime/_Examples folder (Avoids error when Post Processing Stack not installed)
- Renamed "_UniversalRenderPipeline" package to "_URP_VolumeSystem", for clarity
- (URP) Effects are now added to the default renderer, instead of the first in the list
- LUT screenshot capture now temporarily disables the LUT effect so the source screenshot is without any color grading

Fixed:
- Tilt Shift Mode/Quality parameters not having an override checkbox
- (URP) Mosaic effect incorrectly being labeled as "Black Bars" in effect stack
- (URP) Warning about deprecated function in URP 9.0+ (Unity 2020.2)
- (URP) Edge Detection, black screen in build when using a Depth Normals method
- Option to disable scene fog in fog effect inspector did not mark the scene as dirty
- Posterize effect HSV mode not displaying correct colors in Linear color space

2.1.3

Fixed:
- (URP) Mosaic effect incorrectly being labeled as "Black Bars" in effect stack

Added (URP integrated post processing):
- Color Split
- Hue Shift 3D
- Double Vision
- Radial Blur
- Speed Lines
- Mosaic
- Tube Distortion
- Kaleidoscope
- Light Streaks

Added:
- Transition effect, steps through a gradient texture to transition to a black screen (samples package now includes pre-made textures for this)
- Posterize effect now has a "HSV mode", allowing Hue/Saturation/Value channels to be individually posterized

Changed:
- LUT effect now uses LUT texture's import settings, rather than forcing it to bilinear filtering

Fixed:
- Installer errors due to packages now having a "-preview.99" version suffix (2020.1+)
- (URP) Tilt Shift effect flickering to gray screen when not in play mode
- Shader compile errors when building using the OpenGLES 2.0 graphics API (built-in render pipeline only)

2.1.2
Unity 2019.4 compatibility verified

Update guide can be found here: http://staggart.xyz/unity/sc-post-effects/scpe-docs/?section=update-guide

Added (URP integrated post processing):
- Danger
- Overlay
- Gradient
- Refraction
- Tilt Shift

Changed:
- (URP) A warning will now be displayed if an effect requires the depth texture, but this is disabled in the URP settings.
- The way setup warnings are displayed in inspectors has been has been further abstracted between PPS and URP (hence all files are modified)
- LUT effect now allows textures to use Point filtering

Fixed:
- Errors in FogLightSource script if post-processing was not installed
- (URP) GUID warning for SharpenEditor script file

2.1.1

Update guide can be found here: http://staggart.xyz/unity/sc-post-effects/scpe-docs/?section=update-guide

Added (URP integrated post processing):
- Pixelize
- Posterize
- Colorize
- Dithering
- Ripples
- Kuwahara
- Black Bars

(Import the _UniversalRenderPipeline package, and unpack it to overwrite the scripts for the Post Processing Stack with URP versions)

Changed:
- (URP) Effects can now automatically be added to the forward renderer through their inspector

Fixed:
- Build error in LUT shader when using OpenGLES
- (URP) Sharpen effect rendering when effect was disabled
- (URP) Build errors for effect shaders not yet ported to URP

2.1.0
Added support for the Universal Render Pipeline's integrated post processing. (Uninstall Post Processing package and re-run installer to make use of this).

Ported effects:
- Blur
- Edge Detection
- LUT
- Scanlines
- Sharpen
- Sketch
- Sunshafts

More effects will follow through consecutive updates. 

Note that the URP doesn't support custom effects to be executed before built-in effects are, because of this LUT will "mute" the Bloom effect. 

Added:
- Radius parameter to Sharpen effect

2.0.0
Minimum required version is now 2019.1.0. Only Post Processing installed through Package Manager is now supported.

DO NOT UPDATE IN EXISTING PROJECTS! Effects will be removed from profiles in any event. Requires to delete SC Post Effects folder before importing!

To ensures compatibility with future Unity versions, folder structure has been changed. This also required to set the minimum compatible version at 2019.1.0

Changed:
- Package now uses an assembly definition
- Default settings for effect now render them invisible. Eg: "opacity" or "intensity" values are 0 by default. 
  This ensures volumes can be faded out, without needing a "base" volume with the effect added in a disabled state.
- The "SCPE" scripting define symbol is no longer necessary and can safely be removed.

Fixed:
- Effects not saving to profiles in Unity 2020.1+, causing them to be removed when Unity closes

Removed:
- Invert color effect, now incorporated into Color Grading LUT effect

1.0.2
Added:
- Tilt Shift
	* Area falloff parameter
	* Performance and Appearance quality modes
	* Debug mode (visualizes the blur area)
- Fog
	* Directional Light color support, creates a faux-atmospheric scattering effect.

Changed:
- Installer restricts Post Processing package to version 2.1.7. Newer versions introduce a bug affecting custom effects.

Fixed:
- LUT, compiler error when building project using OpenGL on MacOS
- Ripples and Pixelize effects no longer increase in strength in a diagonal direction (thanks to user Johannes)

1.0.1
2019.2 compatibility verified. Note that 2019.1+ requires the LWRP/HDRP for VR compatibility.

Fixed:
- LUT
	* Compiler error when building for OpenGLES platforms (mobile platforms remain unsupported, but this at least resolves the error)
- Fog
	* Radial toggle not taking effect when the gradient texture color mode is used

1.0.0
Initial version, read the full details here: http://staggart.xyz/sc-post-effects-leaves-beta

- When using Unity 2018.1+, the installation screen no longer accepts a GitHub installation of the Post Processing Stack. The Package Manager must be used. Existing projects will not be affected.
- Revised demo scene, now showcases almost every effect. Certain environment effects remain excluded since these need a larger context to showcase.
- Documentation is now complete. All effects now feature a documentation button in their inspector, which opens the corresponding web page.
- Some variable names have been alterated for consistency, excisting volume settings may have to be re-adjusted
- Gradient textures with incorrect import settings will now display a warning and "Fix" button in the inspector.
- Remote package version checking now happens asynchronously and won't stall the installer or help window GUI on slow connections or time-outs.
- Some effects will now show the chosen solver or method in the title name when overridden

Changed:
- Dithering
	* No longer uses a procedural dithering matrix, but uses a 32x32 texture instead. Allowing for greater variety at faster speeds. Several sample textures have been added.
- Fog
	* Color parameter now allows for HDR values, enabling emissive fog
- Sunshafts
	* SunshaftCaster component now has an "infinite distance" toggle, to more accuractly match a skybox's sun position
- Cloud Shadows
	* Now samples a texture's Red channel, for easier texture creation
- Refraction 
	* Minor visual improvement to usage with normal maps. 
- Colorize
	* Now executes after the stack so it takes tonemapping into account
- Kuwahara
	* Distance field is now hidden when method is set to "Regular"
	* Minor shader optimization

Fixed:
- Fog
	* Added skybox color mode support for LWRP 5.7.2+ up to Unity 2019.1.2f1. Existing installations using LWRP need to re-run the installation to activate this. Which can be done through the Help window.
- Refraction 
	* Normal map offsetting entire screen in Unity 2017.2 and newer
- Gradient
	* Not respecting alpha values set in color fields, resulting in incorrect opacity behaviour
- Hue Shift 3D
	* Precision artifacts when running the effect for a long period of time
- Color Grading LUT
	* Extracter storing the scene's luminance in the screenshot's alpha channel when using FXAA
- Pixelize
	* Was not taking the screen's aspect ratio into account. Pixels will now always be square. (Note: your existing profiles need to be re-adjusted!)
- Blur
	* (Potential fix) Sometimes receiving the name "Amount" or "Length" when added to the stack
- Lens Flares
	* Corrected behaviour of Threshold parameter in both Linear and Gamma color spaces
- Light Streaks
	* Corrected behaviour of Threshold parameter in both Linear and Gamma color spaces
- Sunshafts
	* Intensity parameter not working with volume blending (Note: your current intensity values need to be re-adjusted!)

0.8.2:
Fixed compilation errors when using LWRP in 2019.1

Fixed:
- Performance regression when using skybox color sampling in the Fog effect

0.8.1:
Added:
- Overlay
	* Now features a "Auto aspect" toggle, to prevent a square texture from being stretched

Fixed:
- Dithering
	* Color blowout when using HDR (occasional occurrence)
- Sketch
	* Screen-space mode will maintain a square ratio for the sketch texture

0.8.0

Added:
- Sketch effect + sample textures
- Fog
	* Light scattering feature
- Colorize
	* Blending modes
- Black Bars
	* Added "max size" parameter

Fixed:
- Several shader compilation warnings
- Single-Pass Stereo Rendering
	* Double Vision
	* Ripples
	* 3D Hue Shift

0.7.2
Tested for compatibility with 2018.3 and Post Processing Stack v2.1.2 and LW/HD SRP 3.0.0

Added:
- Tilt Shift
	* Horizontal and Radial depth of field effect

Changed:
- Black Bars
	* Increased maximum range
- Fog
	* Skybox color will never be sampled from any hidden camera's

Fixed:
- Define symbol not being added for all platforms
- Demo scene profiles
- Lens Flares, build error on Linux

Fixed:
- Incorrect settings on demo scene profiles

0.7.1
*	Demo scenes and samples can now be installed separately.
*	Shaders no longer need specific configuration for either the GitHub or Package Manager installation of the Post Processing Stack. 
	-	This change prepares for Unity 2018.3+ compatibility.
	-	Package folder can now sit anywhere

Added:
- Color Grading LUT
	* Use traditional LUT strips for color grading. Includes the option to blend two LUTs over a distance
	* Utility for extracting a LUT from a scene screenshot
- Speed Lines
	* Cartoon/Anime-style speed intensity effect

0.7.0
- Added tooltips to all effect parameters
- Installer now allows to install the Post Processing GitHub repository in 2018.1+
- The Help window now allows you to reconfigure the shaders for either Post Processing installations (GitHub/Package Manager). This doesn't force 2018.1+ users to use the Package Manager installation anymore.

- Colorize
	* Applies a color gradient to the image (eg. night or thermal vision)
- Fog
	* Added skybox color mode

Changed:
- Light Streaks
	* Fixed intensity parameter behaviour
	* Fixed vertical direction being too short
- Lens Flares
	* Fixed serialization issue
- Color Split
	* Brought offset value into 0-1 range, for consistency
- Posterize
	* Brought intensity value into 0-1 range, for consistency
- Danger
	* Removed refraction parameter 
- Kuwahara
	* Added start/end distance to Depth Fade mode
- Dithering
	* Added Small/Big size options, since values other than 0.5 and 1 induce an interference pattern
	* Effect is now executed before tonemapping for improved luminance threshold response
- Pixelize
	* Reconfigured to work with dynamic resolutions
- Refraction
	* Added option in the absence of a DUDV map, where a supplied normal map can be converted in the shader
- Scanlines
	* Added animation speed parameter

0.6.1
- Checked compatibility with Post Processing Stack v2.0.12
- Minor fixes for macOS and Unity 2018.3 Beta

0.6.0
- Package now features an installer window when importing the package in a new project

Added:
- Edge Detection, start and end distance mode

Fixed:
- Profile serialization issues causing effects to be removed when Unity closes
- Edge Detection: flickering when using a Scriptable Render Pipeline

Changed:
- Edge Detection, optimized per-pixel shader operations
- Edge Detection, merged both sobel solvers into one and added a "thin edges" toggle

0.5.2
- Repacked contents for Unity 2018.1+
- "PostProcessing" layer is automatically added to the project if not present

Fixed:
- Build error for Radial Blur effect
- Build error for Lens Flares effect
- Fog noise streaking over skybox when distance fog is disabled

0.5.1
Added:
- Gradient
	* Blending modes

Fixed:
- Errors messages for some effects when using HDRP
- Threshold behaviour for Light Streaks and Lens Flares
- Sunshafts
	* Directional Light position influencing origin of sun rays
	* High intensity values causing banding artifacts

0.5.0
Added:
- New effects
	* Sunshafts, radiate sun rays from a directional light and skybox
	* Fog, distance and height-based fog with density noise
	* Clouds shadows, projects a texture over the world (cloud example texture included)
- Screen Gradient
	* Color fields option
	* Rotation parameter
- Overlay
	* Blending modes
	* Tiling parameter

Fixed:
- Single-Pass Stereo rendering fixes for:
	* Light Streaks
	* Blur (Box method)

Changed:
- Effects dropdown is now grouped in categories
- All effects are now in the SCPE namespace to avoid possible conflicts
- Edge Detection will show a warning for unsupported modes when using LWRP/HDRP
- Ambient Occlusion 2D and Lightshafts are automatically disabled in VR

Removed:
- Fisheye effect (replaced by Unity's Lens Distortion effect)
- Fog Gradient effect (replaced by Fog effect)

0.4.0
Added:
- Auto installation of Post Processing Stack package in Unity 2018.1+

- New effects:
	* Radial Blur
	* Lens flares
- Light streaks:
	* High quality mode
	* Direction parameter
- Double Vision:
	* Edges mode
- Edge Detection:
	* Distance fade parameter

Changed:
- Lensflares was renamed to "Light streaks"
- Color Washing was renamed to "3D Hue Shift"
- Improved threshold behaviour for Light Streaks effect
- Blur, code improvements
- Certains effect now skip rendering when effect isn't visible

Fixed:
- "DX11 Assertion failed" error for all shaders

0.3.0
Added:
- Volume blending example scene
- Volume trigger example scene

Changed:
- Single-Pass Stereo VR fixes for: 
	*Edge Detection
	*Blur 
	*Overlay
	*Color Split
	*Double Vision
	*LightStreaks
- Color Depth effect was renamed to "Posterize"
- Clearer instructions window in Unity 2018.1

Removed:
- Bloom buffer option from Lens Flare effect

0.2.0
Added:
- Intensity parameter to Screen Gradient and Overlays effects
- Demo scene

0.1.0
Added:
- Blur effect
- Ambient Occlusion 2D effect
- Lens flares effect (expiremental)
- Mosaic effect

Fixed:
- Refraction offsetting entire screen in some cases

Changed:
- Converted sample refraction normal maps to DUDV maps

0.0.1
Initial release