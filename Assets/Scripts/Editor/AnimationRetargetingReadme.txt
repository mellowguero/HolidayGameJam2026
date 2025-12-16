ANIMATION RETARGETING SYSTEM - USER GUIDE
==========================================

OVERVIEW
--------
This system allows you to retarget animations from one character rig to another by:
1. Creating bone mapping entry assets (individual bone-to-bone mappings)
2. Creating bone mapping collections (arrays of mapping entries)
3. Previewing retargeted animations in real-time
4. Baking and saving retargeted animations as Unity assets

ARCHITECTURE
------------
The system uses a modular ScriptableObject architecture:

1. BoneMappingEntry (ScriptableObject)
   - Individual bone mapping asset
   - Contains source bone path and target bone path
   - Can be reused across multiple mapping collections

2. BoneMapping (ScriptableObject)
   - Collection of BoneMappingEntry assets
   - Contains rig references and source animations
   - Main asset for the retargeting workflow

QUICK START
-----------
1. Open the Animation Retargeting window: Window > Animation Retargeting
2. Create a new Bone Mapping Collection asset
3. Assign Source Rig and Target Rig GameObjects
4. Add source animation clips
5. Generate bone mapping entry assets (auto-generate from table or manually)
6. Preview the retargeted animation
7. Save the baked animations

WORKFLOW STEPS
--------------

STEP 1: CREATE BONE MAPPING COLLECTION
---------------------------------------
- Click "Create New Mapping" in the Animation Retargeting window
- This creates a BoneMapping ScriptableObject
- You can create multiple collections for different rig combinations

STEP 2: ASSIGN RIGS
-------------------
- Source Rig: The GameObject containing the original animation skeleton
- Target Rig: The GameObject you want to apply animations to
- Both can be scene GameObjects or prefab references

STEP 3: ADD SOURCE ANIMATIONS
------------------------------
- Click "Add Animation Clip" to add animation clips from your source rig
- You can add multiple clips to retarget in batch

STEP 4: CREATE BONE MAPPING ENTRIES
------------------------------------
There are three ways to create bone mapping entries:

A. Auto-Generate from Table (RECOMMENDED):
   - Click "Auto-Generate from Table"
   - Set the asset save path (default: Assets/Animations/BoneMappingEntries)
   - Paste mappings in format: "SourceBonePath = TargetBonePath"
   - Each mapping on a new line
   - Click "Generate Mappings"
   - Individual BoneMappingEntry assets are created and added to the collection

B. Manual Asset Creation:
   - Right-click in Project window
   - Create > Animation Retargeting > Bone Mapping Entry
   - Set source and target bone paths
   - Drag the asset into the Bone Mappings array

C. Add Empty and Assign:
   - Click "Add Mapping Asset" in the window
   - Assign an existing BoneMappingEntry asset from the project

STEP 5: PREVIEW ANIMATION
--------------------------
- Select an animation from the dropdown
- Click "Create Preview" to generate a retargeted preview
- Use Play/Pause/Stop controls to view the animation
- Drag the time slider to scrub through frames
- The preview creates a temporary instance in your scene
- Click "Cleanup" to remove the preview instance

STEP 6: SAVE RETARGETED ANIMATIONS
-----------------------------------
- Set the output folder path (e.g., "Assets/Animations/Retargeted")
- Click "Save Retargeted Animations"
- All source animations will be retargeted and saved as new .anim files
- Original animations remain untouched

BONE PATH FORMAT
----------------
Bone paths are hierarchical paths from the rig root:
- Single bone: "Hips"
- Nested bone: "Hips/Spine/Chest"
- Mixamo rig: "mixamorig:Hips/mixamorig:Spine"

To find bone paths:
1. Select the rig in the scene
2. Expand the hierarchy
3. Note the path from root to the desired bone

MAPPING TABLE FORMAT
--------------------
When using Auto-Generate, use this format:

SourceBonePath = TargetBonePath

Example:
mixamorig:Hips = Hips
mixamorig:Spine = Spine
mixamorig:LeftArm = Left_UpperArm
mixamorig:RightLeg = Right_LowerLeg

- Lines starting with # or // are ignored (comments)
- Empty lines are ignored
- Whitespace around = is trimmed

BENEFITS OF MODULAR ARCHITECTURE
---------------------------------
1. Reusability: Individual bone mapping entries can be shared across multiple collections
2. Organization: Easy to manage and update specific bone mappings
3. Flexibility: Mix and match mapping entries for different retargeting scenarios
4. LLM Integration: Auto-generate creates discrete assets for easy inspection
5. Version Control: Individual entries are easier to track in source control

TIPS AND BEST PRACTICES
------------------------
1. Always test with a simple animation first
2. Ensure bone hierarchies match in structure (parent-child relationships)
3. Root bones should map to root bones
4. Preview before baking to verify correctness
5. Keep bone mapping assets organized in dedicated folders
6. Reuse bone mapping collections for multiple animation batches
7. Use descriptive names for mapping assets (e.g., "MixamoToPlayerRig_Collection")
8. Create a mapping entry library for commonly used bone pairs

TROUBLESHOOTING
---------------
Problem: Preview looks incorrect
Solution: Check that bone paths are correct and hierarchies match

Problem: Animation doesn't transfer rotation properly
Solution: Verify parent-child relationships are mapped correctly

Problem: Some bones don't animate
Solution: Ensure those bones are included in the mapping entries

Problem: Preview instance won't delete
Solution: Use "Cleanup" button or manually delete the GameObject with name "_AnimationRetargetingPreview_"

Problem: Mapping entry assets cluttering project
Solution: Organize them in a dedicated folder structure

TECHNICAL DETAILS
-----------------
- Sample Rate: 60 FPS
- Retargeting Method: Direct transform mapping with curve sampling
- Supported: Position and Rotation (local space)
- Output Format: Unity AnimationClip (.anim)

FILES IN THE SYSTEM
-------------------
- BoneMappingEntry.cs: ScriptableObject for individual bone mapping
- BoneMapping.cs: ScriptableObject for mapping collection
- AnimationRetargeter.cs: Core retargeting algorithm
- AnimationRetargetingWindow.cs: Main editor window
- AutoGenerateMappingWindow.cs: Bulk mapping import tool
- BoneMappingEditor.cs: Custom inspector for BoneMapping collection
- BoneMappingEntryEditor.cs: Custom inspector for BoneMappingEntry

For questions or issues, refer to the Unity documentation on Animation and Animation Clips.
