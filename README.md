GameFlow
===
* Game flow management system using Unity's addressable asset structure.

<!-- START doctoc generated TOC please keep comment here to allow auto update -->
<!-- DON'T EDIT THIS SECTION, INSTEAD RE-RUN doctoc TO UPDATE -->
## Table of Contents

- [Getting started](#getting-started)
- [UPM Package](#upm-package)
  - [Install via git URL](#install-via-git-url)
- [License](#license)

<!-- END doctoc generated TOC please keep comment here to allow auto update -->

Getting started
---
Install via [UPM package](#upm-package) with git reference.

```csharp
void Demo()
{

}
```

UPM Package
---
### Install via git URL

Requires a version of unity that supports path query parameter for git packages (Unity >= 2019.3.4f1, Unity >= 2020.1a21). You can add `https://github.com/huyhung1404/com.huyhung1404.gameflow.git` to Package Manager

![image](https://docs.unity3d.com/uploads/Main/upm-ui-giturl.png)

or navigate to your project's Packages folder and open the `manifest.json` file. Then add this package somewhere in
the `dependencies` block:

```json
{
  "dependencies": {
    "com.huyhung1404.gameflow": "https://github.com/huyhung1404/com.huyhung1404.gameflow.git",
    ...
  }
}
```

To update the package, change suffix `#{version}` to the target version.

* e.g. `"com.huyhung1404.gameflow": "https://github.com/huyhung1404/com.huyhung1404.gameflow.git"#2.0.0",`

Or, use [UpmGitExtension](https://github.com/huyhung1404/com.huyhung1404.gameflow) to install and update the package.

License
---
This library is under the MIT License.
