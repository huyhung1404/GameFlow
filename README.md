Game Flow
===

* Hệ thống quản lý luồng game (game flow) sử dụng cấu trúc Addressable Asset của Unity.

## Mục lục

- [Giới thiệu](#giới-thiệu)
- [Cài đặt (UPM Package)](#cài-đặt-upm-package)
- [Cách sử dụng](#cách-sử-dụng)
  - [Các thành phần cơ bản](#các-thành-phần-cơ-bản)
  - [GameCommand API](#gamecommand-api)
  - [UI Flow và FlowInfo](#ui-flow-và-flowinfo)
- [License](#license)

## Giới thiệu
**Game Flow** là một package giúp quản lý vòng đời của các thành phần trong game (như Scene, UI Canvas, Prefab) thông qua hệ thống định tuyến lệnh `GameCommand` đơn giản và mạnh mẽ. Hệ thống được xây dựng dựa trên `ScriptableObject` để định nghĩa cấu hình và hỗ trợ nạp/hủy tài nguyên linh hoạt bằng Addressables.

## Cài đặt (UPM Package)

### Cài đặt qua Git URL

Yêu cầu một phiên bản Unity hỗ trợ tham số đường dẫn (path query parameter) cho git packages. Bạn có thể thêm link `https://github.com/huyhung1404/GameFlow.git` trực tiếp vào **Package Manager**:

![image](https://docs.unity3d.com/uploads/Main/upm-ui-giturl.png)

Hoặc di chuyển đến thư mục `Packages` của dự án và mở file `manifest.json`. Sau đó thêm dòng khai báo package vào trong khối `dependencies`:

```json
{
  "dependencies": {
    "com.huyhung1404.gameflow": "https://github.com/huyhung1404/GameFlow.git"
  }
}
```

## Cách sử dụng

### Các thành phần cơ bản

Để sử dụng package, bạn cần tạo các class kế thừa từ một trong hai lớp cơ sở:
- `GameFlowElement`: Dành cho các thành phần game chung như Scene thông thường hoặc khởi tạo các Prefab quản lý chung của game.
- `UIFlowElement`: (Kế thừa từ `GameFlowElement`) Dành riêng cho các thành phần Giao diện người dùng (UI), hỗ trợ quản lý `Canvas` và thứ tự hiển thị `Sorting Order` trên màn hình.

Các phần tử (Elements) này đều là `ScriptableObject`, chứa thông tin tham chiếu đến Asset (thông qua `AssetReference`) và cấu hình vòng đời của chúng.

### GameCommand API

Package cung cấp class tĩnh `GameCommand` để thao tác và điều khiển luồng game thông qua cú pháp chaining (Builder pattern).

#### 1. Thêm một phần tử (Add Element)
Sử dụng `GameCommand.Add<T>()` để thêm một Scene hoặc khởi tạo Prefab element.

```csharp
public class GameInitializer : MonoBehaviour
{
    void Start()
    {
        // Tải một Scene hoặc Prefab element
        GameCommand.Add<MainSceneElement>()
                   .LoadingID(1) // Bật luồng Loading màn hình ID tương ứng nếu cần
                   .ActiveData(new LevelInitData()) // Truyền dữ liệu khởi tạo
                   .OnCompleted(obj => Debug.Log("Nạp Main Scene thành công!"))
                   .Build();
    }
}
```

#### 2. Tải một phần tử UI (Load UI Element)
Sử dụng `GameCommand.Load<T>()` để hiển thị một `UIFlowElement` đại diện cho một màn hình UI.

```csharp
void OpenMenu()
{
    // Hiển thị phần tử Main Menu UI
    GameCommand.Load<MainMenuElement>()
               .OnCompleted(menu => menu.Open())
               .Build();
}
```

#### 3. Giải phóng phần tử (Release Element)
Sử dụng `GameCommand.Release<T>()` để đóng và giải phóng tài nguyên hệ thống đã cấp phát cho UI hay Scene đó.

```csharp
void CloseMenu()
{
    // Giải phóng phần tử UI
    GameCommand.Release<MainMenuElement>()
               .IgnoreAnimationHide() // Tùy chọn bỏ qua animation ẩn
               .OnCompleted(() => Debug.Log("Đóng Menu"))
               .Build();
}
```

#### 4. Các tiện ích kiểm soát luồng khác
`GameCommand` cũng cung cấp các API hữu ích để kiểm soát luồng tổng thể của ứng dụng:
- `GameCommand.LockFlow()` / `GameCommand.UnlockFlow()`: Khóa hoặc Mở khóa việc thực thi các lệnh luồng mới.
- `GameCommand.EnableKeyBack()` / `GameCommand.DisableKeyBack()`: Bật hoặc Tắt thao tác phím quay lại (Back) trên thiết bị Android/Mobile.
- `GameCommand.LoadingOn(id)` / `GameCommand.LoadingOff(id)`: Kích hoạt hoặc tắt thủ công một màn hình Loading theo ID.

### UI Flow và FlowInfo

Đối với luồng sự kiện UI UI, package cung cấp lớp tĩnh `FlowInfo` thuận tiện cho việc kiểm tra trạng thái hiện hành của lưới Stack UI:
- `FlowInfo.CurrentCanvasCount()`: Lấy số lượng Canvas UI hiện đang mở.
- `FlowInfo.IsTopCanvas<T>()`: Kiểm tra xem element `T` có đang nằm trên cùng của ngăn xếp hay không.
- `FlowInfo.GetCanvas<T>()`: Lấy tham chiếu Component `UnityEngine.Canvas` của element UI tương ứng.
- `FlowInfo.TopElement()`: Lấy thông tin UI Element hiện ở phần trên cùng của màn hình.

## License
Thư viện này phát hành dưới giấy phép khóa MIT (MIT License).
