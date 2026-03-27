---
name: controller-gen
description: Sinh Controller (DI trực tiếp, [Authorize], thin). Tham chiếu dotnet-controller-convention.md.
version: 1.0
---

# Controller Generation Sub-Skill

> **BẮT BUỘC đọc trước:** `../dotnet-convention-checker/dotnet-controller-convention.md` — checklist CT1–CT10

---

## Input

- Interface + Service đã sinh ở Step 3–4
- FunctionConst constants (nếu có)

---

## Generation Rules

### 1. Template

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BOModule.{Module}
{
    public class {Name}Controller : BaseController
    {
        private readonly I{Name}Service _service;

        public {Name}Controller(I{Name}Service service)
        {
            _service = service;
        }

        /// <summary>
        /// Tìm kiếm danh sách {Name} theo điều kiện lọc.
        /// </summary>
        /// <param name="request">Điều kiện lọc: từ khoá, trạng thái, phân trang.</param>
        /// <returns>Danh sách {Name} phân trang.</returns>
        [HttpGet]
        [Authorize(Roles = FunctionConst.Func{Name}GetListId)]
        [ProducesResponseType(typeof(PaginationResponse<{Name}Response>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Search([FromQuery] {Name}FilterRequest request)
        {
            var response = await _service.Search(request);
            return Response(response);
        }

        /// <summary>
        /// Lấy thông tin chi tiết {Name} theo mã.
        /// </summary>
        /// <param name="code">Mã {Name} cần tra cứu.</param>
        /// <returns>Thông tin chi tiết {Name}.</returns>
        [HttpGet]
        [Route("{code}")]
        [Authorize(Roles = FunctionConst.Func{Name}GetListId)]
        [ProducesResponseType(typeof({Name}Response), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetBy(string code)
        {
            var response = await _service.GetBy(code);
            return Response(response);
        }

        // ── Detail Action (Optional — khi SRS có yêu cầu xem chi tiết) ──

        // Variant A: Route-based Detail (trả single object + child list)
        /// <summary>
        /// Xem chi tiết {Name}.
        /// </summary>
        /// <param name="request">Điều kiện tra cứu chi tiết.</param>
        /// <returns>Thông tin chi tiết đầy đủ.</returns>
        [HttpGet]
        [Route("detail")]
        [Authorize(Roles = FunctionConst.Func{Name}ViewId)]
        [ProducesResponseType(typeof({Name}DetailResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Detail([FromQuery] {Name}DetailRequest request)
        {
            var res = await _service.GetDetail(request);
            return Response(res);
        }

        // Variant B: ID-based Detail (trả simple list, không phân trang)
        /// <summary>
        /// Xem chi tiết {Name} theo ID.
        /// </summary>
        /// <param name="id">ID {Name}.</param>
        /// <returns>Thông tin chi tiết.</returns>
        [HttpGet("{id}")]
        [Authorize(Roles = FunctionConst.Func{Name}GetListId)]
        [ProducesResponseType(typeof({Name}DetailResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Detail(decimal id)
        {
            var res = await _service.GetDetail(id);
            return Response(res);
        }

        // Variant C: Detail-as-Table (trả PaginationResponse — bảng con có phân trang)
        // Dùng khi: SRS có "Xem chi tiết" hiển thị BẢNG DỮ LIỆU CON có phân trang
        /// <summary>
        /// Xem chi tiết {Name} — danh sách con có phân trang.
        /// </summary>
        /// <param name="request">Điều kiện lọc bảng con.</param>
        /// <returns>Danh sách chi tiết phân trang.</returns>
        [HttpGet]
        [Route("detail")]
        [Authorize(Roles = FunctionConst.Func{Name}ViewId)]
        [ProducesResponseType(typeof(PaginationResponse<{Name}HisResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Detail([FromQuery] {Name}HisFilterRequest request)
        {
            var res = await _service.GetListDetail(request);
            return Response(res);
        }

        /// <summary>
        /// Tạo mới {Name}.
        /// </summary>
        /// <param name="request">Thông tin {Name} cần tạo.</param>
        /// <returns>Kết quả tạo mới.</returns>
        [HttpPost]
        [Authorize(Roles = FunctionConst.Func{Name}CreateId)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Create({Name}CreateRequest request)
        {
            await _service.Create(request);
            return Response();
        }

        /// <summary>
        /// Cập nhật thông tin {Name}.
        /// </summary>
        /// <param name="request">Thông tin {Name} cần cập nhật.</param>
        /// <returns>Kết quả cập nhật.</returns>
        [HttpPut]
        [Authorize(Roles = FunctionConst.Func{Name}UpdateId)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update({Name}UpdateRequest request)
        {
            await _service.Update(request);
            return Response();
        }

        /// <summary>
        /// Kích hoạt hoặc khoá {Name}.
        /// </summary>
        /// <param name="request">Mã {Name} cần thay đổi trạng thái.</param>
        /// <returns>Kết quả thay đổi trạng thái.</returns>
        [HttpPatch]
        [Authorize(Roles = FunctionConst.Func{Name}ChangeStatusId)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ActiveOrLock({Name}ChangeStatusRequest request)
        {
            await _service.ActiveOrLock(request);
            return Response();
        }

        /// <summary>
        /// Xoá {Name}.
        /// </summary>
        /// <param name="request">Mã {Name} cần xoá.</param>
        /// <returns>Kết quả xoá.</returns>
        [HttpDelete]
        [Authorize(Roles = FunctionConst.Func{Name}DeleteId)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete({Name}ChangeStatusRequest request)
        {
            await _service.Delete(request.Code);
            return Response();
        }
    }
}
```

### 2. Rules

| Rule                                                | Convention                                                                |
| --------------------------------------------------- | ------------------------------------------------------------------------- |
| DI                                                  | Constructor injection trực tiếp — **KHÔNG dùng `IServiceProvider`**       |
| Base class                                          | `: BaseController`                                                        |
| Authorization                                       | `[Authorize(Roles = FunctionConst.Func{Name}{Action}Id)]` trên MỖI action |
| Return                                              | `Response(data)` hoặc `Response()` — KHÔNG dùng `Ok()` / `BadRequest()`   |
| Thin                                                | Chỉ nhận request → gọi service → trả response                             |
| Search params                                       | `[FromQuery]` trên GET parameters                                         |
| Folder                                              | `Controllers/` (số nhiều)                                                 |
| No `using Microsoft.Extensions.DependencyInjection` | Không cần khi dùng DI trực tiếp                                           |

### 3. FunctionConst (cần tạo kèm nếu chưa có)

```csharp
// Constants/FunctionConst.{Name}.cs
namespace BOModule.{Module};

public partial class FunctionConst
{
    public const string Func{Name}GetListId = "{FUNC_GET_LIST_ID}";
    public const string Func{Name}ViewId = "{FUNC_VIEW_ID}";           // Optional — for Detail action
    public const string Func{Name}CreateId = "{FUNC_CREATE_ID}";
    public const string Func{Name}UpdateId = "{FUNC_UPDATE_ID}";
    public const string Func{Name}ChangeStatusId = "{FUNC_CHANGE_STATUS_ID}";
    public const string Func{Name}DeleteId = "{FUNC_DELETE_ID}";
}
```

---

## Output

- Controller: `modules/BOModule.{Name}/Controllers/{Name}Controller.cs`
- FunctionConst: `modules/BOModule.{Name}/Constants/FunctionConst.{Name}.cs` (if new)
- PHẢI pass checklist CT1–CT10
