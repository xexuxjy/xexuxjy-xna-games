The following are some notes on the Gladius Model Importer project and it's structure and should give an overview


Folders :

*Assets/Editor - Directory contains scripts for the unity asset preprocessor to convert gladius data to unity format.
Assets/GCModels - Directory in which to put gamecube model files to have them converted to unity models. the files need to end in the extension .bytes
Assets/Resources/GladiusAnims - animation files - more can be added as long as the file name ends in .bytes , same anim data is used on all platforms
Assets/Resources/Materials - directory for materials created during import process
*Assets/Resources/Textures - Texture files needed to create models, these need to be added 'manually' or from the texture packs
Assets/Resources/GCModelPrefabs - output directory for models created from gamecube asset files
Assets/Resources/XboxModelPrefabs - output directory for models created from xbox asset files
*Assets/Scripts - scripts for using gladius data files and doing animations etc
Assets/XboxModels - Directory in which to put xbox model files to have them converted to unity models. the files need to end in the extension .bytes

Scenes :

ModelAnimationExample - shows how a model can be setup to play an animation - default is barbarian with idle anim.


All the folders marked * are essential for things to work,others are situational, you can easily build new scenes or projects using this structure as a core
