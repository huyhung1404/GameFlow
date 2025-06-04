Game Flow
===

* Game flow management system using Unity's addressable asset structure.

## Table of Contents

- [Getting started](#getting-started)
- [Usage](#usage)
- [UPM Package](#upm-package)
    - [Install via git URL](#install-via-git-url)
- [License](#license)

Getting started
---
Install via [UPM package](#upm-package) with git reference.

```csharp
void Demo()
{
    //Add New Element
    GameCommand.Add<DemoElement>().Build();
    
    //Load User Interface Element
    GameCommand.Load<DemoUIElement>().Build();
    
    //Release Element
    GameCommand.Release<DemoElement>().Build();
}
```
Usage
-----
The package exposes a static `GameCommand` API to control flow elements. After
creating your own classes that derive from `GameFlowElement` or `UIFlowElement`,
you can load and release them at runtime as shown below.

```csharp
public class GameInitializer : MonoBehaviour
{
    void Start()
    {
        // Load a scene or prefab element
        GameCommand.Add<MainSceneElement>()
                   .OnCompleted(obj => Debug.Log("Loaded main scene"))
                   .Build();

        // Show a UI element
        GameCommand.Load<MainMenuElement>()
                   .OnCompleted(menu => menu.Open())
                   .Build();
    }
}

// Later, release the UI element
GameCommand.Release<MainMenuElement>().Build();
```

UPM Package
---

### Install via git URL

Requires a version of unity that supports path query parameter for git packages. You can add `https://github.com/huyhung1404/GameFlow.git` to Package Manager

![image](https://docs.unity3d.com/uploads/Main/upm-ui-giturl.png)

or navigate to your project's Packages folder and open the `manifest.json` file. Then add this package somewhere in
the `dependencies` block:

```json
{
  "dependencies": {
    "com.huyhung1404.gameflow": "https://github.com/huyhung1404/GameFlow.git"
  }
}
```

License
---
This library is under the MIT License.
