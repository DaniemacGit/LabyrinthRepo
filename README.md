# Rollio
**Description:** A physics-driven mobile game created in Unity where players tilt their phone to maneuver a ball around a map
containing obstacles to drop into the finish hole and move onto more intense levels.

## Milestone 1:
- Basic gameplay functionality and UI concept/layout created
- Determined architecture and technology stack

## Milestone 2:
- Session gameplay completed with at least one playable level
- UI navigates to some pages

## Milestone 3:
- TBD

# Contributions



**Tanisq (Tanisqgg)**
-  11/14: added frontend - home page and levels page

**Daniel (Daniemac1)**


**Bao (baoxiong840)**
- 11/14: added to frontend - settings and exit page
- 11/14: added to `README.md` - app title, description, and tracking milestones

**Misty (mistyw1)**



*todo-daniel*
1️⃣ Add a method in SceneChanger for each new level

In SceneChanger.cs, create one method per level exactly like these:

public void LoadLevel3CompletePage()
{
    PlayerPrefs.SetString("SelectedLevel", "Level3");
    PlayerPrefs.Save();
    SceneManager.LoadScene("complete_page");
}

public void LoadLevel4CompletePage()
{
    PlayerPrefs.SetString("SelectedLevel", "Level4");
    PlayerPrefs.Save();
    SceneManager.LoadScene("complete_page");
}


IMPORTANT:

The string "Level3" must match the actual scene name of the level.

You need one method per level.

2️⃣ On the Levels Page, wire each level button

For button Level 3:

Select the Level 3 button

Go to Button → OnClick() → +

Drag in the SceneChanger object

Pick:

👉 SceneChanger → LoadLevel3CompletePage()

Repeat for Level 4, Level 5, etc.

3️⃣ Add Reset script to each level’s player

Open Level3 scene → select the Player object.

In Inspector:

Reset script values:

Current Level Scene Name: "Level3"

Complete Page Scene Name: "complete_page"

Example:

public string currentLevelSceneName = "Level3";
public string completePageSceneName = "complete_page";


Repeat for Level4, Level5, etc.

This ensures:

Time is recorded correctly

Stars are saved under the right level key

You return to the correct complete_page

4️⃣ Ensure Layer setup is consistent

For the Reset script to detect win/loss:

Layer 6: obstacles / fall reset

Layer 7: GOAL object

Tell him:
➡️ “Make sure the goal object in every level is Layer 7.”

5️⃣ Make sure every new level scene is added to Build Settings

Go to:

File → Build Settings → Scenes In Build

Add:

Level3

Level4

etc.

Otherwise Unity will not load the scene.

6️⃣ Stars display will update automatically

No changes needed here — your CompletePageController already handles:

Hiding stars for unfinished levels

Showing correct star count for finished levels

Reading PlayerPrefs key: "Level3_Stars" etc.

As long as steps 1–3 are followed, stars will work for every new level.

⭐ FINAL “SEND TO PARTNER” CHECKLIST (COPY THIS)

For every new level you create:

SceneChanger

Add a method like LoadLevel3CompletePage()

Save "SelectedLevel" = "Level3"

Load "complete_page"

Levels Page

Hook the Level3 button to LoadLevel3CompletePage()

Inside Level 3 Scene

Player object → Reset script

currentLevelSceneName = "Level3"

completePageSceneName = "complete_page"

Goal object must be Layer 7

Build Settings

Add Level3 scene to Build Settings

